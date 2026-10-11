namespace BookStay.API.DTOs.Booking;

/// <summary>
/// Data Transfer Object for retrieving booking information.
/// 
/// General: This DTO is used to return booking data to API clients. It contains
/// the essential information about a booking without exposing internal database details.
/// 
/// Technical: Using a record type provides immutability and value-based equality.
/// This is returned from GET endpoints to provide a clean API contract.
/// The properties are read-only and set via the constructor.
/// </summary>
/// <param name="Id">The unique identifier of the booking</param>
/// <param name="StayId">The ID of the stay/accommodation being booked</param>
/// <param name="StayName">The name of the stay/accommodation</param>
/// <param name="CheckIn">The check-in date</param>
/// <param name="CheckOut">The check-out date</param>
/// <param name="Guests">The number of guests</param>
/// <param name="TotalPrice">The total price for the booking</param>
/// <param name="Status">The current status of the booking (Pending, Confirmed, Cancelled)</param>
/// <param name="CreatedAtUtc">The UTC timestamp when the booking was created</param>
/// <param name="UpdatedAtUtc">The UTC timestamp when the booking was last updated (nullable)</param>
public record GetBookingDto(
    int Id,
    int StayId,
    string StayName,
    DateOnly CheckIn,
    DateOnly CheckOut,
    int Guests,
    decimal TotalPrice,
    string Status,
    DateTime CreatedAtUtc,
    DateTime? UpdatedAtUtc
);
