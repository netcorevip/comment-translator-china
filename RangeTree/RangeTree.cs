namespace RangeTree
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// The standard range tree implementation. Keeps a root node and forwards all queries to it. Whenenver new items are added or items are removed, the tree goes "out of sync" and is rebuild when it's queried next.
    /// </summary>
    /// <typeparam name="TKey">The type of the range.</typeparam>
    /// <typeparam name="T">The type of the data items.</typeparam>
    public class RangeTree<TKey, T> : IRangeTree<TKey, T>
        where TKey : IComparable<TKey>
        where T : IRangeProvider<TKey>
    {
        private RangeTreeNode<TKey, T> root;
        private List<T> items;
        private bool isInSync;
        private bool autoRebuild;
        private IComparer<T> rangeComparer;

        /// <summary>
        /// Gets a value indicating whether the tree is currently in sync or not. If it is "out of sync"  you can either rebuild it manually (call Rebuild) or let it rebuild automatically when you query it next.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is in synchronize; otherwise, <c>false</c>.
        /// </value>
        public bool IsInSync
        {
            get { return this.isInSync; }
        }

        /// <summary>
        /// Gets all of the tree items.
        /// </summary>
        /// <value>
        /// The items.
        /// </value>
        public IEnumerable<T> Items
        {
            get { return this.items; }
        }

        /// <summary>
        /// Gets the count of all tree items.
        /// </summary>
        /// <value>
        /// The count.
        /// </value>
        public int Count
        {
            get { return this.items.Count; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether rebuild automatically. Defaults to true.
        /// </summary>
        /// <value>
        ///   <c>true</c> if [automatic rebuild]; otherwise, <c>false</c>.
        /// </value>
        public bool AutoRebuild
        {
            get { return this.autoRebuild; }
            set { this.autoRebuild = value; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RangeTree{TKey, T}"/> class.
        /// </summary>
        /// <param name="rangeComparer">The range comparer.</param>
        public RangeTree(IComparer<T> rangeComparer)
        {
            this.rangeComparer = rangeComparer;
            this.root = new RangeTreeNode<TKey, T>(rangeComparer);            
            this.items = new List<T>();
            this.isInSync = true;
            this.autoRebuild = true;    
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RangeTree{TKey, T}"/> class.
        /// </summary>
        /// <param name="items">The items.</param>
        /// <param name="rangeComparer">The range comparer.</param>
        public RangeTree(IEnumerable<T> items, IComparer<T> rangeComparer)
        {
            this.rangeComparer = rangeComparer;
            this.root = new RangeTreeNode<TKey, T>(items, rangeComparer);
            this.items = items.ToList();
            this.isInSync = true;
            this.autoRebuild = true;
        }

        /// <summary>
        /// Performans a "stab" query with a single value. All items with overlapping ranges are returned.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The resulting <see cref="List{T}"/></returns>
        public List<T> Query(TKey value)
        {
            if (!this.isInSync && this.autoRebuild)
            {
                this.Rebuild();
            }

            return this.root.Query(value);
        }

        /// <summary>
        /// Performans a range query. All items with overlapping ranges are returned.
        /// </summary>
        /// <param name="range">The range.</param>
        /// <returns>The resulting <see cref="List{T}"/></returns>
        public List<T> Query(Range<TKey> range)
        {
            if (!this.isInSync && this.autoRebuild)
            {
                this.Rebuild();
            }

            return this.root.Query(range);
        }

        /// <summary>
        /// Rebuilds the tree if it is out of sync.
        /// </summary>
        public void Rebuild()
        {
            if (this.isInSync)
            {
                return;
            }

            this.root = new RangeTreeNode<TKey, T>(this.items, this.rangeComparer);
            this.isInSync = true;
        }

        /// <summary>
        /// Adds the specified item. Tree will go out of sync.
        /// </summary>
        /// <param name="item">The item.</param>
        public void Add(T item)
        {
            this.isInSync = false;
            this.items.Add(item);
        }

        /// <summary>
        /// Adds the specified items. Tree will go out of sync.
        /// </summary>
        /// <param name="items">The items.</param>
        public void Add(IEnumerable<T> items)
        {
            this.isInSync = false;
            this.items.AddRange(items);
        }

        /// <summary>
        /// Removes the specified item. Tree will go out of sync.
        /// </summary>
        /// <param name="item">The item.</param>
        public void Remove(T item)
        {
            this.isInSync = false;
            this.items.Remove(item);
        }

        /// <summary>
        /// Removes the specified items. Tree will go out of sync.
        /// </summary>
        /// <param name="items">The items.</param>
        public void Remove(IEnumerable<T> items)
        {
            this.isInSync = false;

            foreach (var item in items)
            {
                this.items.Remove(item);
            }
        }

        /// <summary>
        /// Clears the tree (removes all items).
        /// </summary>
        public void Clear()
        {
            this.root = new RangeTreeNode<TKey, T>(this.rangeComparer);            
            this.items = new List<T>();
            this.isInSync = true;
        }
    }
}
