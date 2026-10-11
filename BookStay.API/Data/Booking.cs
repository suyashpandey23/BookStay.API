using BookStay.API.Enums;

namespace BookStay.API.Data;

/// <summary>
/// Entity representing a booking for a stay/accommodation.
/// 
/// General: This class represents a reservation made by a user for a specific stay.
/// It tracks the booking details, dates, guests, and status.
/// 
/// Technical: This is an Entity Framework entity class that maps to the Bookings table.
/// It has foreign key relationships with Stay and ApplicationUser.
/// </summary>
public class Booking
{
    /// <summary>
    /// Primary key for the Booking entity.
    /// 
    /// General: Unique identifier for each booking.
    /// 
    /// Technical: Auto-incrementing integer primary key.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Foreign key to the Stay entity.
    /// 
    /// General: ID of the stay/accommodation being booked.
    /// 
    /// Technical: This is a foreign key that references the Stay table's Id column.
    /// The [required] modifier ensures this field cannot be null.
    /// </summary>
    public required int StayId { get; set; }

    /// <summary>
    /// Navigation property to the related Stay entity.
    /// 
    /// General: The stay/accommodation that is being booked.
    /// 
    /// Technical: This navigation property represents the many-to-one relationship
    /// (many bookings belong to one stay). Entity Framework uses this to load
    /// the related Stay when querying bookings.
    /// </summary>
    public Stay? Stay { get; set; }

    /// <summary>
    /// Foreign key to the ApplicationUser entity.
    /// 
    /// General: ID of the user who made this booking.
    /// 
    /// Technical: This is a foreign key that references the AspNetUsers table's Id column.
    /// The [required] modifier ensures this field cannot be null.
    /// </summary>
    public required string UserId { get; set; }

    /// <summary>
    /// Navigation property to the related ApplicationUser entity.
    /// 
    /// General: The user who made this booking.
    /// 
    /// Technical: This navigation property represents the many-to-one relationship
    /// (many bookings belong to one user). Entity Framework uses this to load
    /// the related ApplicationUser when querying bookings.
    /// </summary>
    public ApplicationUser? User { get; set; }

    /// <summary>
    /// The check-in date for the booking.
    /// 
    /// General: The date when the guest will arrive at the accommodation.
    /// 
    /// Technical: DateOnly type is used for dates without time components,
    /// which is appropriate for check-in/check-out dates.
    /// </summary>
    public DateOnly CheckIn { get; set; }

    /// <summary>
    /// The check-out date for the booking.
    /// 
    /// General: The date when the guest will leave the accommodation.
    /// 
    /// Technical: DateOnly type is used for dates without time components.
    /// This should be after the CheckIn date.
    /// </summary>
    public DateOnly CheckOut { get; set; }

    /// <summary>
    /// The number of guests for this booking.
    /// 
    /// General: How many people will be staying at the accommodation.
    /// 
    /// Technical: Integer value representing the guest count.
    /// This can be used for capacity validation.
    /// </summary>
    public int Guests { get; set; }

    /// <summary>
    /// The total price for the booking.
    /// 
    /// General: The total amount charged for this booking.
    /// 
    /// Technical: decimal type is used for financial calculations to avoid
    /// floating-point precision issues. This is typically calculated as
    /// (CheckOut - CheckIn).Days * Stay.PerNightCharge.
    /// </summary>
    public decimal TotalPrice { get; set; }

    /// <summary>
    /// The UTC timestamp when the booking was created.
    /// 
    /// General: When the booking was first made.
    /// 
    /// Technical: DateTime.UtcNow is used to ensure consistent UTC timestamps.
    /// This defaults to the current UTC time when a new booking is created.
    /// </summary>
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// The UTC timestamp when the booking was last updated.
    /// 
    /// General: When the booking was last modified.
    /// 
    /// Technical: Nullable DateTime - null if the booking has never been updated.
    /// This can be used for tracking modifications and audit purposes.
    /// </summary>
    public DateTime? UpdatedAtUtc { get; set; }

    /// <summary>
    /// The current status of the booking.
    /// 
    /// General: Tracks whether the booking is pending, confirmed, or cancelled.
    /// 
    /// Technical: Uses the BookingStatus enum to ensure only valid status values.
    /// Defaults to BookingStatus.Pending when a new booking is created.
    /// </summary>
    public BookingStatus Status { get; set; } = BookingStatus.Pending;
}
