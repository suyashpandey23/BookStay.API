namespace BookStay.API.Enums;

/// <summary>
/// Enumeration representing the status of a booking.
/// 
/// General: This enum tracks the lifecycle of a booking from creation to completion.
/// 
/// Technical: Used in the Booking entity to track status. Values are:
/// - Pending: Booking has been created but not yet confirmed
/// - Confirmed: Booking has been confirmed and is active
/// - Cancelled: Booking has been cancelled by the user or system
/// </summary>
public enum BookingStatus
{
    /// <summary>
    /// Booking has been created but not yet confirmed.
    /// </summary>
    Pending,

    /// <summary>
    /// Booking has been confirmed and is active.
    /// </summary>
    Confirmed,

    /// <summary>
    /// Booking has been cancelled.
    /// </summary>
    Cancelled
}
