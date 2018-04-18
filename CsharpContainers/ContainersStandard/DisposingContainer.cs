using System;
using System.Collections;
using System.Collections.Generic;

namespace Containers
{
    /// <summary>
    /// A wrapper around List, which calls `Dispose` on contained items when they are removed, replaced, or the list itself is disposed.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DisposingContainer<T> : IDisposable, ICollection<T>
        where T : IDisposable{

        private readonly List<T> _innerContainer;
        private readonly object _lock;
        private volatile bool _isDisposed;

        /// <summary>
        /// Create a new disposing container
        /// </summary>
        public DisposingContainer()
        {
            _isDisposed = false;
            _lock = new object();
            _innerContainer = new List<T>();
        }

        /// <summary>
        /// Dispose of the container, and call dispose on all contents
        /// </summary>
        public void Dispose()
        {
            lock (_lock)
            {
                if (_isDisposed) return;
                _isDisposed = true;

                DisposeAll();
            }
        }

        private void DisposeAll()
        {
            foreach (var item in _innerContainer)
            {
                try { if (item != null) item.Dispose(); }
                catch { Ignore(); }
            }
        }

        private static void Ignore() { } 

        /// <summary>
        /// Get an iterator for the contained objects
        /// </summary>
        public IEnumerator<T> GetEnumerator() => _innerContainer.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        /// <inheritdoc />
        public void Add(T item)
        {
            if (item == null) return;
            lock(_lock){_innerContainer.Add(item); }
        }

        /// <summary>
        /// Dispose all contained items then clear
        /// </summary>
        public void Clear()
        {
            lock (_lock)
            {
                if (_isDisposed) return;
                _isDisposed = true;
                DisposeAll();
                _innerContainer.Clear();

                _isDisposed = false;
            }
        }

        /// <inheritdoc />
        public bool Contains(T item) => _innerContainer.Contains(item);

        /// <inheritdoc />
        public void CopyTo(T[] array, int arrayIndex) => _innerContainer.CopyTo(array, arrayIndex);

        /// <inheritdoc />
        public bool Remove(T item)
        {
            if (item == null) return false;
            item.Dispose();
            return _innerContainer.Remove(item);
        }

        /// <inheritdoc />
        public int Count { get { return _innerContainer.Count; } }

        /// <inheritdoc />
        public bool IsReadOnly { get { return false; } }
    }
}