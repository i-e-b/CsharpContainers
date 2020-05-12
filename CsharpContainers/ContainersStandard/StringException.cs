using System;

namespace Containers
{
    /// <summary>
    /// An exception that holds a message string
    /// </summary>
    public class StringException : Exception
    {
        /// <summary>
        /// Create a new string exception 
        /// </summary>
        public StringException(string reason):base(reason) { }
    }
}