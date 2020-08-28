using System.Collections.Generic;

namespace Spx.Collections.Weak
{
    public interface IWeakCollection<T> : IReadOnlyWeakCollection<T>, ICollection<T> where T : class?
    {
        /// <summary>
        /// Add item's weak reference to collection
        /// </summary>
        /// <param name="item">Item to add</param>
        /// <param name="trackResurrection"></param>
        void Add(T item, bool trackResurrection = false);

        /// <summary>
        /// Start process of detect & remove dead references
        /// </summary>
        /// <returns>true, если одна или несколько ссылок были удалены</returns>
        bool RemoveDeadReferences();

        IReadOnlyCollection<T> ToReadOnly();
    }
}