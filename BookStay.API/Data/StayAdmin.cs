namespace BookStay.API.Data;

/// <summary>
/// Entity representing the relationship between a Stay and an Administrator user.
/// 
/// General: This entity allows multiple users to be administrators of a specific stay/accommodation.
/// This is useful for properties that have multiple managers or owners.
/// 
/// Technical: This is a many-to-many relationship table between Stay and ApplicationUser.
/// A user can be an admin of multiple stays, and a stay can have multiple admins.
/// Entity Framework will map this to a join table in the database.
/// </summary>
public class StayAdmin
{
    /// <summary>
    /// Primary key for the StayAdmin entity.
    /// 
    /// General: Unique identifier for each stay-admin relationship.
    /// 
    /// Technical: Auto-incrementing integer primary key.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Navigation property to the related Stay entity.
    /// 
    /// General: The stay/accommodation this user administers.
    /// 
    /// Technical: Represents the many-to-one relationship (many admins belong to one stay).
    /// Entity Framework uses this to load the related Stay when querying StayAdmins.
    /// </summary>
    public Stay? Stay { get; set; }

    /// <summary>
    /// Foreign key to the Stay entity.
    /// 
    /// General: ID of the stay this user administers.
    /// 
    /// Technical: This is a foreign key that references the Stay table's Id column.
    /// Entity Framework uses this to establish the relationship between StayAdmin and Stay.
    /// </summary>
    public int StayId { get; set; }

    /// <summary>
    /// Navigation property to the related ApplicationUser entity.
    /// 
    /// General: The user who is an administrator of this stay.
    /// 
    /// Technical: Represents the many-to-one relationship (many stay-admin records belong to one user).
    /// Entity Framework uses this to load the related ApplicationUser when querying StayAdmins.
    /// </summary>
    public ApplicationUser? User { get; set; }

    /// <summary>
    /// Foreign key to the ApplicationUser entity.
    /// 
    /// General: ID of the user who administers this stay.
    /// 
    /// Technical: This is a foreign key that references the AspNetUsers table's Id column.
    /// The [required] modifier ensures this field cannot be null.
    /// </summary>
    public required string UserId { get; set; }
}
