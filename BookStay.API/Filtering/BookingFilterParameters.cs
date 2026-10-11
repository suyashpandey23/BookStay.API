using BookStay.API.Enums;

namespace BookStay.API.Filtering;

/// <summary>
/// Filter parameters for booking queries.
/// 
/// General: This class provides filtering options for searching and filtering bookings.
/// It allows clients to filter bookings by status, dates, price, guest count, and creation time.
/// 
/// Technical: Inherits from BaseFilterParameters to get common sorting options.
/// Each nullable property represents an optional filter - if null, that filter is not applied.
/// These parameters are used to build dynamic LINQ queries with Where clauses.
/// This enables flexible API queries without creating many different endpoints.
/// 
/// Why it's needed:
/// - Allows clients to filter bookings without backend changes
/// - Reduces the number of API endpoints needed (one endpoint with filters vs many specific endpoints)
/// - Improves performance by filtering at the database level (SQL WHERE clauses)
/// - Provides a consistent filtering pattern across the application
/// </summary>
public class BookingFilterParameters : BaseFilterParameters
{
    /// <summary>
    /// Filter by booking status.
    /// 
    /// General: Only return bookings with this status (Pending, Confirmed, or Cancelled).
    /// 
    /// Technical: When set, adds a WHERE clause: b.Status == @Status.
    /// Nullable to allow filtering by any status when not specified.
    /// </summary>
    public BookingStatus? Status { get; set; }

    /// <summary>
    /// Filter by check-in date (from).
    /// 
    /// General: Only return bookings with check-in on or after this date.
    /// 
    /// Technical: When set, adds a WHERE clause: b.CheckIn >= @CheckInFrom.
    /// Useful for finding upcoming bookings.
    /// </summary>
    public DateOnly? CheckInFrom { get; set; }

    /// <summary>
    /// Filter by check-in date (to).
    /// 
    /// General: Only return bookings with check-in on or before this date.
    /// 
    /// Technical: When set, adds a WHERE clause: b.CheckIn <= @CheckInTo.
    /// Useful for finding bookings within a date range.
    /// </summary>
    public DateOnly? CheckInTo { get; set; }

    /// <summary>
    /// Filter by check-out date (from).
    /// 
    /// General: Only return bookings with check-out on or after this date.
    /// 
    /// Technical: When set, adds a WHERE clause: b.CheckOut >= @CheckOutFrom.
    /// </summary>
    public DateOnly? CheckOutFrom { get; set; }

    /// <summary>
    /// Filter by check-out date (to).
    /// 
    /// General: Only return bookings with check-out on or before this date.
    /// 
    /// Technical: When set, adds a WHERE clause: b.CheckOut <= @CheckOutTo.
    /// </summary>
    public DateOnly? CheckOutTo { get; set; }

    /// <summary>
    /// Filter by minimum total price.
    /// 
    /// General: Only return bookings with total price at or above this amount.
    /// 
    /// Technical: When set, adds a WHERE clause: b.TotalPrice >= @MinPrice.
    /// Useful for finding expensive bookings.
    /// </summary>
    public decimal? MinPrice { get; set; }

    /// <summary>
    /// Filter by maximum total price.
    /// 
    /// General: Only return bookings with total price at or below this amount.
    /// 
    /// Technical: When set, adds a WHERE clause: b.TotalPrice <= @MaxPrice.
    /// Useful for finding affordable bookings.
    /// </summary>
    public decimal? MaxPrice { get; set; }

    /// <summary>
    /// Filter by minimum number of guests.
    /// 
    /// General: Only return bookings with at least this many guests.
    /// 
    /// Technical: When set, adds a WHERE clause: b.Guests >= @MinGuests.
    /// </summary>
    public int? MinGuests { get; set; }

    /// <summary>
    /// Filter by maximum number of guests.
    /// 
    /// General: Only return bookings with at most this many guests.
    /// 
    /// Technical: When set, adds a WHERE clause: b.Guests <= @MaxGuests.
    /// </summary>
    public int? MaxGuests { get; set; }

    /// <summary>
    /// Filter by creation date (after).
    /// 
    /// General: Only return bookings created after this date/time.
    /// 
    /// Technical: When set, adds a WHERE clause: b.CreatedAtUtc >= @CreatedAfter.
    /// Useful for finding recent bookings.
    /// </summary>
    public DateTime? CreatedAfter { get; set; }

    /// <summary>
    /// Filter by creation date (before).
    /// 
    /// General: Only return bookings created before this date/time.
    /// 
    /// Technical: When set, adds a WHERE clause: b.CreatedAtUtc <= @CreatedBefore.
    /// </summary>
    public DateTime? CreatedBefore { get; set; }
}
