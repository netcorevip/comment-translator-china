﻿namespace RangeTree
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// The async range tree implementation. Keeps a root node and forwards all queries to it. Whenenver new items are added or items are removed, the tree goes "out of sync" and when the next query is started, the tree is being rebuilt in an async task. During the rebuild, queries are still done on the old tree plus on the items currently not part of the tree. If items were removed, these are filtered out. there is no need to wait for the rebuild to finish in order to return the query results.
    /// </summary>
    /// <typeparam name="TKey">The type of the range.</typeparam>
    /// <typeparam name="T">The type of the data items.</typeparam>
    public class RangeTreeAsync<TKey, T> : IRangeTree<TKey, T>
        where TKey : IComparable<TKey>
        where T : IRangeProvider<TKey>
    {
        private RangeTree<TKey, T> rangeTree;
        private List<T> addedItems = new List<T>();
        private List<T> removedItems = new List<T>();
        private List<T> addedItemsRebuilding = new List<T>();
        private List<T> removedItemsRebuilding = new List<T>();
        private IComparer<T> rangeComparer;
        private Task rebuildTask;
        private CancellationTokenSource rebuildTaskCancelSource;
        private bool isRebuilding;

        private object locker = new object();

        /// <summary>
        /// Gets the items in the tree.
        /// </summary>
        /// <value>
        /// The items.
        /// </value>
        public IEnumerable<T> Items
        {
            get { return this.rangeTree.Items.Concat(this.addedItemsRebuilding).Concat(this.addedItems); }
        }

        /// <summary>
        /// Gets the count of all items in the tree.
        /// </summary>
        /// <value>
        /// The count.
        /// </value>
        public int Count
        {
            get { return this.rangeTree.Count + this.addedItemsRebuilding.Count + this.addedItems.Count; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RangeTreeAsync{TKey, T}"/> class.
        /// </summary>
        /// <param name="rangeComparer">The range comparer.</param>
        public RangeTreeAsync(IComparer<T> rangeComparer)
        {
            this.rangeTree = new RangeTree<TKey, T>(rangeComparer) { AutoRebuild = false };
            this.rangeComparer = rangeComparer;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RangeTreeAsync{TKey, T}"/> class.
        /// </summary>
        /// <param name="items">The items.</param>
        /// <param name="rangeComparer">The range comparer.</param>
        public RangeTreeAsync(IEnumerable<T> items, IComparer<T> rangeComparer)
        {
            this.rangeTree = new RangeTree<TKey, T>(items, rangeComparer) { AutoRebuild = false };
            this.rangeComparer = rangeComparer;
        }

        /// <summary>
        /// Performans a "stab" query with a single value. All items with overlapping ranges are returned.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The resulting <see cref="List{T}"/></returns>
        public List<T> Query(TKey value)
        {
            // check if we need to start a rebuild task
            if (this.NeedsRebuild())
            {
                this.RebuildTree();
            }

            lock (this.locker)
            {
                // query the tree (may be out of date)
                var results = this.rangeTree.Query(value);

                // add additional results
                results.AddRange(this.addedItemsRebuilding.Where(item => item.Range.Contains(value)));
                results.AddRange(this.addedItems.Where(item => item.Range.Contains(value)));

                return this.FilterResults(results);
            }
        }

        /// <summary>
        /// Performans a range query. All items with overlapping ranges are returned.
        /// </summary>
        /// <param name="range">The range.</param>
        /// <returns>The resulting <see cref="List{T}"/></returns>
        public List<T> Query(Range<TKey> range)
        {
            // check if we need to start a rebuild task
            if (this.NeedsRebuild())
            {
                this.RebuildTree();
            }

            lock (this.locker)
            {
                // query the tree (may be out of date)
                var results = this.rangeTree.Query(range);

                 // add additional results
                results.AddRange(this.addedItemsRebuilding.Where(item => item.Range.Intersects(range)));
                results.AddRange(this.addedItems.Where(item => item.Range.Intersects(range)));

                return this.FilterResults(results);
            }
        }

        /// <summary>
        /// Filter out results, if items were removed since the last rebuild.
        /// </summary>
        /// <param name="results">The results.</param>
        /// <returns>The resulting <see cref="List{T}"/></returns>
        private List<T> FilterResults(List<T> results)
        {
            if (this.removedItemsRebuilding.Count > 0 || this.removedItems.Count > 0)
            {
                var hs = new HashSet<T>(results);
                foreach (var item in this.removedItemsRebuilding)
                {
                    hs.Remove(item);
                }

                foreach (var item in this.removedItems)
                {
                    hs.Remove(item);
                }

                results = hs.ToList();
            }

            return results;
        }

        /// <summary>
        /// Adds the specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        public void Add(T item)
        {
            lock (this.locker)
            {
                this.addedItems.Add(item);
            }
        }

        /// <summary>
        /// Adds the specified items.
        /// </summary>
        /// <param name="items">The items.</param>
        public void Add(IEnumerable<T> items)
        {
            lock (this.locker)
            {
                this.addedItems.AddRange(items);
            }
        }

        /// <summary>
        /// Removes the specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        public void Remove(T item)
        {
            lock (this.locker)
            {
                this.removedItems.Add(item);
            }
        }

        /// <summary>
        /// Removes the specified items.
        /// </summary>
        /// <param name="items">The items.</param>
        public void Remove(IEnumerable<T> items)
        {
            lock (this.locker)
            {
                this.removedItems.AddRange(items);
            }
        }

        /// <summary>
        /// Clears the tree (removes all items).
        /// </summary>
        public void Clear()
        {
            lock (this.locker)
            {
                this.rangeTree.Clear();
                this.addedItems = new List<T>();
                this.removedItems = new List<T>();
                this.addedItemsRebuilding = new List<T>();
                this.removedItemsRebuilding = new List<T>();

                if (this.rebuildTaskCancelSource != null)
                {
                    this.rebuildTaskCancelSource.Cancel();
                }
            }
        }

        /// <summary>
        /// Start the rebuild task if a rebuild is necessary.
        /// </summary>
        public void Rebuild()
        {
            if (this.NeedsRebuild())
            {
                this.RebuildTree();
            }
        }

        /// <summary>
        /// Rebuilds the tree by starting an async task.
        /// </summary>
        private void RebuildTree()
        {
            lock (this.locker)
            {
                // if a rebuild is in progress return
                if (this.isRebuilding || this.addedItems.Count == 0)
                {
                    return;
                }

                this.isRebuilding = true;
            }

            this.rebuildTaskCancelSource = new CancellationTokenSource();

            this.rebuildTask = Task.Factory.StartNew(
                () =>
                {
                    lock (this.locker)
                    {
                        // store the items to be added, we need this if a query takes places
                        // before we are finished rebuilding
                        this.addedItemsRebuilding = this.addedItems.ToList();
                        this.addedItems.Clear();

                        // store the items to be removed ...
                        this.removedItemsRebuilding = this.removedItemsRebuilding.ToList();
                        this.removedItems.Clear();
                    }

                    // all items of the tree
                    var allItems = this.rangeTree.Items.ToList();
                    allItems.AddRange(this.addedItemsRebuilding);

                    // we may have to remove some
                    foreach (var item in this.removedItemsRebuilding)
                    {
                        allItems.Remove(item);
                    }

                    // build the new tree
                    var newTree = new RangeTree<TKey, T>(allItems, this.rangeComparer) { AutoRebuild = false };

                    // if task was not cancelled, set the new tree as the current one
                    if (!this.rebuildTaskCancelSource.Token.IsCancellationRequested)
                    {
                        lock (this.locker)
                        {
                            this.rangeTree = newTree;
                            this.addedItemsRebuilding.Clear();
                            this.removedItemsRebuilding.Clear();
                        }
                    }
                    else
                    {
                        // nop
                    }
                }, this.rebuildTaskCancelSource.Token)
            .ContinueWith(task =>
            {
                // done with rebuilding, do we need to start again?
                this.isRebuilding = false;

                if (this.NeedsRebuild())
                {
                    this.RebuildTree();
                }
            });
        }

        /// <summary>
        /// Checks whether a rebuild is necessary.
        /// </summary>
        /// <returns>[true] is needs rebuild, otherwise [false]</returns>
        private bool NeedsRebuild()
        {
            lock (this.locker)
            {
                // only if count of added or removed items is > 100
                // otherwise, the sequential query is ok
                return this.addedItems.Count > 100 || this.removedItems.Count > 100;
            }
        }
    }
}
