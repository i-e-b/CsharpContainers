using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Containers.Types;
using JetBrains.Annotations;

namespace Containers
{
    /// <summary>
    /// A container for results of computation that may partially fail and be left
    /// in a recoverable state
    /// </summary>
    /// <typeparam name="T">Success data type</typeparam>
    public struct PartialResult<T>
    {
        [CanBeNull] private List<Exception> _causes;

        /// <summary>
        /// The state of the computation
        /// </summary>
        public PartialResultState State { get; set; }

        /// <summary>
        /// Data result. May be invalid depending on the State
        /// </summary>
        [CanBeNull]
        public T ResultData { get; set; }

        /// <summary>
        /// If true, the result contains a valid ResultData and represents a complete computation
        /// <para>If false, the ResultData should not be used and the FailureCauses can be inspected.</para>
        /// </summary>
        public bool IsSuccess => State == PartialResultState.Success;

        /// <summary>
        /// If true, the ResultData may be invalid and the FailureCauses can be inspected.
        /// <para>Any partial completion is considered a failed state by this property</para>
        /// </summary>
        public bool IsFailure => !IsSuccess;

        /// <summary>
        /// Read-only list of warnings and errors added to the result
        /// </summary>
        [NotNull]
        public IReadOnlyCollection<Exception> Causes => new ReadOnlyCollection<Exception>(_causes ?? new List<Exception>());

        /// <summary>
        /// Enumerates through the message properties of causes, if there are any.
        /// Returns empty if no causes are stored. Never returns null.
        /// </summary>
        [NotNull]
        public IEnumerable<string> CauseMessages
        {
            get
            {
                if (_causes == null) yield break;
                foreach (var cause in _causes)
                {
                    if (string.IsNullOrWhiteSpace(cause?.Message)) continue;
                    yield return cause.Message;
                }
            }
        }


        /// <summary>
        /// Sets the result state to success, and adds result data
        /// </summary>
        public static PartialResult<T> Success(T data)
        {
            return new PartialResult<T>
            {
                State = PartialResultState.Warning,
                ResultData = data
            };
        }

        /// <summary>
        /// Sets the result state to warning, and adds message to the failure causes
        /// </summary>
        public static PartialResult<T> Warning(string message)
        {
            return new PartialResult<T>
            {
                State = PartialResultState.Warning,
                _causes = new List<Exception> {new StringException(message)}
            };
        }

        /// <summary>
        /// Sets the result state to warning, and adds exception to the failure causes
        /// </summary> 
        public static PartialResult<T> Warning(Exception cause)
        {
            return new PartialResult<T>
            {
                State = PartialResultState.Warning,
                _causes = new List<Exception> {cause}
            };
        }


        /// <summary>
        /// Sets the result state to Failed, and adds message to the failure causes
        /// </summary>
        public static PartialResult<T> Failure(string message)
        {
            return new PartialResult<T>
            {
                State = PartialResultState.Failed,
                _causes = new List<Exception> {new StringException(message)}
            };
        }

        /// <summary>
        /// Sets the result state to Failed, and adds exception to the failure causes
        /// </summary> 
        public static PartialResult<T> Failure(Exception cause)
        {
            return new PartialResult<T>
            {
                State = PartialResultState.Failed,
                _causes = new List<Exception> {cause}
            };
        }


        /// <summary>
        /// Add to the failure cause list. Does not change the result state
        /// </summary>
        public void AddFailureCause(Exception cause)
        {
            if (_causes == null) _causes = new List<Exception>();
            _causes.Add(cause);
        }

        /// <summary>
        /// Add data to this result. Does not change the result state
        /// </summary>
        public PartialResult<T> WithData(T data)
        {
            ResultData = data;
            return this;
        }

        /// <summary>
        /// Update the state of the result. This will only progress up the 'failure chain'.
        /// <para>(e.g. a warning will replace a success, but not a failure)</para>
        /// </summary>
        public void AddState(PartialResultState newState)
        {
            if ((int) newState > (int) State) State = newState;
        }

        /// <summary>
        /// Allow the result to be treated as a bool
        /// </summary>
        public static implicit operator bool(PartialResult<T> res)
        {
            return res.IsSuccess;
        }

        /// <summary>
        /// Allow the result to be treated as the contained value. Will throw the original failure exception
        /// if this is not a success result. Throws an exception if the result data is null
        /// </summary>
        [NotNull]
        public static implicit operator T(PartialResult<T> res)
        {
            if (res.ResultData == null) throw new InvalidOperationException("Tried to get the data from an empty result");
            return res.ResultData;
        }

        /// <summary>
        /// Convert the container type of a failure to allow propagation
        /// </summary>
        public PartialResult<T1> PropagateFailure<T1>()
        {
            if (IsSuccess) throw new InvalidOperationException("Tried to propagate the failure of a successful result");
            return new PartialResult<T1>
            {
                _causes = _causes,
                State = State,
            };
        }
    }

