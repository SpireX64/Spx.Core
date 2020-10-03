using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;

namespace Spx.Collections.Weak
{
    public class WeakCollection<T> : IWeakCollection<T> where T : class?
    {
        private readonly LinkedList<WeakReference<T>?> _realCollection = new LinkedList<WeakReference<T>?>();
        
        private int _version;

        public bool Remove(T item)
        {
            var node = FindItemNode(item);

            if (node == null)
                return false;
            
            IncrementVersion();
            _realCollection.Remove(node);
            return true;
        }

        public int Count
        {
            get
            {
                RemoveDeadReferences();
                return _realCollection.Count;
            }
        }

        public bool IsReadOnly { get; } = false;

        public void Add(T item)
        {
            Add(item, false);
        }

        public void Clear()
        {
            if (_realCollection.Count == 0) return;
            IncrementVersion();
            _realCollection.Clear();
        }

        public bool Contains(T item)
        {
            return FindItemNode(item) != null;
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            if (array is null)
                throw new ArgumentNullException();
            
            if (arrayIndex < 0)
                throw new ArgumentOutOfRangeException(nameof(arrayIndex), "Index is less then 0");

            if (arrayIndex + this.Count > array.Length)
                throw new ArgumentException(
                    "Number of elements is collection is greater then available space of the destination array",
                    nameof(array));
            
            var index = arrayIndex;
            foreach (var item in this)
            {
                array[index++] = item;
            }
        }

        public void Add(T item, bool trackResurrection)
        {
            IncrementVersion();
            _realCollection.AddLast(item is null
                ? null
                : new WeakReference<T>(item, trackResurrection));
        }

        public bool RemoveDeadReferences()
        {
            if(_realCollection.Count == 0) return false;

            var isAnyRemoved = false;
            var node = _realCollection.First;
            do
            {
                var weakRef = node.Value;

                if (weakRef == null || weakRef.TryGetTarget(out _))
                {
                    node = node.Next;
                    continue;
                }

                var nextNode = node.Next;
                _realCollection.Remove(node);
                isAnyRemoved = true;
                node = nextNode;
            } while (node != null);
            return isAnyRemoved;
        }

        public IReadOnlyCollection<T> ToReadOnly() => new ReadOnlyWeakCollection<T>(this);

        public IEnumerator<T> GetEnumerator()
        {
            RemoveDeadReferences();
            return new WeakCollectionEnumerator(this, _version);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private LinkedListNode<WeakReference<T>?>? FindItemNode(T item)
        {
            if (_realCollection.Count == 0) return null;
            
            var iterableNode = _realCollection.First;
            do
            {
                var weakRef = iterableNode.Value;
                if (weakRef is null)
                {
                    if (item == null)
                        return iterableNode;
                } else if (weakRef.TryGetTarget(out var weakItem) && weakItem == item)
                    return iterableNode;

                iterableNode = iterableNode.Next;
            } while (iterableNode != null);

            return null;
        }
        
        private void IncrementVersion()
        {
            Interlocked.Increment(ref _version);
        }

        private struct WeakCollectionEnumerator: IEnumerator<T>
        {
            private const byte EnumerationStateNotStarted = 0;
            private const byte EnumerationStateInProcess = 1;
            private const byte EnumerationStateFinished = 2;
            
            private readonly WeakCollection<T> _collection;
            private readonly int _collectionVersion;
            private byte _state;
            private LinkedListNode<WeakReference<T>?>? _currentNode;
            private T _current;

            internal WeakCollectionEnumerator(WeakCollection<T> collection, int version)
            {
                _collection = collection;
                _collectionVersion = version;
                _state = EnumerationStateNotStarted;
                _currentNode = null;
                _current = null!;
            }

            private void UpdateCurrentValue()
            {
                var weakRef = _currentNode!.Value;
                if (weakRef != null && weakRef.TryGetTarget(out var value))
                    _current = value;
                else
                    _current = null!;
            }
            
            public bool MoveNext()
            {
                CheckCollectionVersion();

                switch (_state)
                {
                    case EnumerationStateNotStarted:
                    {
                        if (_collection._realCollection.Count == 0)
                        {
                            _state = EnumerationStateFinished;
                            return false;
                        }

                        _currentNode = _collection._realCollection.First;
                        UpdateCurrentValue();
                        _state = EnumerationStateInProcess;
                        return true;
                    }
                    
                    case EnumerationStateInProcess:
                    {
                        CheckEnumerationNotFinished();
                        _currentNode = _currentNode!.Next;
                        if (_currentNode == null)
                        {
                            _state = EnumerationStateFinished;
                            Dispose();
                            return false;
                        }
                        
                        UpdateCurrentValue();
                        return true;
                    }
                }

                return false;
            }

            public void Reset()
            {
                CheckCollectionVersion();
                Dispose();
                _state = EnumerationStateNotStarted;
            }

            private void CheckCollectionVersion()
            {
                if (_collectionVersion != _collection._version)
                    throw new InvalidOperationException("Collection version mismatch");
            }

            private void CheckEnumerationStated()
            {
                if (_state == EnumerationStateNotStarted)
                    throw new InvalidOperationException("Enumeration not started");
            }

            private void CheckEnumerationNotFinished()
            {
                if (_state == EnumerationStateFinished)
                    throw new InvalidOperationException("Enumeration already finished");
            }
                        
            public T Current
            {
                get
                {
                    CheckEnumerationStated();
                    CheckEnumerationNotFinished();
                    return _current;
                }
            }

            object IEnumerator.Current => Current!;

            public void Dispose()
            {
                _current = null!;
                _currentNode = null;
            }
        }
    }
}