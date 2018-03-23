# CsharpContainers
Some containers and base classes for general C# development

* `PartiallyOrdered` is an Abstract class that handles sorting support, equality and inequality overrides from a single comparison method.
* `Result` is a container for passing the value of computations that might fail (such as calls to IO or external services)
* `ValidationOutcome` is a container for passing the value of checks that might fail in an informative way.
* `DisposingContainer` is a disposable list, which calls `Dispose` on contained items when they are removed, replaced, or the list itself is disposed.
* `Map` is a dictionary wrapper that can generate entries when requested. This helps when working with loosely structured data

Both `Result` and `ValidationOutcome` can be treated as booleans or their contained types to keep code clean.

There are some helper types for working with these:

* `Nothing` - Represents an empty type in containers. All nothings are created equal.