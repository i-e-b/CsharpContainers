using System;

namespace Containers
{
    /// <summary>
    /// Derive from this type and implement `CompareTo(object)` to get
    /// ordering, comparing and sorting support.
    /// </summary>
    public abstract class PartiallyOrdered : IComparable {
        /// <summary>
        /// Return a value that indicating the relative order of the objects being compared.
        /// -1 or less: `this` is before <paramref name="obj" /> in the sort order.
        ///  0 `this` is in the same position in the sort order as <paramref name="obj" />.
        ///  1 or more: `this` is after <paramref name="obj" /> in the sort order.
        /// </summary>
        /// <returns>A value that indicates the relative order of the objects being compared. The return value has these meanings: Value Meaning Less than zero This instance precedes <paramref name="obj" /> in the sort order. Zero This instance occurs in the same position in the sort order as <paramref name="obj" />. Greater than zero This instance follows <paramref name="obj" /> in the sort order. </returns>
        public abstract int CompareTo(object obj);
        public abstract override int GetHashCode();

        public static int CompareTo(PartiallyOrdered x, object y) { return x.CompareTo(y); }
        public static bool operator  < (PartiallyOrdered x, PartiallyOrdered y) { return CompareTo(x, y)  < 0; }
        public static bool operator  > (PartiallyOrdered x, PartiallyOrdered y) { return CompareTo(x, y)  > 0; }
        public static bool operator <= (PartiallyOrdered x, PartiallyOrdered y) { return CompareTo(x, y) <= 0; }
        public static bool operator >= (PartiallyOrdered x, PartiallyOrdered y) { return CompareTo(x, y) >= 0; }
        public static bool operator == (PartiallyOrdered x, PartiallyOrdered y) { return CompareTo(x, y) == 0; }
        public static bool operator != (PartiallyOrdered x, PartiallyOrdered y) { return CompareTo(x, y) != 0; }
        public bool Equals(PartiallyOrdered x)    { return CompareTo(this, x) == 0; }
        public override bool Equals(object obj)
        {
            return (obj is PartiallyOrdered ordered) && (CompareTo(this, ordered) == 0);
        }
    }

}