    /// <summary>
    /// Required for interface consistency
    /// </summary>
    public static class PartialResultExtensions
    {
        /// <summary>
        /// Update the status if appropriate and set result data
        /// </summary>
        public static PartialResult<T> Success<T>(this PartialResult<T> src, T data)
        {
            src.AddState(PartialResultState.Success);
            src.ResultData = data;
            return src;
        }

        /// <summary>
        /// Update the status if appropriate, and adds message to the failure causes
        /// </summary>
        public static PartialResult<T> Warning<T>(this PartialResult<T> src, string message)
        {
            src.AddState(PartialResultState.Warning);
            src.AddFailureCause(new StringException(message));
            return src;
        }

        /// <summary>
        /// Update the status if appropriate, and adds exception to the failure causes
        /// </summary> 
        public static PartialResult<T> Warning<T>(this PartialResult<T> src, Exception cause)
        {
            src.AddState(PartialResultState.Warning);
            src.AddFailureCause(cause);
            return src;
        }

        /// <summary>
        /// Update the status if appropriate, and adds message to the failure causes
        /// </summary>
        public static PartialResult<T> Failure<T>(this PartialResult<T> src, string message)
        {
            src.AddState(PartialResultState.Failed);
            src.AddFailureCause(new StringException(message));
            return src;
        }

        /// <summary>
        /// Update the status if appropriate, and adds exception to the failure causes
        /// </summary> 
        public static PartialResult<T> Failure<T>(this PartialResult<T> src, Exception cause)
        {
            src.AddState(PartialResultState.Failed);
            src.AddFailureCause(cause);
            return src;
        }
    }

    /// <summary>
    /// Constructor helper for PartialResult&lt;T&gt;
    /// </summary>
    public static class PartialResult
    {
        /// <summary>
        /// Create a new success result with data and no failure reason
        /// </summary>
        public static PartialResult<T> Success<T>(T data)
        {
            return new PartialResult<T>
            {
                State = PartialResultState.Success,
                ResultData = data
            };
        }

        /// <summary>
        /// Create a partial result with data. This does not set a valid result type.
        /// </summary>
        public static PartialResult<T> WithData<T>(T data)
        {
            return new PartialResult<T>
            {
                State = PartialResultState.Invalid,
                ResultData = data
            };
        }

        /// <summary>
        /// Create a new success result with no data and no failure reason
        /// </summary>
        public static PartialResult<Nothing> Success()
        {
            return new PartialResult<Nothing>
            {
                State = PartialResultState.Success,
                ResultData = Nothing.Instance
            };
        }

        /// <summary>
        /// Create a new warning result with no data and a failure reason
        /// </summary>
        public static PartialResult<Nothing> Warning(Exception cause)
        {
            var result = new PartialResult<Nothing>
            {
                State = PartialResultState.Warning,
                ResultData = Nothing.Instance,
            };
            result.AddFailureCause(cause);
            return result;
        }

        /// <summary>
        /// Create a new warning result with no data and a failure reason
        /// </summary>
        public static PartialResult<Nothing> Warning(string message)
        {
            var result = new PartialResult<Nothing>
            {
                State = PartialResultState.Warning,
                ResultData = Nothing.Instance,
            };
            result.AddFailureCause(new StringException(message));
            return result;
        }

        /// <summary>
        /// Create a new failure result with no data and a failure reason
        /// </summary>
        public static PartialResult<Nothing> Failure(Exception cause)
        {
            var result = new PartialResult<Nothing>
            {
                State = PartialResultState.Failed,
                ResultData = Nothing.Instance,
            };
            result.AddFailureCause(cause);
            return result;
        }

        /// <summary>
        /// Create a new failure result with no data and a failure reason
        /// </summary>
        public static PartialResult<Nothing> Failure(string message)
        {
            var result = new PartialResult<Nothing>
            {
                State = PartialResultState.Failed,
                ResultData = Nothing.Instance,
            };
            result.AddFailureCause(new StringException(message));
            return result;
        }
    }

    /// <summary>
    /// Result states for PartialResult&lt;T&gt; 
    /// </summary>
    public enum PartialResultState
    {
        /// <summary>
        /// The result state has not been set correctly
        /// </summary>
        Invalid = 0,

        /// <summary>
        /// The computation was successful, and completed
        /// </summary>
        Success = 1,

        /// <summary>
        /// The computation may have completed, but the data may not be entirely reliable.
        /// <para>In this state, both the ResultData and FailureCauses may be populated</para>
        /// </summary>
        Warning = 2,

        /// <summary>
        /// The computation failed, but changes were rolled back.
        /// The system data should be the same as before the computation started.
        /// </summary>
        Failed = 3
    }
}