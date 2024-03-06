using System;

#pragma warning disable 1591
// ReSharper disable UnusedMember.Global
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable IntroduceOptionalParameters.Global
// ReSharper disable MemberCanBeProtected.Global
// ReSharper disable InconsistentNaming
// ReSharper disable once CheckNamespace
namespace JetBrains.Annotations;

/// <summary>
/// Indicates that the value of the marked element could be <c>null</c>
/// </summary>
[AttributeUsage(AttributeTargets.All)]
internal sealed class CanBeNullAttribute : Attribute { }

/// <summary>
/// Indicates that the value of the marked element could never be <c>null</c>.
/// </summary>
[AttributeUsage(AttributeTargets.All)]
internal sealed class NotNullAttribute : Attribute { }

/// <summary>
/// Can be applied to symbols of types derived from IEnumerable as well as to symbols of Task
/// and Lazy classes to indicate that the value of a collection item, of the Task.Result property
/// or of the Lazy.Value property can never be null.
/// </summary>
[AttributeUsage(AttributeTargets.All)]
internal sealed class ItemNotNullAttribute : Attribute { }

/// <summary>
/// The return value holds unmanaged state and must be disposed at end of scope.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Constructor | AttributeTargets.Method | AttributeTargets.Parameter)]
public sealed class MustDisposeResourceAttribute : Attribute
{
    public MustDisposeResourceAttribute() { Value = true; }
    public MustDisposeResourceAttribute(bool value) { Value = value; }
    /// <summary>
    /// When set to <c>false</c>, disposing of the resource is not obligatory.
    /// The main use-case for explicit <c>[MustDisposeResource(false)]</c> annotation is to loosen inherited annotation.
    /// </summary>
    public bool Value { get; }
}

[AttributeUsage(AttributeTargets.Parameter)]
public sealed class MaybeNullWhenAttribute : Attribute
{
    public MaybeNullWhenAttribute() { Value = true; }
    public MaybeNullWhenAttribute(bool value) { Value = value; }
    /// <summary>
    /// When set to <c>false</c>, disposing of the resource is not obligatory.
    /// The main use-case for explicit <c>[MustDisposeResource(false)]</c> annotation is to loosen inherited annotation.
    /// </summary>
    public bool Value { get; }
}