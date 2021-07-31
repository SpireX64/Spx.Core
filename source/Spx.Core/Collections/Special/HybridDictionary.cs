using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;

namespace Spx.Collections.Special
{
    public class HybridDictionary<TKey, TValue>: IDictionary<TKey, TValue>
    {
        private HybridDictionary _internalDictionary;
        
        public HybridDictionary()
        {
            _internalDictionary = new HybridDictionary();
        }

        public HybridDictionary(int initialSize)
        {
            _internalDictionary = new HybridDictionary(initialSize);
        }

        public HybridDictionary(IDictionary<TKey, TValue> dict)
        {
            _internalDictionary = new HybridDictionary(dict.Count);
            foreach (var keyValuePair in dict)
            {
                _internalDictionary.Add(keyValuePair.Key!, keyValuePair.Value);
            }
        }
        
        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() => 
            new HybridDictionaryEnumerator<TKey, TValue>(this);

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(KeyValuePair<TKey, TValue> item) => 
            _internalDictionary.Add(item.Key!, item.Value);

        public void Clear() => 
            _internalDictionary.Clear();

        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            if (!_internalDictionary.Contains(item.Key!)) return false;
            var value = _internalDictionary[item.Key!];
            return value.Equals(item.Value);
        }

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            for (var i = arrayIndex; i < array.Length; i++)
            {
                var item = array[i];
                _internalDictionary.Add(item.Key!, item.Value);
            }
        }

        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            var key = item.Key;
            if (!_internalDictionary.Contains(key!)) return false;
            
            var value = _internalDictionary[key!];
            if (!value.Equals(item.Value)) return false;
            
            _internalDictionary.Remove(key!);
            return true;

        }

        public int Count => _internalDictionary.Count;
        public bool IsReadOnly => _internalDictionary.IsReadOnly;

        public void Add(TKey key, TValue value) =>
            _internalDictionary.Add(key!, value);

        public bool ContainsKey(TKey key) => _internalDictionary.Contains(key!);

        public bool Remove(TKey key)
        {
            if (!_internalDictionary.Contains(key!)) 
                return false;
            
            _internalDictionary.Remove(key!);
            return true;
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            if (_internalDictionary.Contains(key!))
            {
                value = (TValue) _internalDictionary[key!];
                return true;
            }

            value = default!;
            return false;
        }

        public TValue this[TKey key]
        {
            get => (TValue) _internalDictionary[key!];
            set => _internalDictionary[key!] = value;
        }

        public ICollection<TKey> Keys => _internalDictionary.Keys.OfType<TKey>().ToList();
        public ICollection<TValue> Values => _internalDictionary.Values.OfType<TValue>().ToList();

    }

    internal class HybridDictionaryEnumerator<TKey, TValue>: IEnumerator<KeyValuePair<TKey, TValue>>
    {
        private HybridDictionary<TKey, TValue>? _hybridDictionary;
        private List<TKey>? _keys;
        private int _index = -1;

        public HybridDictionaryEnumerator(HybridDictionary<TKey, TValue> hybridDictionary)
        {
            _hybridDictionary = hybridDictionary;
            _keys = hybridDictionary.Keys.ToList();
        }
        
        public bool MoveNext() => ++_index < (_keys?.Count ?? 0);

        public void Reset()
        {
            _index = -1;
        }

        public KeyValuePair<TKey, TValue> Current
        {
            get
            {
                var key = _keys![_index];
                var value = _hybridDictionary![key];
                return new KeyValuePair<TKey, TValue>(key, value);
            }
        }

        object IEnumerator.Current => Current;

        public void Dispose()
        {
            _keys = null;
            _hybridDictionary = null;
        }
    }
}