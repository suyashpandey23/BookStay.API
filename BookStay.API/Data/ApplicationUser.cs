using Microsoft.AspNetCore.Identity;

namespace BookStay.API.Data;

/// <summary>
/// Custom user entity that extends ASP.NET Core Identity's built-in IdentityUser.
/// 
/// General: This represents a user in our application's database.
/// It inherits all the standard IdentityUser properties (Id, Email, PasswordHash, etc.)
/// and adds our custom properties (FirstName, LastName).
/// 
/// Technical: IdentityUser is a base class from ASP.NET Core Identity that provides:
/// - Id (string): Unique identifier (GUID)
/// - UserName (string): Login username
/// - NormalizedUserName (string): Uppercase version for case-insensitive lookups
/// - Email (string): User's email address
/// - NormalizedEmail (string): Uppercase email for lookups
/// - EmailConfirmed (bool): Whether email has been verified
/// - PasswordHash (string): Hashed password (never plain text!)
/// - SecurityStamp (string): Changes when security-related info changes
/// - ConcurrencyStamp (string): For optimistic concurrency control
/// - PhoneNumber (string): Optional phone number
/// - PhoneNumberConfirmed (bool): Whether phone is verified
/// - TwoFactorEnabled (bool): Whether 2FA is enabled
/// - LockoutEnd (DateTimeOffset?): When lockout expires (if locked)
/// - LockoutEnabled (bool): Whether account can be locked
/// - AccessFailedCount (int): Number of failed login attempts
/// 
/// Entity Framework maps this class to the "AspNetUsers" table in the database.
/// </summary>
public class ApplicationUser: IdentityUser 
{
    /// <summary>
    /// User's first name.
    /// 
    /// General: Stores the user's given name for display purposes.
    /// 
    /// Technical: Initialized to string.Empty to prevent null reference issues.
    /// This is a custom property added to extend the base IdentityUser.
    /// Entity Framework will create a "FirstName" column in the database.
    /// </summary>
    public string FirstName {get; set;}=string.Empty;

    /// <summary>
    /// User's last name (family name).
    /// 
    /// General: Stores the user's family name for display purposes.
    /// 
    /// Technical: Same as FirstName - initialized to string.Empty.
    /// This is a custom property that extends the base IdentityUser.
    /// Entity Framework will create a "LastName" column in the database.
    /// </summary>
    public string LastName {get; set;}=string.Empty;
}
