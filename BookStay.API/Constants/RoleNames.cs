namespace BookStay.API.Constants;

/// <summary>
/// Contains constant strings for role names used throughout the application.
/// 
/// General: This class defines the role names that are used for authorization
/// in the application. Using constants ensures consistency and prevents typos.
/// 
/// Technical: These role names are used in RoleConfigurations.cs to seed
/// the Identity roles in the database, and in controller attributes like
/// [Authorize(Roles = RoleNames.Administrator)] to restrict access.
/// </summary>
public static class RoleNames
{
    /// <summary>
    /// The Administrator role name.
    /// 
    /// General: Users with this role have full administrative access to the system.
    /// 
    /// Technical: Used in [Authorize] attributes to restrict endpoints to administrators only.
    /// </summary>
    public const string Administrator = "Administrator";

    /// <summary>
    /// The User role name.
    /// 
    /// General: Regular users who can access standard features of the application.
    /// 
    /// Technical: This is the default role for most authenticated users.
    /// </summary>
    public const string User = "User";

    /// <summary>
    /// The StayAdmin role name.
    /// 
    /// General: Users who can manage stay listings but may not have full administrative access.
    /// 
    /// Technical: Used for users who need to manage stays but not other system resources.
    /// </summary>
    public const string StayAdmin = "StayAdmin";
}
