using BookStay.API.DTOs.Booking;
using BookStay.API.Filtering;
using BookStay.API.Results;

namespace BookStay.API.Contracts;

/// <summary>
/// Interface for booking-related business operations.
/// 
/// General: This interface defines the contract for all operations related to bookings
/// in the BookStay application. It includes CRUD operations, status management,
/// and filtering capabilities.
/// 
/// Technical: Using an interface allows for dependency injection, loose coupling,
/// and easier testing. We can mock this interface for unit tests without needing
/// a real database. All methods return Result types for explicit error handling.
/// </summary>
public interface IBookingService
{
    /// <summary>
    /// Retrieves all bookings for a specific stay (admin view).
    /// 
    /// General: Gets paginated and filtered bookings for a stay.
    /// This is intended for administrators to view all bookings for a stay.
    /// </summary>
    /// <param name="stayId">The ID of the stay</param>
    /// <param name="paginationParameters">Pagination settings</param>
    /// <param name="filters">Filter criteria</param>
    /// <returns>Result containing paginated bookings or errors</returns>
    Task<Result<PagedResult<GetBookingDto>>> GetBookingsForStayAsync(
        int stayId,
        PaginationParameters paginationParameters,
        BookingFilterParameters filters);

    /// <summary>
    /// Retrieves bookings for a specific stay for the current user.
    /// 
    /// General: Gets paginated and filtered bookings for a stay that belong to
    /// the currently authenticated user.
    /// </summary>
    /// <param name="stayId">The ID of the stay</param>
    /// <param name="paginationParameters">Pagination settings</param>
    /// <param name="filters">Filter criteria</param>
    /// <returns>Result containing paginated bookings or errors</returns>
    Task<Result<PagedResult<GetBookingDto>>> GetUserBookingsForStayAsync(
        int stayId,
        PaginationParameters paginationParameters,
        BookingFilterParameters filters);

    /// <summary>
    /// Creates a new booking.
    /// 
    /// General: Creates a booking for the currently authenticated user.
    /// Validates date overlap and calculates total price.
    /// </summary>
    /// <param name="createBookingDto">The booking data to create</param>
    /// <returns>Result containing the created booking or errors</returns>
    Task<Result<GetBookingDto>> CreateBookingAsync(CreateBookingDto createBookingDto);

    /// <summary>
    /// Updates an existing booking.
    /// 
    /// General: Modifies an existing booking's dates and guest count.
    /// Only the booking owner can update their own booking.
    /// </summary>
    /// <param name="stayId">The ID of the stay</param>
    /// <param name="bookingId">The ID of the booking to update</param>
    /// <param name="updateBookingDto">The updated booking data</param>
    /// <returns>Result containing the updated booking or errors</returns>
    Task<Result<GetBookingDto>> UpdateBookingAsync(
        int stayId,
        int bookingId,
        UpdateBookingDto updateBookingDto);

    /// <summary>
    /// Cancels a booking (user action).
    /// 
    /// General: Allows a user to cancel their own booking.
    /// Only the booking owner can cancel their booking.
    /// </summary>
    /// <param name="stayId">The ID of the stay</param>
    /// <param name="bookingId">The ID of the booking to cancel</param>
    /// <returns>Result indicating success or failure</returns>
    Task<Result> CancelBookingAsync(int stayId, int bookingId);

    /// <summary>
    /// Cancels a booking (admin action).
    /// 
    /// General: Allows an administrator to cancel any booking for a stay.
    /// Does not require the admin to be the booking owner.
    /// </summary>
    /// <param name="stayId">The ID of the stay</param>
    /// <param name="bookingId">The ID of the booking to cancel</param>
    /// <returns>Result indicating success or failure</returns>
    Task<Result> AdminCancelBookingAsync(int stayId, int bookingId);

    /// <summary>
    /// Confirms a booking (admin action).
    /// 
    /// General: Allows an administrator to confirm a pending booking.
    /// Changes the status from Pending to Confirmed.
    /// </summary>
    /// <param name="stayId">The ID of the stay</param>
    /// <param name="bookingId">The ID of the booking to confirm</param>
    /// <returns>Result indicating success or failure</returns>
    Task<Result> AdminConfirmBookingAsync(int stayId, int bookingId);
}
