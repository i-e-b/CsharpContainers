using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Containers;

/// <summary>
/// A multi-thread safe dictionary of TK => List(TV)
/// </summary>
/// <typeparam name="TK">Key type</typeparam>
/// <typeparam name="TV">Value type</typeparam>
public class MultiMap<TK,TV> : IEnumerable<KeyValuePair<TK,List<TV>>> where TK : notnull
{
    /// <summary>
    /// Number of values held across all keys
    /// </summary>
    public int ValueCount => GetValueCount();

    /// <summary>
    /// Number of unique keys
    /// </summary>
    public int KeyCount
    {
        get {
            lock (_lock)
            {
                return _container.Count;
            }
        }
    }

    private readonly object _lock = new();
    private readonly Dictionary<TK, List<TV>> _container = new();

    /// <summary>
    /// Add a new value with the given key.
    /// Existing items are not removed or replaced.
    /// </summary>
    public void Add(TK key, TV item)
    {
        lock (_lock)
        {
            if (!_container.ContainsKey(key)) _container.Add(key, new List<TV>());
            _container[key]!.Add(item);
        }
    }

    /// <summary>
    /// Remove a value, only if found against the given key.
    /// This can be called while iterating over the output of <see cref="ListFor"/>.
    /// <p/>
    /// If no matching item found, returns <c>false</c>
    /// </summary>
    public bool Remove(TK key, TV item)
    {
        lock (_lock)
        {
            if (!_container.TryGetValue(key, out var list)) return false;
            return list!.Remove(item);
        }
    }

    /// <summary>
    /// Return an enumerable of all values listed against a single key.
    /// If key is not present, an empty enumerable is returned.
    /// <p/>
    /// This gives a snap-shot, and <see cref="Remove"/> and <see cref="Add"/>
    /// can be called while iterating over the result.
    /// </summary>
    public IEnumerable<TV> ListFor(TK key)
    {
        lock (_lock)
        {
            if (!_container.TryGetValue(key, out var list)) return new TV[0];
            return list?.ToArray() ?? new TV[0];
        }
    }

    /// <summary>
    /// Return an enumerable of all keys in the map.
    /// <p/>
    /// This gives a snap-shot, and <see cref="Remove"/> and <see cref="Add"/>
    /// can be called while iterating over the result.
    /// </summary>
    public IEnumerable<TK> AllKeys()
    {
        lock (_lock)
        {
            return _container.Keys?.ToArray() ?? new TK[0];
        }
    }

    private int GetValueCount()
    {
        lock (_lock)
        {
            return _container.Values?.Sum(list => list.Count) ?? 0;
        }
    }

    /// <inheritdoc />
    public IEnumerator<KeyValuePair<TK, List<TV>>> GetEnumerator()
    {
        var result = new List<KeyValuePair<TK, List<TV>>>();
        lock (_lock)
        {
            result.AddRange(_container);
        }
        return result.GetEnumerator();
    }

    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}