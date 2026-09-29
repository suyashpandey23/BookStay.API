using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BookStay.API.Data;

/// <summary>
/// Database context for the BookStay application.
/// 
/// General: This class represents the database session and is the main entry point
/// for interacting with the database using Entity Framework Core. It defines the tables
/// (DbSets) and their relationships.
/// 
/// Technical: Inherits from IdentityDbContext<ApplicationUser> which provides
/// built-in support for ASP.NET Core Identity (user management, roles, etc.).
/// This means our context already has tables for Users, Roles, Claims, etc.
/// We add our custom tables (Stays, Countries) on top of that.
/// 
/// DbContext is a bridge between your domain entities and the database.
/// It tracks changes to entities and generates SQL commands to persist those changes.
/// </summary>
public class StayListingDBContext : IdentityDbContext<ApplicationUser> 
{
    /// <summary>
    /// Constructor that accepts DbContextOptions.
    /// 
    /// General: This constructor is required for Entity Framework to configure
    /// the context with the database connection string and other options.
    /// 
    /// Technical: The options parameter contains configuration like the connection string,
    /// database provider (SQL Server), logging settings, etc. These are configured
    /// in Program.cs when calling AddDbContext.
    /// 
    /// base(options) passes the options to the base IdentityDbContext constructor.
    /// </summary>
    /// <param name="options">Configuration options for the context</param>
    public StayListingDBContext(DbContextOptions<StayListingDBContext> options) : base(options) 
    {
        
    }

    /// <summary>
    /// DbSet representing the Stays table.
    /// 
    /// General: This property represents the "Stays" table in the database.
    /// You can query, add, update, or delete stays through this property.
    /// 
    /// Technical: DbSet<T> is a collection of entities that maps to a database table.
    /// Entity Framework Core automatically generates SQL queries when you use LINQ
    /// on this DbSet. The table name is derived from the type name (Stay -> Stays).
    /// </summary>
    public DbSet<Stay> Stays { get; set; }

    /// <summary>
    /// DbSet representing the Countries table.
    /// 
    /// General: This property represents the "Countries" table in the database.
    /// You can query, add, update, or delete countries through this property.
    /// 
    /// Technical: Similar to Stays, this DbSet maps to the Countries table.
    /// Entity Framework tracks the state of entities retrieved through this DbSet
    /// and automatically generates the appropriate SQL commands when SaveChanges is called.
    /// </summary>
    public DbSet<Country> Countries { get; set; }
}