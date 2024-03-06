using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace Containers;


/// <summary>
/// A thread-safe dictionary that has a maximum age for elements.
/// This is for caching.
/// </summary>
public class AgeDictionary<TK,TV> where TK : notnull
{
    // Storage
    private readonly object _lock = new();
    private readonly Dictionary<TK, AgeWrapper> _store = new();
    
    // Age
    private readonly TimeSpan _maxAge;
    
    // Capacity
    private readonly int _maxCount;
    private volatile bool _trimInProgress;
    private const int Hysteresis = 128; // we have some hysteresis to prevent overload near capacity

    /// <summary>
    /// Create a dictionary, with a maximum age for elements and a maximum count of elements
    /// </summary>
    /// <param name="maxAge">Maximum age for entries. Must be more than one second</param>
    /// <param name="maxCount">Maximum count of entries. Must be more than 256</param>
    public AgeDictionary(TimeSpan maxAge, int maxCount)
    {
        if (maxAge < TimeSpan.FromSeconds(1)) throw new ArgumentOutOfRangeException(nameof(maxAge));
        if (maxCount < 256) throw new ArgumentOutOfRangeException(nameof(maxCount));
        
        _trimInProgress = false;
        _maxAge = maxAge;
        _maxCount = maxCount;
    }

    /// <summary>
    /// Try to read value based on key.
    /// Returns <c>false</c> if the key is not present, or the value is older than configured max age.
    /// Returns <c>true</c> and sets <c>value</c> if the key is present and less than max age.
    /// </summary>
    public bool TryGetValue(TK key, [MaybeNullWhen(false)] out TV value)
    {
        bool found;
        value = default!;
        AgeWrapper? wrapper;
        lock (_lock)
        {
            found = _store.TryGetValue(key, out wrapper);
        }
        if (!found || wrapper is null) return false;
        if (wrapper.Age > _maxAge) return false;

        value = wrapper.Value;
        return true;
    }

    /// <summary>
    /// Either add or replace the given key with a new value.
    /// This resets the value's age.
    /// </summary>
    public void TryAdd(TK key, TV value)
    {
        var wrapper = new AgeWrapper(value);
        lock (_lock)
        {
            _store[key] = wrapper;
            if (_store.Count <= _maxCount + Hysteresis) return;
        }
        
        // Store has become too big:
        TrimStorage();
    }

    private void TrimStorage()
    {
        if (_trimInProgress) return;
        try
        {
            KeyValuePair<TK, AgeWrapper>[] copy;
            lock (_lock)
            {
                if (_trimInProgress) return;
                _trimInProgress = true;
                copy = _store.AsEnumerable().ToArray();
            }

            var limitDate = DateTime.UtcNow - _maxAge;
            
            // First, try removing old items
            foreach (var item in copy)
            {
                if (item.Value is not null && item.Value.Date < limitDate) continue;
                lock (_lock) { _store.Remove(item.Key); }
            }

            // If we removed enough, then stop
            lock (_lock) { if (_store.Count <= _maxCount) return; }
            
            // Otherwise, sort remaining by age and trim again
            var hys = Math.Max(128, _maxCount - Hysteresis); // try to reduce below true maximum, to prevent overload at capacity
            lock (_lock)
            {
                copy = _store.AsEnumerable().ToArray();
            }
            Array.Sort(copy, (a, b) => a.Value!.Date.CompareTo(b.Value!.Date));
            lock (_lock)
            {
                for (int i = hys; i < copy.Length; i++)
                {
                    _store.Remove(copy[i].Key);
                }
            }
        }
        finally
        {
            _trimInProgress = false;
        }
    }

    /// <summary>
    /// Wrapper for value + age
    /// </summary>
    private class AgeWrapper
    {
        public AgeWrapper(TV value)
        {
            Value = value;
            Date = DateTime.UtcNow;
        }
        
        internal readonly DateTime Date;
        
        /// <summary>
        /// Stored value
        /// </summary>
        public TV Value { get; }
        
        /// <summary>
        /// Age of value
        /// </summary>
        public TimeSpan Age => DateTime.UtcNow - Date;
    }
}