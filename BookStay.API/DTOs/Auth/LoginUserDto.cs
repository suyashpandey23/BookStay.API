using System.ComponentModel.DataAnnotations;

namespace BookStay.API.DTOs.Auth;

/// <summary>
/// Data Transfer Object (DTO) for user login.
/// This DTO receives login credentials from the client.
/// It's simpler than RegisterUserDto since login only needs email and password.
/// </summary>
public class LoginUserDto
{
    /// <summary>
    /// User's email address used for login.
    /// [Required]: Ensures email must be provided.
    /// [EmailAddress]: Validates the format is a proper email.
/// 
/// Technical: Initialized to string.Empty to prevent null reference issues.
/// The validation runs before the controller action executes.
/// </summary>
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// User's password for authentication.
    /// [Required]: Ensures password must be provided.
/// 
/// Technical: The password is sent from the client and validated against
/// the hashed password stored in the database using UserManager.
/// </summary>
    [Required(ErrorMessage = "Password is required")]
    public string Password { get; set; } = string.Empty;
}
