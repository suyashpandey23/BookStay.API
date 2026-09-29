using System.ComponentModel.DataAnnotations;

namespace BookStay.API.Data;

/// <summary>
/// Entity representing a stay/accommodation in the system.
/// 
/// General: This class represents an accommodation (hotel, apartment, etc.)
/// that users can book. Each stay is located in a specific country.
/// 
/// Technical: This is an Entity Framework entity class that maps to the Stays table.
/// It has a foreign key relationship with Country (many stays belong to one country).
/// Data annotations define validation rules and database constraints.
/// </summary>
public class Stay 
{
    /// <summary>
    /// Primary key for the Stay entity.
    /// 
    /// General: Unique identifier for each accommodation.
    /// 
    /// Technical: Auto-incrementing integer primary key.
    /// </summary>
    public int Id {get; set;}

    /// <summary>
    /// The name of the accommodation.
    /// 
    /// General: Name of the hotel, apartment, or other accommodation.
    /// 
    /// Technical: [Required] makes this column NOT NULL.
    /// [MaxLength(200)] limits to 200 characters.
    /// </summary>
    [Required]
    [MaxLength(200)]
    public string Name {get; set;} = string.Empty;

    /// <summary>
    /// The street address of the accommodation.
    /// 
    /// General: Physical address where the accommodation is located.
    /// 
    /// Technical: [Required] makes this column NOT NULL.
    /// [MaxLength(500)] limits to 500 characters.
    /// </summary>
    [Required]
    [MaxLength(500)]
    public string Address {get; set;} = string.Empty;

    /// <summary>
    /// The city where the accommodation is located.
    /// 
    /// General: City name (e.g., "Paris", "New York", "Tokyo").
    /// 
    /// Technical: [Required] makes this column NOT NULL.
    /// [MaxLength(100)] limits to 100 characters.
    /// </summary>
    [Required]
    [MaxLength(100)]
    public string City {get; set;} = string.Empty;

    /// <summary>
    /// The rating of the accommodation.
    /// 
    /// General: Rating from 0 to 5 stars (e.g., 4.5 for a 4.5-star hotel).
    /// 
    /// Technical: [Range(0, 5)] enforces that the value must be between 0 and 5.
    /// This is a double (floating-point number) to allow decimal ratings.
    /// </summary>
    [Range(0, 5)]
    public double Rating {get; set;}

    /// <summary>
    /// URL to an image of the accommodation.
    /// 
    /// General: Link to a photo of the property.
    /// 
    /// Technical: [MaxLength(500)] limits to 500 characters.
    /// No [Required] attribute means this field is optional.
    /// </summary>
    [MaxLength(500)]
    public string ImageUrl {get; set;} = string.Empty;
    
    /// <summary>
    /// Foreign key to the Country entity.
    /// 
    /// General: ID of the country where this accommodation is located.
    /// 
    /// Technical: [Required] makes this column NOT NULL.
    /// This is a foreign key that references the Country table's Id column.
    /// Entity Framework uses this to establish the relationship between
    /// Stay and Country entities.
    /// 
    /// By convention, Entity Framework recognizes "CountryId" as the foreign key
    /// for the "Country" navigation property.
    /// </summary>
    [Required]
    public int CountryId {get; set;}

    /// <summary>
    /// Navigation property to the related Country entity.
    /// 
    /// General: The country where this accommodation is located.
    /// 
    /// Technical: This navigation property represents the many-to-one relationship
    /// (many stays belong to one country). Entity Framework uses this to load
    /// the related Country when querying stays.
    /// 
    /// Country? is nullable - it can be null if not loaded.
    /// When using Include(), Entity Framework will populate this property.
    /// </summary>
    public Country? Country {get; set;}
}