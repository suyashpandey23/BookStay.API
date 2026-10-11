using BookStay.API.AuthorizationFilters;
using BookStay.API.Constants;
using BookStay.API.Contracts;
using BookStay.API.DTOs.Booking;
using BookStay.API.Filtering;
using BookStay.API.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookStay.API.Controllers;

/// <summary>
/// Controller for booking-related HTTP endpoints for a specific stay.
/// 
/// General: This controller exposes REST API endpoints for managing bookings
/// for a specific stay/accommodation. It follows REST conventions: GET for reading,
/// POST for creating, PUT for updating.
/// 
/// Technical: Inherits from BaseApiController which provides the ToActionResult
/// helper method for consistent error handling. Uses dependency injection to get
/// the IBookingService for business logic.
/// 
/// Attributes:
/// - [Route]: Defines the URL pattern as "api/stays/{stayId}/bookings"
/// - [ApiController]: Enables automatic model validation and other API features
/// - [Authorize]: Requires authentication for all endpoints
/// </summary>
[Route("api/stays/{stayId:int}/bookings")]
[ApiController]
[Authorize]
public class StayBookingsController : BaseApiController
{
    /// <summary>
    /// Service for booking business logic.
    /// 
    /// General: This service handles all the actual work for booking operations.
    /// 
    /// Technical: readonly ensures the reference cannot be changed after injection.
    /// Constructor uses primary constructor syntax (C# 12).
    /// </summary>
    private readonly IBookingService _bookingService;

    /// <summary>
    /// Constructor with dependency injection.
    /// 
    /// General: ASP.NET Core automatically provides the IBookingService implementation.
    /// 
    /// Technical: Constructor injection is the recommended way to inject dependencies.
    /// </summary>
    /// <param name="bookingService">The booking service implementation</param>
    public StayBookingsController(IBookingService bookingService)
    {
        _bookingService = bookingService;
    }

    /// <summary>
    /// Gets bookings for the current user for a specific stay.
    /// 
    /// General: Returns a paginated list of the current user's bookings for a stay.
    /// 
    /// Technical: [HttpGet] attribute maps HTTP GET requests to this method.
    /// The URL will be /api/stays/{stayId}/bookings.
    /// </summary>
    /// <param name="stayId">The stay ID</param>
    /// <param name="paginationParameters">Pagination settings</param>
    /// <param name="filters">Filter criteria</param>
    /// <returns>200 OK with paginated bookings or error status</returns>
    [HttpGet]
    public async Task<ActionResult<PagedResult<GetBookingDto>>> GetBookings(
        [FromRoute] int stayId,
        [FromQuery] PaginationParameters paginationParameters,
        [FromQuery] BookingFilterParameters filters)
    {
        var result = await _bookingService.GetUserBookingsForStayAsync(stayId, paginationParameters, filters);
        return ToActionResult(result);
    }

    /// <summary>
    /// Creates a new booking for a stay.
    /// 
    /// General: Creates a booking for the currently authenticated user.
    /// 
    /// Technical: [HttpPost] maps POST requests to this method.
    /// POST is the standard HTTP method for creating resources.
    /// On success, returns 201 Created with a Location header.
    /// </summary>
    /// <param name="stayId">The stay ID</param>
    /// <param name="createBookingDto">The booking data to create</param>
    /// <returns>.Created with Location header on success, error status on failure</returns>
    [HttpPost]
    public async Task<ActionResult<GetBookingDto>> CreateBooking(
        [FromRoute] int stayId,
        [FromBody] CreateBookingDto createBookingDto)
    {
        var result = await _bookingService.CreateBookingAsync(createBookingDto);
        
        if (!result.IsSuccess)
        {
            return (ActionResult<GetBookingDto>)MapErrorsToResponse(result.Errors);
        }
        
        return CreatedAtAction(
            "GetBookings",
            new { stayId },
            result.Value);
    }

