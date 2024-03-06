using System;

namespace Containers;

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
    /// <para></para>
    /// For reference types, `null` is considered equal to any other `null`, and less than any non-null value.
    /// </summary>
    /// <returns>A value that indicates the relative order of the objects being compared.
    /// The return value has these meanings: 
    /// Less than zero: This instance precedes <paramref name="obj" /> in the sort order.
    /// Zero: This instance occurs in the same position in the sort order as <paramref name="obj" />.
    /// Greater than zero: This instance follows <paramref name="obj" /> in the sort order. </returns>
    public abstract int CompareTo(object? obj);

    /// <inheritdoc />
    public abstract override int GetHashCode();

    /// <summary>
    /// Return a value that indicating the relative order of the objects being compared.
    /// <dl>
    /// <dt>-1 or less</dt><dd><paramref name="x" /> is before <paramref name="y" /> in the sort order</dd>
    /// <dt>0</dt><dd><paramref name="x" /> is in the same position in the sort order as <paramref name="y" /></dd>
    /// <dt>1 or more</dt><dd><paramref name="x" /> is after <paramref name="y" /> in the sort order</dd>
    /// </dl>
    /// <para></para>
    /// For reference types, <c>null</c> is considered equal to any other <c>null</c>, and less than any non-null value.
    /// </summary>
    public static int CompareTo(PartiallyOrdered x, object? y) { if (ReferenceEquals(x, null!)) { return ReferenceEquals(y!, null!) ? 0 : -1; } return x.CompareTo(y); }

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public static bool operator  < (PartiallyOrdered x, PartiallyOrdered y) { return CompareTo(x, y)  < 0; }
    public static bool operator  > (PartiallyOrdered x, PartiallyOrdered y) { return CompareTo(x, y)  > 0; }
    public static bool operator <= (PartiallyOrdered x, PartiallyOrdered y) { return CompareTo(x, y) <= 0; }
    public static bool operator >= (PartiallyOrdered x, PartiallyOrdered y) { return CompareTo(x, y) >= 0; }
    public static bool operator == (PartiallyOrdered x, PartiallyOrdered y) { return CompareTo(x, y) == 0; }
    public static bool operator != (PartiallyOrdered x, PartiallyOrdered y) { return CompareTo(x, y) != 0; }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
    
    /// <summary>
    /// Returns true if this instance is considered in equal-order to <paramref name="x" />.
    /// </summary>
    public bool Equals(PartiallyOrdered x)    { return CompareTo(this, x) == 0; }

    /// <inheritdoc />
    public override bool Equals(object? obj)
    {
        return (obj is PartiallyOrdered ordered) && (CompareTo(this, ordered) == 0);
    }
}