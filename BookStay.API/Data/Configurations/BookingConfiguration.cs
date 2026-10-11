using BookStay.API.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookStay.API.Data.Configurations;

/// <summary>
/// Entity configuration for the Booking entity.
/// 
/// General: This class defines how the Booking entity is mapped to the database,
/// including column constraints, indexes, and conversions.
/// 
/// Technical: Implements IEntityTypeConfiguration<Booking> to configure
/// Entity Framework Core mappings for the Booking table.
/// </summary>
public class BookingConfiguration : IEntityTypeConfiguration<Booking>
{
    /// <summary>
    /// Configures the Booking entity mapping.
    /// 
    /// General: Sets up database-specific configurations for the Booking table.
    /// 
    /// Technical: 
    /// - Converts Status enum to string for database storage
    /// - Adds indexes for performance optimization
    /// - Limits Status field length to 20 characters
    /// </summary>
    /// <param name="builder">The entity type builder</param>
    public void Configure(EntityTypeBuilder<Booking> builder)
    {
        /// <summary>
        /// Configure the Status property to be stored as a string.
        /// 
        /// General: Stores the booking status as a readable string in the database
        /// instead of an integer enum value.
        /// 
        /// Technical: HasConversion<string>() tells Entity Framework to convert
        /// the BookingStatus enum to its string representation when saving to the
        /// database and back to enum when reading. MaxLength(20) limits the column
        /// size to accommodate the longest enum value name.
        /// </summary>
        builder.Property(q => q.Status)
            .HasConversion<string>()
            .HasMaxLength(20);

        /// <summary>
        /// Create an index on the UserId column.
        /// 
        /// General: Improves query performance when searching for bookings by user.
        /// 
        /// Technical: This index will speed up queries like "Get all bookings for user X".
        /// </summary>
        builder.HasIndex(x => x.UserId);

        /// <summary>
        /// Create an index on the StayId column.
        /// 
        /// General: Improves query performance when searching for bookings by stay.
        /// 
        /// Technical: This index will speed up queries like "Get all bookings for stay X".
        /// </summary>
        builder.HasIndex(x => x.StayId);

        /// <summary>
        /// Create a composite index on CheckIn and CheckOut columns.
        /// 
        /// General: Improves query performance when searching for bookings by date range.
        /// 
        /// Technical: This composite index will speed up queries that filter by both
        /// check-in and check-out dates, which is common for availability checks.
        /// </summary>
        builder.HasIndex(x => new { x.CheckIn, x.CheckOut });
    }
}
