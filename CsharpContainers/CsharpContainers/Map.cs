using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Containers
{
    /// <summary>
    /// Map is a thread-safe dictionary that implicitly creates key-value pairs as required, 
    /// and returns default values when queried for non-present keys.
    /// </summary>
    /// <typeparam name="TKey">Key type</typeparam>
    /// <typeparam name="TValue">Value type</typeparam>
    // ReSharper disable RedundantExtendsListEntry
    public class Map<TKey, TValue>: 
            IDictionary<TKey, TValue>, ICollection<KeyValuePair<TKey, TValue>>, IEnumerable<KeyValuePair<TKey, TValue>>,
            IEnumerable, IDictionary, ICollection, IReadOnlyDictionary<TKey, TValue>, IReadOnlyCollection<KeyValuePair<TKey, TValue>>, ISerializable
    // ReSharper restore RedundantExtendsListEntry
    {
        private readonly Func<TKey, TValue> _generator;
        private readonly Dictionary<TKey, TValue> _storage;
        private readonly object dlock = new object();

        /// <inheritdoc />
        public object SyncRoot { get; }

        /// <inheritdoc />
        public bool IsSynchronized => true;

        /// <summary>
        /// Create a new empty map that returns language default values when queried for non-present keys
        /// </summary>
        public Map()
        {
            SyncRoot = new object();
            _storage = new Dictionary<TKey, TValue>();
            _generator = key => default(TValue);
        }

        /// <summary>
        /// Create a new empty map that uses a generator function to create values when queried for non-present keys
        /// </summary>
        /// <param name="generator">Function to generate missing values</param>
        public Map(Func<TKey, TValue> generator)
        {
            SyncRoot = new object();
            _storage = new Dictionary<TKey, TValue>();
            _generator = generator;
        }

        /// <inheritdoc />
        void IDictionary.Clear()
        {
            lock(dlock) { _storage.Clear(); }
        }

        void ICollection<KeyValuePair<TKey, TValue>>.Clear()
        {
            lock(dlock) { _storage.Clear(); }
        }

        /// <inheritdoc />
        public void Remove(object key)
        {
            if (key is TKey key1) {
                Remove(key1);
            }
        }

        /// <inheritdoc />
        public bool Remove(TKey key)
        {
            lock (dlock){
                return _storage.Remove(key);
            }
        }

        /// <inheritdoc />
        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            lock (dlock){
                return _storage.Remove(item.Key);
            }
        }

        object IDictionary.this[object key]
        {
            get {
                if (key is TKey key1) {
                    return this[key1];
                }
                throw new ArrayTypeMismatchException();
            }
            set {
                if (key is TKey key1 && value is TValue value1) {
                    this[key1] = value1;
                }
                throw new ArrayTypeMismatchException();
            }
        }
        

        /// <summary>
        /// Get or set a value by key
        /// </summary>
        public TValue this[TKey key]
        {
            get
            {
                lock (dlock)
                {
                    if (!_storage.ContainsKey(key)) _storage[key] = _generator(key);
                    return _storage[key];
                }
            }
            set
            {
                lock (dlock){
                    if (_storage.ContainsKey(key)) _storage[key] = value;
                    else _storage.Add(key, value);
                }
            }
        }


        TValue IReadOnlyDictionary<TKey, TValue>.this[TKey key]
        {
            get
            {
                lock (dlock)
                {
                    if (_storage.ContainsKey(key)) return _storage[key];
                }
                return default(TValue);
            }
        }

        /// <inheritdoc />
        IDictionaryEnumerator IDictionary.GetEnumerator()
        {
            Dictionary<TKey, TValue>.Enumerator gen;
            lock (dlock)
            {
                gen = _storage.GetEnumerator();
            }
            return gen;
        }

        IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator()
        {
            IEnumerator<KeyValuePair<TKey, TValue>> gen;
            lock (dlock)
            {
                gen = _storage.GetEnumerator();
            }
            return gen;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            IEnumerator<KeyValuePair<TKey, TValue>> gen;
            lock (dlock)
            {
                gen = _storage.GetEnumerator();
            }
            return gen;
        }

        /// <summary>
        /// Add or update a value by key
        /// </summary>
        public void Add(KeyValuePair<TKey, TValue> item)
        {
            this[item.Key] = item.Value;
        }
        
        /// <summary>
        /// Add or update a value by key
        /// </summary>
        public void Add(object key, object value)
        {
            ((IDictionary)this)[key] = value;
        }
        
        /// <summary>
        /// Add or update a value by key
        /// </summary>
        public void Add(TKey key, TValue value)
        {
            this[key] = value;
        }

        /// <inheritdoc />
        public bool Contains(object key)
        {
            if (key is TKey key1) {
                lock(dlock){
                    return _storage.ContainsKey(key1);
                }
            }
            return false;
        }

        /// <inheritdoc />
        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            lock(dlock){
                if ( ! _storage.ContainsKey(item.Key)) return false;
                return this[item.Key].Equals(item.Value);
            }
        }

        /// <inheritdoc />
        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            lock(dlock){
                ((ICollection<KeyValuePair<TKey, TValue>>)_storage).CopyTo(array, arrayIndex);
            }
        }

        /// <inheritdoc />
        public void CopyTo(Array array, int index)
        {
            lock(dlock){
                ((ICollection)_storage).CopyTo(array, index);
            }
        }

        int ICollection.Count
        {
            get { lock (dlock) { return _storage.Count; } }
        }

        int ICollection<KeyValuePair<TKey, TValue>>.Count
        {
            get { lock (dlock) { return _storage.Count; } }
        }

        int IReadOnlyCollection<KeyValuePair<TKey, TValue>>.Count
        {
            get { lock (dlock) { return _storage.Count; } }
        }

        IEnumerable<TKey> IReadOnlyDictionary<TKey, TValue>.Keys
        {
            get { lock (dlock) { return _storage.Keys; } }
        }

        /// <summary>
        /// All keys in the collection
        /// </summary>
        public ICollection<TKey> Keys
        {
            get { lock (dlock) { return _storage.Keys; } }
        }

        ICollection IDictionary.Keys
        {
            get { lock (dlock) { return _storage.Keys; } }
        }

        IEnumerable<TValue> IReadOnlyDictionary<TKey, TValue>.Values
        {
            get { lock (dlock) { return _storage.Values; } }
        }

        ICollection IDictionary.Values
        {
            get { lock (dlock) { return _storage.Values; } }
        }

        /// <summary>
        /// All values in the collection
        /// </summary>
        public ICollection<TValue> Values
        {
            get { lock (dlock) { return _storage.Values; } }
        }

        bool IDictionary.IsReadOnly
        {
            get { return false; }
        }

        /// <inheritdoc />
        public bool IsFixedSize { get{return false; } }

        bool ICollection<KeyValuePair<TKey, TValue>>.IsReadOnly
        {
            get { return false; }
        }

        /// <inheritdoc />
        public bool ContainsKey(TKey key)
        {
            lock(dlock){
                return _storage.ContainsKey(key);
            }
        }

        bool IReadOnlyDictionary<TKey, TValue>.ContainsKey(TKey key)
        {
            return ContainsKey(key);
        }

        bool IReadOnlyDictionary<TKey, TValue>.TryGetValue(TKey key, out TValue value)
        {
            return TryGetValue(key, out value);
        }

        /// <inheritdoc />
        public bool TryGetValue(TKey key, out TValue value)
        {
            lock(dlock){
                return _storage.TryGetValue(key, out value);
            }
        }

        /// <inheritdoc />
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            lock(dlock){
                _storage.GetObjectData(info, context);
            }
        }
    }
}