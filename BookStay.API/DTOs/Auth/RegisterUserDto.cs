using System.ComponentModel.DataAnnotations;

namespace BookStay.API.DTOs.Auth;

/// <summary>
/// Data Transfer Object (DTO) for user registration.
/// A DTO is a simple object used to transfer data between processes.
/// In this case, it's used to receive registration data from the client API request.
/// 
/// Technical Note: The [Required] and other validation attributes come from
/// System.ComponentModel.DataAnnotations and are used for automatic model validation.
/// ASP.NET Core automatically validates these before the controller action executes.
/// </summary>
public class RegisterUserDto
{
    /// <summary>
    /// User's email address.
    /// [Required]: Ensures this field cannot be null or empty. If validation fails,
    /// the specified error message is returned to the client.
    /// [EmailAddress]: Validates that the string is in a valid email format.
/// 
/// Technical: The 'string.Empty' initialization ensures the property is never null,
/// which helps prevent NullReferenceExceptions in code.
/// </summary>
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// User's password for authentication.
    /// [Required]: Ensures the password must be provided.
    /// [MinLength(6)]: Enforces minimum password length of 6 characters for security.
/// 
/// Technical: Passwords should be hashed before storing in the database.
/// The UserManager (used in AuthService) handles this hashing automatically.
/// </summary>
    [Required(ErrorMessage = "Password is required")]
    [MinLength(6, ErrorMessage = "Password must be at least 6 characters")]
    public string Password { get; set; } = string.Empty;

    /// <summary>
    /// User's first name (optional).
    /// [MaxLength(100)]: Limits the length to prevent excessively long values.
/// 
/// Technical: The 'string?' type indicates this is a nullable string.
/// The '?' after 'string' means this property can be null.
/// This is different from 'string.Empty' - null means "no value provided".
/// </summary>
    [MaxLength(100, ErrorMessage = "First name cannot exceed 100 characters")]
    public string? FirstName { get; set; }

    /// <summary>
    /// User's last name (optional).
    /// [MaxLength(100)]: Limits the length to prevent excessively long values.
/// 
/// Technical: Same as FirstName - nullable string allows this to be null.
/// </summary>
    [MaxLength(100, ErrorMessage = "Last name cannot exceed 100 characters")]
    public string? LastName { get; set; }
}