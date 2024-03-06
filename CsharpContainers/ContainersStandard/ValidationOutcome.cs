using System;

namespace Containers;

/// <summary>
/// A container for the results of a validation check. The type specified is the container of validation failure data
/// </summary>
public struct ValidationOutcome<T>
{
    /// <summary>
    /// True if validation passed. In that case ValidationErrors may be null or invalid.
    /// <para>False if validation failed, in which case ValidationErrors should be populated.</para>
    /// </summary>
    public bool IsValid { get; internal set; }

    /// <summary>
    /// False if validation passed. In that case ValidationErrors may be null or invalid.
    /// <para>True if validation failed, in which case ValidationErrors should be populated.</para>
    /// </summary>
    public bool HasError => !IsValid;

    /// <summary>
    /// Recorded errors that caused validation to fail
    /// </summary>
    public T ValidationErrors { get; internal set; }


    /// <summary>
    /// Create a validation success result
    /// </summary>
    public static ValidationOutcome<T> Pass() {
        return new ValidationOutcome<T> { IsValid = true };
    }

    /// <summary>
    /// Create a validation failure result
    /// </summary>
    public static ValidationOutcome<T> Fail(T error) {
        return new ValidationOutcome<T> {
            IsValid = false,
            ValidationErrors = error
        };
    }

    /// <summary>
    /// Allow the result to be treated as a bool
    /// </summary>
    public static implicit operator bool(ValidationOutcome<T> res)
    {
        return res.IsValid;
    }

    /// <summary>
    /// Allow the result to be treated as the contained value. Will throw an exception if there are no errors.
    /// </summary>
    public static implicit operator T(ValidationOutcome<T> res)
    {
        if (res) throw new InvalidOperationException("Tried to read errors of a successful validation");
        return res.ValidationErrors;
    }
}
    
/// <summary>
/// Constructor helper for ValidationOutcome&lt;T&gt;
/// </summary>
public static class ValidationOutcome {
    /// <summary>
    /// Create a new validation failure with associated error
    /// </summary>
    public static ValidationOutcome<T> Fail<T>(T error) {
        return ValidationOutcome<T>.Fail(error);
    }
        
    /// <summary>
    /// Create a new validation failure with associated error
    /// </summary>
    public static ValidationOutcome<T> Pass<T>() {
        return ValidationOutcome<T>.Pass();
    }
}