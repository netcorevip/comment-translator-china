namespace RangeTree
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Defines the <see cref="IRangeTree{TKey, T}" />
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="T"></typeparam>
    public interface IRangeTree<TKey, T>
        where TKey : IComparable<TKey>
        where T : IRangeProvider<TKey>
    {
        /// <summary>
        /// Gets the Items
        /// </summary>
        IEnumerable<T> Items { get; }

        /// <summary>
        /// Gets the Count
        /// </summary>
        int Count { get; }

        /// <summary>
        /// The Query
        /// </summary>
        /// <param name="value">The value<see cref="TKey"/></param>
        /// <returns>The <see cref="List{T}"/></returns>
        List<T> Query(TKey value);

        /// <summary>
        /// The Query
        /// </summary>
        /// <param name="range">The range<see cref="Range{TKey}"/></param>
        /// <returns>The <see cref="List{T}"/></returns>
        List<T> Query(Range<TKey> range);

        /// <summary>
        /// The Rebuild
        /// </summary>
        void Rebuild();

        /// <summary>
        /// The Add
        /// </summary>
        /// <param name="item">The item<see cref="T"/></param>
        void Add(T item);

        /// <summary>
        /// The Add
        /// </summary>
        /// <param name="items">The items<see cref="IEnumerable{T}"/></param>
        void Add(IEnumerable<T> items);

        /// <summary>
        /// The Remove
        /// </summary>
        /// <param name="item">The item<see cref="T"/></param>
        void Remove(T item);

        /// <summary>
        /// The Remove
        /// </summary>
        /// <param name="items">The items<see cref="IEnumerable{T}"/></param>
        void Remove(IEnumerable<T> items);

        /// <summary>
        /// The Clear
        /// </summary>
        void Clear();
    }
}
