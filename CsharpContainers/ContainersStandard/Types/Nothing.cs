namespace Containers.Types;

/// <summary>
/// Nothing represents a type that will never be populated with a value
/// </summary>
public class Nothing
{
    private const string StringValue = "[Nothing]";

    /// <summary>
    /// An instance value to use if required
    /// </summary>
    public static Nothing Instance { get { return new Nothing(); } }

    /// <inheritdoc />
    public override bool Equals(object obj)
    {
        return obj is Nothing;
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        return 0;
    }

    /// <inheritdoc />
    public override string ToString()
    {
        return StringValue;
    }
}