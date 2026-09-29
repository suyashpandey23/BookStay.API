using BookStay.API.Constants;

namespace BookStay.API.Results;

/// <summary>
/// Represents an error with a code and description.
/// 
/// General: This is a simple data structure that holds information about an error.
/// It's used throughout the application to provide structured error information.
/// 
/// Technical: This is a 'record struct' - a C# 10 feature that combines the benefits
/// of records (immutable, value-based equality) and structs (value types, stack allocation).
/// readonly means the struct cannot be modified after creation.
/// 
/// Using a struct instead of a class can improve performance for small, short-lived objects
/// because it avoids heap allocation and garbage collection.
/// </summary>
/// <param name="Code">The error code (from ErrorCodes class)</param>
/// <param name="Description">Human-readable error message</param>
public readonly record struct Error(string Code, string Description)
{
    /// <summary>
    /// A singleton instance representing "no error".
    /// 
    /// General: Used when an operation succeeds and there's no error to report.
    /// 
    /// Technical: static readonly means this is a shared instance that cannot be modified.
    /// Using a singleton avoids creating new Error objects for the common case of "no error".
    /// </summary>
    public static readonly Error None = new("", "");

    /// <summary>
    /// Property that checks if this error represents "no error".
    /// 
    /// General: Convenient way to check if there's actually an error.
    /// 
    /// Technical: Uses expression-bodied property syntax (=>).
    /// string.IsNullOrWhiteSpace checks if the code is null, empty, or only whitespace.
    /// </summary>
    public bool IsNone => string.IsNullOrWhiteSpace(Code);
}

/// <summary>
/// A result type that indicates success or failure without returning a value.
/// 
/// General: This is used for operations that don't need to return data,
/// only whether they succeeded or failed (e.g., delete operations).
/// 
/// Technical: This is a 'record struct' for performance reasons.
/// It follows the "Result Pattern" - a functional programming pattern
/// that makes error handling explicit and avoids exceptions for control flow.
/// 
/// Instead of throwing exceptions, methods return a Result that indicates
/// success/failure. This makes error handling more predictable and explicit.
/// </summary>
public readonly record struct Result
{
    /// <summary>
    /// Indicates whether the operation was successful.
    /// 
    /// General: True if the operation succeeded, false if it failed.
    /// 
    /// Technical: get-only property (read-only). The value is set in the constructor.
    /// </summary>
    public bool IsSuccess { get; }

    /// <summary>
    /// The error that occurred (if the operation failed).
    /// 
    /// General: Contains error information when IsSuccess is false.
    /// 
    /// Technical: Will be Error.None when IsSuccess is true.
    /// </summary>
    public Error Error { get; }

    /// <summary>
    /// Private constructor - use static factory methods instead.
    /// 
    /// General: We use static factory methods (Success, Failure) to create instances
    /// because they're more readable and enforce valid states.
    /// 
    /// Technical: Making the constructor private forces users to use the static methods,
    /// which ensures that the Result is always in a valid state.
    /// </summary>
    private Result(bool isSuccess, Error error)
    {
        IsSuccess = isSuccess;
        Error = error;
    }

    /// <summary>
    /// Creates a successful Result.
    /// 
    /// General: Call this when an operation completes successfully.
    /// 
    /// Technical: Returns a new Result with IsSuccess=true and Error=Error.None.
    /// </summary>
    public static Result Success() => new(true, Error.None);

    /// <summary>
    /// Creates a failed Result with an error.
    /// 
    /// General: Call this when an operation fails.
    /// 
    /// Technical: Returns a new Result with IsSuccess=false and the specified error.
    /// </summary>
    public static Result Failure(Error error) => new(false, error);

    /// <summary>
    /// Combines multiple Results into one.
    /// 
    /// General: Useful when you need to run multiple operations and
    /// want to know if any of them failed.
    /// 
    /// Technical: params allows passing any number of Results as arguments.
    /// Uses LINQ to filter failed results and return the first error if any exist.
    /// If all succeeded, returns Success.
    /// </summary>
    public static Result Combine(params Result[] results)
    {
        // Filter to only failed results and get their errors.
        var failures = results.Where(r => !r.IsSuccess).Select(r => r.Error).ToArray();
        // If there are any failures, return the first error; otherwise, return success.
        return failures.Any() ? Failure(failures.First()) : Success();
    }
}

