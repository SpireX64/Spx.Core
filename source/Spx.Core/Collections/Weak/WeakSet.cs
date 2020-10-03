using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Spx.Collections.Weak
{
    public class WeakSet<T> : IWeakCollection<T>, ISet<T> where T : class?
    {
        private readonly HashSet<WeakReference<T>?> _realSet = new HashSet<WeakReference<T>?>();
        private uint _version = 0;

        public bool Contains(T item)
        {
            foreach (var weakReference in _realSet)
            {
                if (weakReference is null)
                {
                    if (item is null)
                        return true;
                    continue;
                }

                if (!weakReference.TryGetTarget(out var reference)) continue;

                if (reference!.Equals(item))
                    return true;
            }

            return false;
        }

        public void Add(T item)
        {
            AddItem(item);
        }

        bool ISet<T>.Add(T item)
        {
            return AddItem(item);
        }

        public void Clear()
        {
            if (_realSet.Count == 0) return;
            IncrementVersion();
            _realSet.Clear();
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            _realSet.Select(weakRef => weakRef is null || !weakRef.TryGetTarget(out var item) ? null : item)
                .ToArray()
                .CopyTo(array, arrayIndex);
        }

        public bool Remove(T item)
        {
            var removedCount = _realSet.RemoveWhere(weakRef =>
            {
                if (weakRef is null)
                    return item is null;

                return weakRef.TryGetTarget(out var reference) && reference!.Equals(item);
            });

            if (removedCount == 0)
                return false;

            IncrementVersion();
            return true;
        }

        public int Count
        {
            get
            {
                RemoveDeadReferences();
                return _realSet.Count;
            }
        }

        public bool IsReadOnly { get; } = false;

        public void Add(T item, bool trackResurrection)
        {
            AddItem(item, trackResurrection);
        }

        public bool RemoveDeadReferences()
        {
            var numberOfRemovedRefs = _realSet.RemoveWhere(
                weakRef => (weakRef != null && !weakRef.TryGetTarget(out _)));
            return numberOfRemovedRefs > 0;
        }

        public IReadOnlyCollection<T> ToReadOnly() => new ReadOnlyWeakCollection<T>(this);

        private void IncrementVersion()
        {
            _version += 1;
        }

        private bool AddItem(T item, bool trackResurrection = false, bool incrementVersion = true)
        {
            if (item is null)
            {
                if (incrementVersion) IncrementVersion();
                return _realSet.Add(null);
            }

            if (Contains(item))
                return false;

            var weakRef = new WeakReference<T>(item, trackResurrection);
            if (incrementVersion) IncrementVersion();
            return _realSet.Add(weakRef);
        }

        public void ExceptWith(IEnumerable<T> other)
        {
            var itemsRemoved = _realSet.RemoveWhere(weakRef =>
            {
                T item = null;
                if (weakRef != null && !weakRef.TryGetTarget(out item))
                    return false;

                foreach (var otherItem in other)
                {
                    if (weakRef is null || item is null)
                        return otherItem is null;

                    return item.Equals(otherItem);
                }

                return false;
            });

            if (itemsRemoved > 0)
                IncrementVersion();
        }

        public void IntersectWith(IEnumerable<T> other)
        {
            var itemsRemoved = _realSet.RemoveWhere(weakRef =>
            {
                T item = null;
                if (weakRef != null && !weakRef.TryGetTarget(out item))
                    return false;

                foreach (var otherItem in other)
                {
                    if (weakRef is null || item is null)
                    {
                        return otherItem != null;
                    }

                    return !item.Equals(otherItem);
                }

                return true;
            });

            if (itemsRemoved > 0)
                IncrementVersion();
        }

        public bool IsProperSubsetOf(IEnumerable<T> other)
        {
            return this.All(other.Contains);
        }

        public bool IsProperSupersetOf(IEnumerable<T> other)
        {
            return other.All(this.Contains);
        }

        public bool IsSubsetOf(IEnumerable<T> other)
        {
            return this.All(other.Contains);
        }

        public bool IsSupersetOf(IEnumerable<T> other)
        {
            return other.All(this.Contains);
        }

        public bool Overlaps(IEnumerable<T> other)
        {
            return other.Any(Contains);
        }

        public bool SetEquals(IEnumerable<T> other)
        {
            var enumerable = other.ToList();
            if (_realSet.Count != enumerable.Count)
                return false;
            return this.All(enumerable.Contains) && enumerable.All(this.Contains);
        }

        public void SymmetricExceptWith(IEnumerable<T> other)
        {
            var enumerable = other.ToList();
            if (enumerable.Count == 0) return;

            var itemsToDelete = new LinkedList<T>();

            // Check other enumerable
            foreach (var item in this.Where(thisItem => enumerable.Contains(thisItem)))
            {
                itemsToDelete.AddLast(item);
            }

            // Check this enumerable
            foreach (var item in enumerable.Where(Contains))
            {
                itemsToDelete.AddLast(item);
            }

            ExceptWith(itemsToDelete);
        }

        public void UnionWith(IEnumerable<T> other)
        {
            var itemsToAdd = other.ToList();
            if (itemsToAdd.Count == 0) return;

            var anyAdded = false;
            foreach (var item in itemsToAdd)
                anyAdded |= AddItem(item);

            if (anyAdded)
                IncrementVersion();
        }

        public IEnumerator<T> GetEnumerator()
        {
            return new WeakSetEnumerator(this, _version);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public struct WeakSetEnumerator : IEnumerator<T>
        {
            private const byte WeakSetEnumeratorStateNotStarted = 0;
            private const byte WeakSetEnumeratorStateInProcess = 1;
            private const byte WeakSetEnumeratorStateFinished = 2;
            private const byte WeakSetEnumeratorStateDisposed = 3;

            private readonly WeakSet<T> _weakSet;
            private HashSet<WeakReference<T>?>.Enumerator _enumerator;
            private readonly uint _weakSetVersion;
            private byte _state;
            private T _current;

            public WeakSetEnumerator(WeakSet<T> weakSet, uint weakSetVersion)
            {
                _state = WeakSetEnumeratorStateNotStarted;
                _weakSet = weakSet;
                _enumerator = weakSet._realSet.GetEnumerator();
                _weakSetVersion = weakSetVersion;
                _current = null!;
            }

            public bool MoveNext()
            {
                if (_state == WeakSetEnumeratorStateFinished)
                    return false;

                CheckEnumerator();

                if (_state == WeakSetEnumeratorStateNotStarted)
                    _state = WeakSetEnumeratorStateInProcess;

                while (_enumerator.MoveNext())
                {
                    if (_enumerator.Current == null || !_enumerator.Current.TryGetTarget(out var item)) continue;

                    _current = item;
                    return true;
                }

                _state = WeakSetEnumeratorStateFinished;
                return false;
            }

            public void Reset()
            {
                _state = WeakSetEnumeratorStateNotStarted;
                _enumerator = _weakSet._realSet.GetEnumerator();
                _current = null!;
            }

            public T Current
            {
                get
                {
                    CheckValueAccess();
                    return _current;
                }
            }

            object IEnumerator.Current => Current!;

            public void Dispose()
            {
                _state = WeakSetEnumeratorStateDisposed;
                _current = null!;
            }

            private void CheckValueAccess()
            {
                if (_weakSet == null || _state == WeakSetEnumeratorStateDisposed)
                    throw new InvalidOperationException("Enumerator was disposed");


                if (_weakSet._version != _weakSetVersion)
                    throw new InvalidOperationException("Enumerator version failed");

                if (_state == WeakSetEnumeratorStateNotStarted)
                    throw new InvalidOperationException("Enumeration not started");

                if (_state == WeakSetEnumeratorStateFinished)
                    throw new InvalidOperationException("Enumeration was finished");
            }

            private void CheckEnumerator()
            {
                if (_weakSet == null || _state == WeakSetEnumeratorStateDisposed)
                    throw new InvalidOperationException("Enumerator was disposed");

                if (_weakSet._version != _weakSetVersion)
                    throw new InvalidOperationException("Enumerator version failed");
            }
        }
    }
}