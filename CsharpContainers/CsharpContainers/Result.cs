using System;

namespace Containers
{
    /// <summary>
    /// A container for the results of computation that may fail. The type specified is the successful result value.
    /// </summary>
    public class Result<T>
    {

        internal Result() { } // use the static methods to create

        /// <summary>
        /// If true, the result contains a valid ResultData and a null FailureCause.
        /// <para>If false, the ResultData should not be used and the FailureCause can be inspected.</para>
        /// </summary>
        public bool IsSuccess { get; internal set; }

        /// <summary>
        /// If false, the result contains a valid ResultData and a null FailureCause.
        /// <para>If true, the ResultData should not be used and the FailureCause can be inspected.</para>
        /// </summary>
        public bool IsFailure { get { return !IsSuccess; } }

        /// <summary>
        /// Cause of the result failure, encoded in an exception. This may hold either the original exception or a generic Exception type
        /// with a message supplied by the caller. This will be null for successful results.
        /// </summary>
        public Exception FailureCause { get; internal set; }

        /// <summary>
        /// The data returned by a successful result. This will always be invalid for failed results.
        /// </summary>
        public T ResultData { get; internal set; }

        /// <summary>
        /// Create a new success result with data and no failure reason
        /// </summary>
        public static Result<T> Success(T data)
        {
            return new Result<T>
            {
                IsSuccess = true,
                ResultData = data
            };
        }

        /// <summary>
        /// Create a new result with no data and a failure exception
        /// </summary>
        public static Result<T> Failure(Exception exception)
        {
            return new Result<T>
            {
                IsSuccess = false,
                FailureCause = exception
            };
        }

        /// <summary>
        /// Create a new failure result with a string reason. This is stored as an exception internally.
        /// </summary>
        public static Result<T> Failure(string reason)
        {
            return new Result<T>
            {
                IsSuccess = false,
                FailureCause = new Exception(reason)
            };
        }

        /// <summary>
        /// Create a new failure result with no reason.
        /// </summary>
        public static Result<T> EmptyFailure()
        {
            return new Result<T>
            {
                IsSuccess = false,
                FailureCause = Result.EmptyException
            };
        }

        /// <summary>
        /// Allow the result to be treated as a bool
        /// </summary>
        public static implicit operator bool(Result<T> res)
        {
            return res.IsSuccess;
        }

        /// <summary>
        /// Allow the result to be treated as the contained value. Will throw the original failure exception if this is not a success result.
        /// </summary>
        public static implicit operator T(Result<T> res)
        {
            if (!res) throw res.FailureCause ?? new InvalidOperationException("Tried to read the contents of an empty failure result");
            return res.ResultData;
        }

        /// <summary>
        /// Convert the container type of a failure to allow propagation
        /// </summary>
        public Result<T1> PropagateFailure<T1>()
        {
            if (IsSuccess) throw new InvalidOperationException("Tried to propagate the failure of a successful result");
            return Result<T1>.Failure(FailureCause);
        }
    }

    /// <summary>
    /// Constructor helper for Result&lt;T&gt;
    /// </summary>
    public static class Result {
        internal static readonly Exception EmptyException = new Exception();

        /// <summary>
        /// Create a new success result with data and no failure reason
        /// </summary>
        public static Result<T> Success<T>(T data)
        {
            return new Result<T>
            {
                IsSuccess = true,
                ResultData = data
            };
        }

        /// <summary>
        /// Create a new result with no data and a failure exception
        /// </summary>
        public static Result<T> Failure<T>(Exception exception)
        {
            return new Result<T>
            {
                IsSuccess = false,
                FailureCause = exception
            };
        }

        /// <summary>
        /// Create a new failure result with a string reason. This is stored as an exception internally.
        /// </summary>
        public static Result<T> Failure<T>(string reason)
        {
            return new Result<T>
            {
                IsSuccess = false,
                FailureCause = new Exception(reason)
            };
        }
    }
}