    /// <summary>
    /// Updates an existing booking.
    /// 
    /// General: Modifies a booking's dates and guest count.
    /// Only the booking owner can update their own booking.
    /// 
    /// Technical: [HttpPut("{bookingId}")] maps PUT requests to this method.
    /// PUT is the standard HTTP method for updating resources.
    /// </summary>
    /// <param name="stayId">The stay ID</param>
    /// <param name="bookingId">The booking ID to update</param>
    /// <param name="updateBookingDto">The updated booking data</param>
    /// <returns>200 OK with updated booking or error status</returns>
    [HttpPut("{bookingId:int}")]
    public async Task<ActionResult<GetBookingDto>> UpdateBooking(
        [FromRoute] int stayId,
        [FromRoute] int bookingId,
        [FromBody] UpdateBookingDto updateBookingDto)
    {
        var result = await _bookingService.UpdateBookingAsync(stayId, bookingId, updateBookingDto);
        return ToActionResult(result);
    }

    /// <summary>
    /// Cancels a booking (user action).
    /// 
    /// General: Allows a user to cancel their own booking.
    /// Only the booking owner can cancel their booking.
    /// 
    /// Technical: [HttpPut("{bookingId}/cancel")] maps PUT requests to this method.
    /// Uses a custom route to indicate the cancel action.
    /// </summary>
    /// <param name="stayId">The stay ID</param>
    /// <param name="bookingId">The booking ID to cancel</param>
    /// <returns>204 No Content on success, error status on failure</returns>
    [HttpPut("{bookingId:int}/cancel")]
    public async Task<IActionResult> CancelBooking(
        [FromRoute] int stayId,
        [FromRoute] int bookingId)
    {
        var result = await _bookingService.CancelBookingAsync(stayId, bookingId);
        return ToActionResult(result);
    }

    /// <summary>
    /// Gets all bookings for a stay (admin view).
    /// 
    /// General: Returns a paginated list of all bookings for a stay.
    /// Accessible to system administrators and stay-specific administrators.
    /// 
    /// Technical: [StayOrSystemAdmin] restricts access to system admins or stay admins.
    /// [HttpGet("admin")] provides a separate endpoint for admin operations.
    /// </summary>
    /// <param name="stayId">The stay ID</param>
    /// <param name="paginationParameters">Pagination settings</param>
    /// <param name="filters">Filter criteria</param>
    /// <returns>200 OK with paginated bookings or error status</returns>
    [HttpGet("admin")]
    [StayOrSystemAdmin]
    public async Task<ActionResult<PagedResult<GetBookingDto>>> GetBookingsAdmin(
        [FromRoute] int stayId,
        [FromQuery] PaginationParameters paginationParameters,
        [FromQuery] BookingFilterParameters filters)
    {
        var result = await _bookingService.GetBookingsForStayAsync(stayId, paginationParameters, filters);
        return ToActionResult(result);
    }

    /// <summary>
    /// Cancels a booking (admin action).
    /// 
    /// General: Allows an administrator to cancel any booking for a stay.
    /// Does not require the admin to be the booking owner.
    /// Accessible to system administrators and stay-specific administrators.
    /// 
    /// Technical: [StayOrSystemAdmin] restricts access to system admins or stay admins.
    /// </summary>
    /// <param name="stayId">The stay ID</param>
    /// <param name="bookingId">The booking ID to cancel</param>
    /// <returns>204 No Content on success, error status on failure</returns>
    [HttpPut("{bookingId:int}/admin/cancel")]
    [StayOrSystemAdmin]
    public async Task<IActionResult> AdminCancelBooking(
        [FromRoute] int stayId,
        [FromRoute] int bookingId)
    {
        var result = await _bookingService.AdminCancelBookingAsync(stayId, bookingId);
        return ToActionResult(result);
    }

    /// <summary>
    /// Confirms a booking (admin action).
    /// 
    /// General: Allows an administrator to confirm a pending booking.
    /// Changes the status from Pending to Confirmed.
    /// Accessible to system administrators and stay-specific administrators.
    /// 
    /// Technical: [StayOrSystemAdmin] restricts access to system admins or stay admins.
    /// </summary>
    /// <param name="stayId">The stay ID</param>
    /// <param name="bookingId">The booking ID to confirm</param>
    /// <returns>204 No Content on success, error status on failure</returns>
    [HttpPut("{bookingId:int}/admin/confirm")]
    [StayOrSystemAdmin]
    public async Task<IActionResult> AdminConfirmBooking(
        [FromRoute] int stayId,
        [FromRoute] int bookingId)
    {
        var result = await _bookingService.AdminConfirmBookingAsync(stayId, bookingId);
        return ToActionResult(result);
    }
}
