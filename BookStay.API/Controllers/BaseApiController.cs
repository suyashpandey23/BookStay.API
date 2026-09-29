using Microsoft.AspNetCore.Mvc;
using BookStay.API.Constants;
using BookStay.API.Results;

namespace BookStay.API.Controllers;

/// <summary>
/// Base class for all API controllers in the application.
/// 
/// General: This base class provides common functionality that all controllers
/// can use, such as converting Result objects to HTTP responses. This reduces
/// code duplication and ensures consistent error handling across all controllers.
/// 
/// Technical: Inherits from ControllerBase, which is the base class for MVC controllers
/// in ASP.NET Core. ControllerBase provides common controller features like
/// accessing HttpContext, Request, Response, etc., without the view support
/// that Controller class provides (since we're building an API, not a web app).
/// 
/// The class is abstract because it's meant to be inherited from, not instantiated directly.
/// </summary>
public abstract class BaseApiController : ControllerBase
{
    /// <summary>
    /// Converts a generic Result to an appropriate HTTP response.
    /// 
    /// General: This helper method transforms our internal Result<T> into an
    /// ActionResult that ASP.NET Core can send as an HTTP response. If the
    /// operation succeeded, it returns 200 OK with the value. If it failed,
    /// it returns an appropriate error status code.
    /// 
    /// Technical: ActionResult<T> is a flexible return type that can represent
    /// different HTTP status codes. The method uses a ternary operator to choose
    /// between success and failure responses.
    /// </summary>
    /// <typeparam name="T">The type of value in the Result</typeparam>
    /// <param name="result">The Result to convert</param>
    /// <returns>200 OK with value on success, or error status code on failure</returns>
    protected ActionResult<T> ToActionResult<T>(Result<T> result)
        // If successful, return 200 OK with the value; otherwise, map errors to response.
        => result.IsSuccess ? Ok(result.Value) : MapErrorsToResponse(result.Errors);

    /// <summary>
    /// Converts a non-generic Result to an appropriate HTTP response.
    /// 
    /// General: This overload is for operations that don't return data (like delete).
    /// It returns 204 No Content on success (standard for operations with no return value).
    /// </summary>
    /// <param name="result">The Result to convert</param>
    /// <returns>204 No Content on success, or error status code on failure</returns>
    protected ActionResult ToActionResult(Result result)
        // If successful, return 204 No Content; otherwise, map the single error to response.
        => result.IsSuccess ? NoContent() : MapErrorsToResponse(new[] { result.Error });

    /// <summary>
    /// Maps error codes to appropriate HTTP status codes and responses.
    /// 
    /// General: This method determines the correct HTTP status code based on
    /// the error code and returns an appropriate error response to the client.
    /// 
    /// Technical: Uses a switch expression (C# 8 feature) to match error codes
    /// to HTTP responses. Each error code maps to a specific status code:
    /// - NotFound -> 404 Not Found
    /// - BadRequest -> 400 Bad Request
    /// - ValidationError -> 400 Bad Request
    /// - Conflict -> 409 Conflict
    /// - Any other code -> 500 Internal Server Error
    /// </summary>
    /// <param name="errors">Array of errors to map</param>
    /// <returns>ActionResult with appropriate status code and error details</returns>
    protected ActionResult MapErrorsToResponse(Error[] errors)
    {
        // If no errors, return a generic problem response.
        // General: This shouldn't happen in normal flow, but handles edge cases.
        // Technical: Problem() returns a 500 Internal Server Error response.
        if (errors is null || errors.Length == 0) return Problem();
        
        // Get the first error (primary error).
        // General: We use the first error for determining the status code.
        // Technical: Multiple errors can occur (e.g., validation errors),
        // but we only need one to determine the HTTP status code.
        var e = errors[0];

        // Map the error code to the appropriate HTTP response.
        // General: Each error type gets a different HTTP status code.
        // Technical: Switch expression matches the error code and returns
        // the corresponding ActionResult method result.
        return e.Code switch
        {
            // Resource not found -> 404 Not Found
            ErrorCodes.NotFound => NotFound(e),
            
            // Bad request (invalid data) -> 400 Bad Request
            ErrorCodes.BadRequest => BadRequest(e),
            
            // Validation error -> 400 Bad Request
            ErrorCodes.ValidationError => BadRequest(e),
            
            // Conflict (duplicate, concurrency) -> 409 Conflict
            // Note: Conflict() takes a string message, not the Error object
            ErrorCodes.Conflict => Conflict(e.Description),
            
            // Any other error -> 500 Internal Server Error with all error messages
            // General: Unexpected errors are treated as server errors.
            // Technical: Problem() returns a 500 response. We join all error
            // descriptions with semicolons to provide complete error information.
            _ => Problem(detail: string.Join("; ", errors.Select(x => x.Description)), title: e.Code)
        };
    }
}
