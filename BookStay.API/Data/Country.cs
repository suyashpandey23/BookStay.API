using System.ComponentModel.DataAnnotations;

namespace BookStay.API.Data;

/// <summary>
/// Entity representing a country in the system.
/// 
/// General: This class represents a country (e.g., USA, France, Japan) in the database.
/// Countries are used to categorize stays/accommodations by location.
/// 
/// Technical: This is an Entity Framework entity class. Each property maps to a column
/// in the Countries table. Entity Framework uses this class to generate the database
/// schema and to map data between the database and C# objects.
/// 
/// Data annotations ([Required], [MaxLength], etc.) are used for validation
/// and also influence the database schema generation (e.g., NOT NULL constraints).
/// </summary>
public class Country 
{
    /// <summary>
    /// Primary key for the Country entity.
    /// 
    /// General: Unique identifier for each country.
    /// 
    /// Technical: By convention, Entity Framework treats a property named "Id"
    /// as the primary key. This will be an auto-incrementing integer in the database.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// The name of the country.
    /// 
    /// General: Full name of the country (e.g., "United States", "France").
    /// 
    /// Technical: [Required] makes this column NOT NULL in the database.
    /// [MaxLength(100)] limits the column to 100 characters.
    /// Initialized to string.Empty to prevent null reference issues.
    /// </summary>
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// The ISO country code.
    /// 
    /// General: Standard 2-3 letter country code (e.g., "US", "FRA", "JPN").
    /// 
    /// Technical: [Required] makes this column NOT NULL.
    /// [MaxLength(3)] and [MinLength(2)] enforce the length constraint.
    /// This ensures codes are in the standard ISO format.
    /// </summary>
    [Required]
    [MaxLength(3)]
    [MinLength(2)]
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// The continent where the country is located.
    /// 
    /// General: Continent name (e.g., "North America", "Europe", "Asia").
    /// 
    /// Technical: [Required] makes this column NOT NULL.
    /// [MaxLength(50)] limits the column to 50 characters.
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string Continent { get; set; } = string.Empty;
    
    /// <summary>
    /// Navigation property for stays in this country.
    /// 
    /// General: A list of all accommodations located in this country.
    /// 
    /// Technical: This is a navigation property that represents a one-to-many
    /// relationship (one country has many stays). Entity Framework uses this to
    /// load related stays when querying countries.
    /// 
    /// IList<Stay>? is a nullable list of Stay objects.
    /// The '?' indicates the collection can be null.
    /// Entity Framework will populate this when using Include() in queries.
    /// </summary>
    public IList<Stay>? Stays { get; set; }
}