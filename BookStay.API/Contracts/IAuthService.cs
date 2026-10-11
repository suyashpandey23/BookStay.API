using BookStay.API.DTOs.Auth;
using BookStay.API.Results;

namespace BookStay.API.Services;

/// <summary>
/// Interface for authentication-related operations.
/// 
/// General: An interface defines a contract that classes must implement.
/// This allows for dependency injection - we can inject IAuthService into controllers
/// and swap implementations without changing the controller code.
/// 
/// Technical: Using interfaces promotes loose coupling and makes testing easier.
/// We can create mock implementations for unit tests.
/// </summary>
public interface IAuthService
{
    string UserId { get; }
    /// <summary>
    /// Registers a new user in the system.
    /// 
    /// General: Takes registration data from the client, creates a user account,
    /// and returns the created user information.
    /// 
    /// Technical: Returns Task<Result<RegisteredUserDto>> because it's an async operation.
    /// Task represents an asynchronous operation that may complete in the future.
    /// Result<T> is a custom wrapper that indicates success/failure and contains
    /// either the value (on success) or errors (on failure).
    /// </summary>
    /// <param name="registerUserDto">The registration data from the client</param>
    /// <returns>A Result containing the registered user data or errors</returns>
    Task<Result<RegisteredUserDto>> Register(RegisterUserDto registerUserDto);

    /// <summary>
    /// Authenticates a user with their credentials.
    /// 
    /// General: Validates the email and password against the database.
    /// If valid, the user is authenticated; otherwise, an error is returned.
    /// 
    /// Technical: Returns Task<Result> (non-generic) because login doesn't need
    /// to return data - it just needs to indicate success or failure.
    /// The Result type contains IsSuccess flag and Error information.
    /// </summary>
    /// <param name="loginUserDto">The login credentials from the client</param>
    /// <returns>A Result containing the JWT token on success or error details on failure</returns>
    Task<Result<string>> Login(LoginUserDto loginUserDto);
}
