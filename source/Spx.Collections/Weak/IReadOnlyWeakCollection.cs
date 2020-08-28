using System.Collections.Generic;

namespace Spx.Collections.Weak
{
    public interface IReadOnlyWeakCollection<T> : IReadOnlyCollection<T> where T : class?
    {
        public bool Contains(T item);
    }
}