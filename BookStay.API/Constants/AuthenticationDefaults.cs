namespace BookStay.API.Constants;

/// <summary>
/// Contains default values and constants used for authentication configuration.
/// 
/// General: This class stores string constants that are used throughout the application
/// for authentication-related configuration. Using constants instead of hardcoded strings
/// makes the code more maintainable and prevents typos.
/// 
/// Technical: A static class with const fields is a common C# pattern for constants.
/// const values are compiled into the calling code, making them efficient.
/// These values are used in Program.cs to configure authentication schemes.
/// </summary>
public class AuthenticationDefaults 
{
    /// <summary>
    /// The name of the Basic Authentication scheme.
    /// 
    /// General: This identifier is used to register and reference our custom
    /// Basic Authentication handler in the ASP.NET Core authentication system.
    /// 
    /// Technical: When we call AddScheme in Program.cs, we pass this string
    /// as the scheme name. The authentication middleware uses this name to
    /// identify which handler to use for authentication.
    /// "Basic" is the standard name for Basic Authentication in HTTP.
    /// </summary>
    public const string BasicScheme="Basic";

    /// <summary>
    /// The name of the application.
    /// 
    /// General: This can be used for display purposes, logging, or configuration.
    /// It identifies the application in authentication contexts.
    /// 
    /// Technical: This constant might be used in JWT token generation,
    /// cookie configuration, or other authentication-related settings.
    /// Currently it's defined but may not be actively used in the current implementation.
    /// </summary>
    public const string AppName = "BookStayApi";

    public const string ApiKeyHeaderName = "X-Api-Key";
    public const string ApiKeyScheme="ApiKey"; 
        

}