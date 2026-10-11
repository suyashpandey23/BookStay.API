using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BookStay.API.Constants;
using BookStay.API.Data;
using BookStay.API.DTOs.Auth;
using BookStay.API.Results;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace BookStay.API.Services;

/// <summary>
/// Implementation of the IAuthService interface.
/// 
/// General: This service handles the business logic for user registration and login.
/// It uses ASP.NET Core Identity (a built-in authentication library) to manage users.
/// 
/// Technical: The constructor uses primary constructor syntax (C# 12 feature).
/// Parameters in parentheses after the class name are automatically assigned to
/// private readonly fields. This is a cleaner way to write dependency injection.
/// 
/// UserManager: Handles user creation, deletion, password hashing, and role management.
/// SignInManager: Handles user sign-in, password validation, and authentication.
/// </summary>
public class AuthService(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IOptions<JwtSettings> jwtOptions, IHttpContextAccessor httpContextAccessor
) : IAuthService
{
    /// <summary>
    /// Registers a new user in the system.
    /// 
    /// General: Creates a new user account with the provided information,
    /// hashes the password for security, and saves to the database.
    /// </summary>
    /// <param name="registerUserDto">Registration data from the client</param>
    /// <returns>Result containing registered user data or errors</returns>
    public async Task<Result<RegisteredUserDto>> Register(RegisterUserDto registerUserDto)
    {
        // Create a new ApplicationUser instance with data from the DTO.
        // General: We're copying the data from the client request into our database model.
        // Technical: 'new ApplicationUser { ... }' uses object initializer syntax to set properties.
        // UserName is set to Email because ASP.NET Identity requires a username for login.
        var user = new ApplicationUser
        {
            Email = registerUserDto.Email,
            FirstName = registerUserDto.FirstName,
            LastName = registerUserDto.LastName,
            UserName = registerUserDto.Email
        };

        // Use UserManager to create the user with the password.
        // General: This automatically hashes the password (we don't store plain text passwords!)
        // and saves the user to the database.
        // Technical: await is used because CreateAsync is async (I/O operation to database).
        // The 'await' keyword pauses execution until the operation completes.
        var result = await userManager.CreateAsync(user, registerUserDto.Password);
        
        // Check if the user creation was successful.
        // General: If something went wrong (like duplicate email), we return the errors.
        // Technical: result.Succeeded is a boolean indicating if the operation worked.
        // IdentityResult contains a collection of errors if the operation failed.
        if (!result.Succeeded)
        {
            // Convert Identity errors to our custom Error format.
            // General: Transform the error messages into a format our API understands.
            // Technical: LINQ's Select method transforms each error into our Error type.
            // ToArray() executes the query immediately.
            var errors = result.Errors.Select(e => new Error(ErrorCodes.BadRequest, e.Description)).ToArray();
            return Result<RegisteredUserDto>.Failure(errors);
        }
        await userManager.AddToRoleAsync(user, registerUserDto.Role);
        // Create a DTO to return to the client (without sensitive data like password).
        // General: We only send back safe information - never the password!
        // Technical: Mapping from domain entity (ApplicationUser) to DTO (RegisteredUserDto)
        // is a common pattern to control what data is exposed to clients.
        var registeredUser = new RegisteredUserDto
        {
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Id = user.Id,
            Role = registerUserDto.Role
        };

        // Return success with the registered user data.
        // General: Tell the caller everything worked and send back the user info.
        // Technical: Result<T>.Success creates a successful Result containing the value.
        return Result<RegisteredUserDto>.Success(registeredUser);
    }

    /// <summary>
    /// Authenticates a user with their email and password.
    /// 
    /// General: Verifies the user's credentials against the database.
    /// If valid, the user is authenticated; otherwise, an error is returned.
    /// </summary>
    /// <param name="loginUserDto">Login credentials from the client</param>
    /// <returns>Result containing the JWT token on success or error details on failure</returns>
    public async Task<Result<string>> Login(LoginUserDto loginUserDto)
    {
        // Find the user by email in the database.
        // General: Look up the user account using their email address.
        // Technical: FindByEmailAsync queries the database for a user with matching email.
        // Returns null if no user is found with that email.
        var user = await userManager.FindByEmailAsync(loginUserDto.Email);
        
        // If user doesn't exist, return an error.
        // General: We don't tell them specifically if email or password is wrong
        // (security best practice to prevent email enumeration attacks).
        // Technical: Checking for null is a common pattern in C#.
        if (user == null)
        {
            return Result<string>.Failure(new Error(ErrorCodes.BadRequest, "Invalid email or password"));
        }

        // Verify the password matches the stored hash.
        // General: Check if the provided password is correct for this user.
        // Technical: CheckPasswordSignInAsync compares the provided password with
        // the hashed password in the database. The 'false' parameter means
        // we don't lock the account on failure (for this simple implementation).
        var result = await signInManager.CheckPasswordSignInAsync(user, loginUserDto.Password, false);
        
        // If password is wrong, return an error.
        // General: Same generic error message for security.
        // Technical: result.Succeeded indicates if the password was correct.
        if (!result.Succeeded)
        {
            return Result<string>.Failure(new Error(ErrorCodes.BadRequest, "Invalid email or password"));
        }

        // Return success - user is authenticated.
        // General: Login successful! The caller can proceed.
        // Technical: Result.Success() creates a non-generic successful Result.
        var token = await GenerateToken(user);
        return Result<string>.Success(token);
    }
    
    public string UserId => httpContextAccessor?
                                .HttpContext?
                                .User?
                                .FindFirst(JwtRegisteredClaimNames.Sub)?.Value
                            ?? httpContextAccessor?
                                .HttpContext?
                                .User?
                                .FindFirst(ClaimTypes.NameIdentifier)?.Value
                            ?? string.Empty;
    
    private async Task<string> GenerateToken(ApplicationUser user)
    {
        // Set basic user claims
        var claims = new List<Claim>
        {
            new (JwtRegisteredClaimNames.Sub, user.Id),
            new (JwtRegisteredClaimNames.Email, user.Email!),
            new (JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new (JwtRegisteredClaimNames.Name, user.FirstName)
        };

        // Set user role claims
        var roles = await userManager.GetRolesAsync(user);
        var roleClaims = roles.Select(x => new Claim(ClaimTypes.Role, x)).ToList();

        claims = claims.Union(roleClaims).ToList();

        // Set JWT Key credentials
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.Value.Key));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        // Create an encoded token
        var token = new JwtSecurityToken(
            issuer: jwtOptions.Value.Issuer,
            audience: jwtOptions.Value.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(Convert.ToInt32(jwtOptions.Value.DurationInMinutes)),
            signingCredentials: credentials
        );

        // Return token value
        return new JwtSecurityTokenHandler().WriteToken(token);
    }

}
