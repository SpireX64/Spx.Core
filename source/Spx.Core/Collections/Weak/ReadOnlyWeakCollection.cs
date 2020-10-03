using System.Collections;
using System.Collections.Generic;

namespace Spx.Collections.Weak
{
    /**
     * WeakCollection read-only wrapper to prevent cast to original mutable collection
     */
    public class ReadOnlyWeakCollection<T> : IReadOnlyWeakCollection<T> where T : class?
    {
        private readonly IWeakCollection<T> _collectionRef;
        
        public ReadOnlyWeakCollection(IWeakCollection<T> weakCollection)
        {
            _collectionRef = weakCollection;
        }
        
        public bool Contains(T item) => ((ICollection<T>) _collectionRef).Contains(item);

        public IEnumerator<T> GetEnumerator() => _collectionRef.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => _collectionRef.GetEnumerator();

        public int Count => ((IReadOnlyCollection<T>)_collectionRef).Count;
    }
}