/// <summary>
/// A generic result type that indicates success or failure and can return a value.
/// 
/// General: This is used for operations that need to return data on success.
/// If the operation succeeds, it returns the value; if it fails, it returns errors.
/// 
/// Technical: This is a generic record struct (Result<T>).
/// The generic type T represents the type of value returned on success.
/// This pattern eliminates the need for out parameters or exception-based error handling.
/// </summary>
/// <typeparam name="T">The type of value returned on success</typeparam>
public readonly record struct Result<T>
{
    /// <summary>
    /// Indicates whether the operation was successful.
    /// </summary>
    public bool IsSuccess { get; }

    /// <summary>
    /// The value returned on success (null if failed).
    /// 
    /// General: Contains the operation result when IsSuccess is true.
    /// 
    /// Technical: T? means nullable value type (e.g., int?) or nullable reference type.
    /// Will be null (or default for value types) when IsSuccess is false.
    /// </summary>
    public T? Value { get; }

    /// <summary>
    /// Array of errors that occurred (if the operation failed).
    /// 
    /// General: Contains all error information when IsSuccess is false.
    /// 
    /// Technical: Array allows multiple errors (e.g., validation errors).
    /// Will be empty when IsSuccess is true.
    /// </summary>
    public Error[] Errors { get; }

    /// <summary>
    /// Private constructor - use static factory methods instead.
    /// </summary>
    private Result(bool isSuccess, T? value, Error[] errors)
    {
        IsSuccess = isSuccess;
        Value = value;
        Errors = errors;
    }

    /// <summary>
    /// Creates a successful Result with a value.
    /// 
    /// General: Call this when an operation completes successfully with data.
    /// 
    /// Technical: Returns a new Result with IsSuccess=true, the specified value,
    /// and an empty error array. Array.Empty<Error>() creates an empty array without allocation.
    /// </summary>
    public static Result<T> Success(T value) => new(true, value, Array.Empty<Error>());

    /// <summary>
    /// Creates a failed Result with multiple errors.
    /// 
    /// General: Call this when an operation fails with multiple errors.
    /// 
    /// Technical: Returns a new Result with IsSuccess=false, default value for T,
    /// and the specified error array.
    /// </summary>
    public static Result<T> Failure(Error[] errors) => new(false, default, errors);

    /// <summary>
    /// Creates a failed Result with a single error.
    /// 
    /// General: Call this when an operation fails with a single error.
    /// 
    /// Technical: Convenience method that wraps a single error in an array.
    /// new[] { error } creates an array with one element.
    /// </summary>
    public static Result<T> Failure(Error error) => new(false, default, new[] { error });

    /// <summary>
    /// Creates a failed Result for "not found" scenarios.
    /// 
    /// General: Call this when a requested resource doesn't exist.
    /// 
    /// Technical: Creates a failure with the NotFound error code.
    /// This is a convenience method to avoid manually creating the error.
    /// </summary>
    public static Result<T> NotFound() => new(false, default, new[] { new Error(ErrorCodes.NotFound, "Resource not found") });

    /// <summary>
    /// Creates a failed Result for bad request scenarios.
    /// 
    /// General: Call this when the client sends invalid data.
    /// 
    /// Technical: Creates a failure with the BadRequest error code and custom message.
    /// </summary>
    public static Result<T> BadRequest(string message) => new(false, default, new[] { new Error(ErrorCodes.BadRequest, message) });
    
    /// <summary>
    /// Chains another operation that returns a Result.
    /// 
    /// General: Allows chaining multiple operations where each depends on the previous.
    /// If this Result failed, the next operation is skipped.
    /// 
    /// Technical: This is a monadic "bind" operation from functional programming.
    /// Func<T, Result<K>> is a function that takes a T and returns a Result<K>.
    /// Only calls the function if this Result succeeded; otherwise, propagates the failure.
    /// </summary>
    public Result<K> Map<K>(Func<T, Result<K>> next)
        => IsSuccess ? next(Value) : Result<K>.Failure(Errors);

    /// <summary>
    /// Validates the value against a predicate, failing if it doesn't match.
    /// 
    /// General: Adds a validation check that can fail the Result.
    /// 
    /// Technical: Func<T, bool> is a predicate function that takes T and returns bool.
    /// Only checks the predicate if this Result succeeded.
    /// If the predicate returns false, returns a failure with the specified error.
    /// </summary>
    public Result<T> Ensure(Func<T, bool> predicate, Error error)
        => IsSuccess ? (predicate(Value) ? this : Failure(error)) : this;
}