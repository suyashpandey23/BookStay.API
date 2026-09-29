using BookStay.API.DTOs.Auth;
using BookStay.API.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;

namespace BookStay.API.Handlers;

/// <summary>
/// Custom authentication handler for Basic Authentication.
/// 
/// General: Basic Authentication is a simple authentication scheme where the client
/// sends credentials in the Authorization header as "Basic base64(username:password)".
/// This handler extracts those credentials, validates them, and creates an authentication ticket.
/// 
/// Technical: Inherits from AuthenticationHandler<TOptions>, which is the base class
/// for custom authentication handlers in ASP.NET Core. The HandleAuthenticateAsync method
/// is called by the authentication middleware to validate credentials.
/// 
/// Constructor parameters (primary constructor syntax):
/// - options: Configuration for the authentication scheme
/// - logger: For logging authentication events
/// - encoder: For URL encoding (used by base class)
/// - authService: Our custom service to validate credentials
/// </summary>
public class BasicAuthenticationHandler(
    IOptionsMonitor<AuthenticationSchemeOptions> options,
    ILoggerFactory logger,
    UrlEncoder encoder,
    IAuthService authService
    ) : AuthenticationHandler<AuthenticationSchemeOptions>(options, logger, encoder)
{
    /// <summary>
    /// Core method that handles the authentication logic.
/// Called automatically by ASP.NET Core authentication middleware.
/// 
/// General: This method extracts credentials from the Authorization header,
/// decodes them, validates them against our database, and returns an authentication result.
/// </summary>
    /// <returns>AuthenticateResult indicating success, failure, or no result</returns>
    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        // Check if the Authorization header exists in the request.
        // General: If no Authorization header is present, this handler can't authenticate.
        // Technical: Request.Headers is a dictionary of HTTP headers.
        // TryGetValue returns false if the header doesn't exist.
        if (!Request.Headers.TryGetValue("Authorization", out var authHeaderValues))
        {
            // Return NoResult - tells ASP.NET to try other authentication handlers.
            // General: "I can't authenticate with this scheme, try the next one."
            // Technical: NoResult means this handler didn't find credentials to validate.
            return AuthenticateResult.NoResult();
        }

        // Get the Authorization header value as a string.
        // General: Convert the header values to a single string for processing.
        // Technical: authHeaderValues is a StringValues struct (can have multiple values).
        // ToString() converts it to a regular string.
        var authHeader = authHeaderValues.ToString();
        
        // Check if the header starts with "Basic " (case-insensitive).
        // General: Basic auth headers must start with "Basic " followed by the token.
        // Technical: StartsWith with OrdinalIgnoreCase compares strings ignoring case
        // but using byte-by-byte comparison (faster than culture-aware comparison).
        if (string.IsNullOrWhiteSpace(authHeader) || !authHeader.StartsWith("Basic ", StringComparison.OrdinalIgnoreCase))
        {
            // Not a Basic auth header - let other handlers try.
            return AuthenticateResult.NoResult();
        }

        // Extract the Base64 token (everything after "Basic ").
        // General: Remove the "Basic " prefix to get just the encoded credentials.
        // Technical: This uses C# range operator [..] to get a substring.
        // "Basic ".Length.. means "from index 6 to the end".
        // Trim() removes any extra whitespace.
        var token = authHeader["Basic ".Length..].Trim();
        string decoded;

        try
        {
            // Decode the Base64 token to get the original string.
            // General: Convert the Base64-encoded string back to "username:password".
            // Technical: Convert.FromBase64String converts Base64 to a byte array.
            // Encoding.UTF8.GetString converts those bytes to a string.
            var credentialBytes = Convert.FromBase64String(token);
            // {username:password}
            decoded = Encoding.UTF8.GetString(credentialBytes);
        }
        catch
        {
            // If Base64 decoding fails, the token is invalid.
            // General: The client sent malformed data.
            // Technical: FromBase64String throws FormatException if input isn't valid Base64.
            // Fail() returns a failed authentication result with an error message.
            return AuthenticateResult.Fail("Invalid Basic authentication token.");
        }

        // Find the colon separator between username and password.
        // General: Basic auth format is "username:password", so we need to split them.
        // Technical: IndexOf returns the position of the first ':' character.
        var separatorIndex = decoded.IndexOf(':');
        
        // Validate the format - must have a colon with content on both sides.
        // General: Ensure we actually have both a username and password.
        // Technical: separatorIndex <= 0 means no username (colon at start or not found).
        if (separatorIndex <= 0)
        {
            return AuthenticateResult.Fail("Invalid Basic authentication credentials format.");
        }

        // Extract username (everything before the colon).
        // General: Get the username part of the credentials.
        // Technical: Range operator [..separatorIndex] gets characters from start to separatorIndex.
        var userNameOrEmail = decoded[..separatorIndex];
        
        // Extract password (everything after the colon).
        // General: Get the password part of the credentials.
        // Technical: Range operator [(separatorIndex + 1)..] gets characters after the colon to end.
        var password = decoded[(separatorIndex + 1)..];

        // Create a LoginUserDto with the extracted credentials.
        // General: Prepare the data to send to our authentication service.
        // Technical: We use our existing DTO to maintain consistency with the login endpoint.
        var loginDto = new LoginUserDto
        {
            Email = userNameOrEmail,
            Password = password
        };

        // Validate the credentials using our AuthService.
        // General: Check if these credentials are valid in our database.
        // Technical: await because Login is async (database operation).
        var result = await authService.Login(loginDto);
        
        // If authentication failed, return an error.
        // General: The username or password was incorrect.
        // Technical: result.IsSuccess is false when login fails.
        if (!result.IsSuccess)
        {
            return AuthenticateResult.Fail("Invalid username or password.");
        }

        // Create claims for the authenticated user.
        // General: Claims are pieces of information about the user (like their name).
        // Technical: Claims are used in ASP.NET Core authorization.
        // ClaimTypes.Name is a standard claim type for the user's identifier.
        var claims = new List<Claim>
        {
            new(ClaimTypes.Name, userNameOrEmail),
        };
        
        // Create a ClaimsIdentity from the claims.
        // General: The identity represents the authenticated user.
        // Technical: ClaimsIdentity groups claims and associates them with an authentication type.
        // Scheme.Name is the name of this authentication scheme ("Basic").
        var identity = new ClaimsIdentity(claims, Scheme.Name);
        
        // Create a ClaimsPrincipal from the identity.
        // General: The principal represents the user in the ASP.NET Core system.
        // Technical: ClaimsPrincipal can contain multiple identities (for multi-factor auth).
        // It's what User property in controllers represents.
        var principal = new ClaimsPrincipal(identity);
        
        // Create an authentication ticket.
        // General: The ticket is the proof of authentication.
        // Technical: AuthenticationTicket contains the principal and authentication scheme info.
        // It's what gets stored in the authentication cookie or returned to the client.
        var ticket = new AuthenticationTicket(principal, Scheme.Name);

        // Return success with the authentication ticket.
        // General: Authentication successful! The user is now logged in.
        // Technical: Success() tells ASP.NET Core that authentication succeeded.
        // The ticket is stored and the user can access protected resources.
        return AuthenticateResult.Success(ticket);
    }
}
