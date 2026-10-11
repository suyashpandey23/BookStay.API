using BookStay.API.DTOs.Auth;
using BookStay.API.Results;
using BookStay.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookStay.API.Controllers;

/// <summary>
/// Controller for handling authentication-related HTTP requests.
/// 
/// General: This controller exposes endpoints for user registration and login.
/// It acts as the entry point for client applications to authenticate users.
/// 
/// Technical: Inherits from BaseApiController which likely contains common functionality.
/// Uses dependency injection to get IAuthService for business logic.
/// 
/// Attributes:
/// - [Route]: Defines the URL pattern for this controller's endpoints.
/// - [ApiController]: Enables automatic model validation and other API features.
/// - [AllowAnonymous]: Allows unauthenticated access to these endpoints (needed for login/register).
/// </summary>
[Route("api/[controller]")]
[ApiController]
[AllowAnonymous]
public class AuthController(IAuthService authService) : BaseApiController
{
    /// <summary>
    /// HTTP POST endpoint for user registration.
    /// 
    /// General: Clients send a POST request to /api/auth/register with user data.
    /// If successful, the user is created and their info is returned.
    /// 
    /// Technical:
    /// - [HttpPost("register")]: Maps HTTP POST requests to this method.
    ///   The URL will be /api/auth/register (controller name + method name).
    /// - ActionResult<T>: Allows returning different HTTP status codes with data.
    /// - async Task: Indicates this method performs asynchronous operations.
    /// </summary>
    /// <param name="registerUserDto">Registration data from the request body (auto-bound from JSON)</param>
    /// <returns>200 OK with user data on success, 400 Bad Request with errors on failure</returns>
    [HttpPost("register")]
    public async Task<ActionResult<RegisteredUserDto>> Register(RegisterUserDto registerUserDto)
    {
        // Call the service layer to handle registration logic.
        // General: Delegate the actual work to the service (separation of concerns).
        // Technical: The service handles database operations and business rules.
        // await is used because Register is async (database I/O).
        var result = await authService.Register(registerUserDto);
        
        // Convert the Result to an appropriate HTTP response.
        // General: Transform our internal result format into an HTTP response.
        // Technical: ToActionResult is a helper method (likely in BaseApiController)
        // that converts Result<T> to ActionResult with proper status codes.
        // - Success -> 200 OK with data
        // - Failure -> 400 Bad Request with error details
        return ToActionResult(result);
    }
    
    /// <summary>
    /// HTTP POST endpoint for user login.
    /// 
    /// General: Clients send a POST request to /api/auth/login with credentials.
    /// If valid, the user is authenticated.
    /// 
    /// Technical:
    /// - [HttpPost("login")]: Maps HTTP POST requests to this method.
    ///   The URL will be /api/auth/login.
    /// - IActionResult: Allows returning different HTTP status codes without data.
    /// </summary>
    /// <param name="loginUserDto">Login credentials from the request body (auto-bound from JSON)</param>
    /// <returns>200 OK with JWT token on success, 401 Unauthorized on failure</returns>
    [HttpPost("login")]
    public async Task<ActionResult<string>> Login(LoginUserDto loginUserDto)
    {
        // Call the service layer to validate credentials.
        // General: Delegate authentication to the service.
        // Technical: The service checks the database for matching credentials.
        var result = await authService.Login(loginUserDto);
        
        // Convert the Result to an appropriate HTTP response.
        // General: Transform our internal result format into an HTTP response.
        // Technical: ToActionResult converts Result to ActionResult.
        // - Success -> 200 OK
        // - Failure -> 401 Unauthorized (for login failures)
        return ToActionResult(result);
    } 
}