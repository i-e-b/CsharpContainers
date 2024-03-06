# CsharpContainers

Some containers and base classes for general C# development

## 
* `Result` is a container for passing the value of computations that might fail (such as calls to IO or external services).
* `PartialResult` is a container for passing the value of computations that might fail for multiple reasons or return partially complete results.
* `ValidationOutcome` is a container for passing the value of checks that might fail in an informative way.
* `DisposingContainer` is a disposable list, which calls `Dispose` on contained items when they are removed, replaced, or the list itself is disposed.

Both `Result` and `ValidationOutcome` can be treated as booleans or their contained types to keep code clean.

## Dictionaries
* 
* `Map` is a dictionary wrapper that can generate entries when requested. This helps when working with loosely structured data.
* `MultiMap` is a multi-thread safe dictionary of key => List(value).
* `AgeDictionary` is a thread-safe dictionary that has a maximum age for elements, mainly for caching.

## Queues and Buffers

* `Dequeue` is a generic, auto-sizing, double-ended queue, and attempts to be compatible with JavaScript array semantics.
* `BiPartiteQueue` is a fixed-size circular buffer that gives byte-array access, and keeps each item in a contiguous range.

## General Types

* `PartiallyOrdered` is an Abstract class that handles sorting support, equality and inequality overrides from a single comparison method.
* `Nothing` - Represents an empty type in containers. All nothings are created equal.
