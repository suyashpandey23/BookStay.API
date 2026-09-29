namespace BookStay.API.Constants;

/// <summary>
/// Contains constant error codes used throughout the application.
/// 
/// General: This class defines standardized error codes that are used to categorize
/// different types of errors. Using constants instead of magic strings makes the code
/// more maintainable and prevents typos.
/// 
/// Technical: This is a static class, which means it cannot be instantiated and only
/// contains static members. The const fields are compile-time constants that are
/// embedded into the calling code at compile time.
/// 
/// These error codes are used in conjunction with the Error record struct
/// to provide structured error information to API clients.
/// </summary>
public static class ErrorCodes
{
    /// <summary>
    /// Error code for resource not found scenarios.
    /// 
    /// General: Used when a requested resource (country, stay, user, etc.)
    /// doesn't exist in the database.
    /// 
    /// Technical: Typically maps to HTTP 404 Not Found status code.
    /// </summary>
    public const string NotFound = "NOT_FOUND";

    /// <summary>
    /// Error code for bad request scenarios.
    /// 
    /// General: Used when the client sends invalid data or malformed requests.
    /// Examples: missing required fields, invalid email format, validation failures.
    /// 
    /// Technical: Typically maps to HTTP 400 Bad Request status code.
    /// </summary>
    public const string BadRequest = "BAD_REQUEST";

    /// <summary>
    /// Error code for validation errors.
    /// 
    /// General: Used when business rules or data validation fails.
    /// This is more specific than BadRequest and indicates the data structure
    /// is correct but the values violate business rules.
    /// 
    /// Technical: Can be used for custom validation beyond data annotations.
    /// </summary>
    public const string ValidationError = "ValidationError";

    /// <summary>
    /// Error code for conflict scenarios.
    /// 
    /// General: Used when the request conflicts with existing data.
    /// Example: trying to create a country with a code that already exists.
    /// 
    /// Technical: Typically maps to HTTP 409 Conflict status code.
    /// This is commonly used for duplicate key violations in the database.
    /// </summary>
    public const string Conflict = "CONFLICT";

    /// <summary>
    /// Error code for database-related errors.
    /// 
    /// General: Used when database operations fail unexpectedly.
    /// Examples: connection failures, constraint violations, timeout errors.
    /// 
    /// Technical: This is a catch-all for database-specific errors that
    /// don't fit into other categories. Should be logged for investigation.
    /// </summary>
    public const string DatabaseError = "DATABASE_ERROR";

    /// <summary>
    /// Error code for concurrency conflicts.
    /// 
    /// General: Used when multiple users try to modify the same data simultaneously.
    /// This happens when User A reads data, User B modifies it, then User A tries
    /// to save their version (which is now stale).
    /// 
    /// Technical: Typically maps to HTTP 409 Conflict status code.
    /// Entity Framework tracks concurrency using a "concurrency token" (a field
    /// that changes on every update). If the token doesn't match, this error is raised.
    /// </summary>
    public const string ConcurrencyError = "CONCURRENCY_ERROR";
}