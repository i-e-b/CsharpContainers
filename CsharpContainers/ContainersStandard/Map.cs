using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using JetBrains.Annotations;

namespace Containers
{
    /// <summary>
    /// Map is a thread-safe dictionary that implicitly creates key-value pairs as required, 
    /// and returns default values when queried for non-present keys.
    /// <para></para>
    /// Null values should not be assigned to a Map, but they will be accepted.
    /// Null keys will NOT be accepted.
    /// </summary>
    /// <typeparam name="TKey">Key type</typeparam>
    /// <typeparam name="TValue">Value type</typeparam>
    // ReSharper disable RedundantExtendsListEntry
    public class Map<TKey, TValue>: 
            IDictionary<TKey, TValue>, ICollection<KeyValuePair<TKey, TValue>>, IEnumerable<KeyValuePair<TKey, TValue>>,
            IEnumerable, IDictionary, ICollection, IReadOnlyDictionary<TKey, TValue>, IReadOnlyCollection<KeyValuePair<TKey, TValue>>
    // ReSharper restore RedundantExtendsListEntry
    {
        [NotNull] private readonly Func<TKey, TValue> _generator;
        [NotNull] private readonly Dictionary<TKey, TValue> _storage;
        [NotNull] private readonly object dlock = new object();

        /// <inheritdoc />
        public object SyncRoot { get; }

        /// <inheritdoc />
        public bool IsSynchronized => true;

        /// <summary>
        /// Create a new empty map that returns language default values when queried for non-present keys
        /// </summary>
        public Map()
        {
            var type = typeof(TValue).GetTypeInfo();
            if (type?.IsValueType != true) throw new Exception("A generator function must be given for Map to reference type");

            SyncRoot = new object();
            _storage = new Dictionary<TKey, TValue>();
            _generator = key => default(TValue);
        }
        
        /// <summary>
        /// Create a new empty map that returns a given default value when queried for non-present keys
        /// </summary>
        public Map(TValue defaultValue)
        {
            if (defaultValue == null) throw new ArgumentNullException(nameof(defaultValue));

            SyncRoot = new object();
            _storage = new Dictionary<TKey, TValue>();
            _generator = key => defaultValue;
        }

        /// <summary>
        /// Create a new empty map that uses a generator function to create values when queried for non-present keys
        /// </summary>
        /// <param name="generator">Function to generate missing values. This should never give null values (exceptions will be thrown if it does)</param>
        public Map(Func<TKey, TValue> generator)
        {
            _generator = generator ?? throw new ArgumentNullException(nameof(generator));
            SyncRoot = new object();
            _storage = new Dictionary<TKey, TValue>();
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

        [NotNull] object IDictionary.this[object key]
        {
            [NotNull] get {
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
        [NotNull] public TValue this[TKey key]
        {
            [NotNull] get
            {
                lock (dlock)
                {
                    if (!_storage.ContainsKey(key)) {
                        var newVal = _generator(key);
                        if (newVal == null) throw new Exception("Map 'generator' function returned a null value");
                        _storage[key] = newVal;
                    }
                    // ReSharper disable once AssignNullToNotNullAttribute
                    return _storage[key]; // we are technically not correct -- if an explicit null store is made, the result will be null.
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
            [CanBeNull]get
            {
                lock (dlock)
                {
                    if (_storage.ContainsKey(key)) return _storage[key];
                }
                return default(TValue);
            }
        }

        /// <inheritdoc />
        [NotNull] IDictionaryEnumerator IDictionary.GetEnumerator()
        {
            Dictionary<TKey, TValue>.Enumerator gen;
            lock (dlock)
            {
                gen = _storage.GetEnumerator();
            }
            return gen;
        }

        [NotNull] IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator()
        {
            IEnumerator<KeyValuePair<TKey, TValue>> gen;
            lock (dlock)
            {
                gen = _storage.GetEnumerator();
            }
            return gen;
        }

        [NotNull] IEnumerator IEnumerable.GetEnumerator()
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
            // ReSharper disable once AssignNullToNotNullAttribute
            this[item.Key] = item.Value;
        }
        
        /// <summary>
        /// Add or update a value by key
        /// </summary>
        public void Add([NotNull]object key, [NotNull]object value)
        {
            ((IDictionary)this)[key] = value;
        }
        
        /// <summary>
        /// Add or update a value by key
        /// </summary>
        public void Add([NotNull]TKey key, [NotNull]TValue value)
        {
            this[key] = value;
        }

        /// <inheritdoc />
        public bool Contains(object key)
        {
            if (key == null) return false;
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
    }
}