using AutoMapper;
using BookStay.API.Constants;
using BookStay.API.Contracts;
using BookStay.API.DTOs.Booking;
using BookStay.API.Enums;
using BookStay.API.Filtering;
using BookStay.API.Results;
using BookStay.API.Data;
using Microsoft.EntityFrameworkCore;

namespace BookStay.API.Services;

/// <summary>
/// Implementation of the IBookingService interface.
/// 
/// General: This service handles the business logic for booking operations including
/// creating, updating, canceling, and querying bookings. It includes validation
/// for date overlaps and price calculations.
/// 
/// Technical: Uses Entity Framework Core for database operations and AutoMapper
/// for mapping between entities and DTOs. Constructor uses primary constructor syntax
/// (C# 12 feature) for dependency injection.
/// 
/// StayListingDBContext: Provides access to the database.
/// IAuthService: Used to get the current authenticated user's ID.
/// IMapper: Used for object mapping between entities and DTOs.
/// </summary>
public class BookingService(
    StayListingDBContext context,
    IAuthService authService,
    IMapper mapper) : IBookingService
{
    /// <summary>
    /// Gets the current user's ID from the auth service.
    /// 
    /// General: Retrieves the authenticated user's ID for authorization.
    /// 
    /// Technical: Uses the IAuthService.UserId property which checks both
    /// JwtRegisteredClaimNames.Sub and ClaimTypes.NameIdentifier claims.
    /// </summary>
    private string UserId => authService.UserId;

    /// <summary>
    /// Retrieves all bookings for a specific stay (admin view).
    /// 
    /// General: Gets paginated and filtered bookings for a stay.
    /// This is intended for administrators to view all bookings for a stay.
    /// 
    /// Technical: Validates the stay exists, applies filters, and returns paginated results.
    /// 
    /// Line-by-line:
    /// - var stayExists = await context.Stays.AnyAsync(s => s.Id == stayId): 
    ///   Checks if a stay with the given ID exists in the database.
    /// - if (!stayExists): If the stay doesn't exist, return a NotFound error.
    /// - return Result<PagedResult<GetBookingDto>>.Failure(...): Returns a failure result
    ///   with an error indicating the stay was not found.
    /// - var query = ApplyFilters(stayId, filters): Builds a LINQ query with applied filters.
    /// - var bookings = await query.Include(b => b.Stay).ToListAsync(): 
    ///   Executes the query and includes the related Stay entity for each booking.
    /// - var bookingDtos = mapper.Map<List<GetBookingDto>>(bookings): 
    ///   Uses AutoMapper to convert Booking entities to GetBookingDto objects.
    /// - var pagedResult = new PagedResult<GetBookingDto> { ... }: Creates the paginated result.
    /// - Items = bookingDtos.Skip(...).Take(...): Skips previous pages and takes the current page.
    /// - TotalCount = bookings.Count: Sets the total count of all matching bookings.
    /// - PageNumber = paginationParameters.PageNumber: Sets the current page number.
    /// - PageSize = paginationParameters.PageSize: Sets the page size.
    /// - return Result<PagedResult<GetBookingDto>>.Success(pagedResult): Returns success with data.
    /// </summary>
    public async Task<Result<PagedResult<GetBookingDto>>> GetBookingsForStayAsync(
        int stayId,
        PaginationParameters paginationParameters,
        BookingFilterParameters filters)
    {
        var stayExists = await context.Stays.AnyAsync(s => s.Id == stayId);
        if (!stayExists)
            return Result<PagedResult<GetBookingDto>>.Failure(
                new Error(ErrorCodes.NotFound, $"Stay '{stayId}' was not found."));

        var query = ApplyFilters(stayId, filters);
        var bookings = await query
            .Include(b => b.Stay)
            .ToListAsync();

        var bookingDtos = mapper.Map<List<GetBookingDto>>(bookings);
        var pagedResult = new PagedResult<GetBookingDto>
        {
            Items = bookingDtos.Skip((paginationParameters.PageNumber - 1) * paginationParameters.PageSize)
                               .Take(paginationParameters.PageSize),
            TotalCount = bookings.Count,
            PageNumber = paginationParameters.PageNumber,
            PageSize = paginationParameters.PageSize
        };

        return Result<PagedResult<GetBookingDto>>.Success(pagedResult);
    }

    /// <summary>
    /// Retrieves bookings for a specific stay for the current user.
    /// 
    /// General: Gets paginated and filtered bookings for a stay that belong to
    /// the currently authenticated user.
    /// 
    /// Technical: Similar to GetBookingsForStayAsync but filters by the current user's ID.
    /// 
    /// Line-by-line:
    /// - var userId = UserId: Gets the current authenticated user's ID from the auth service.
    /// - var stayExists = await context.Stays.AnyAsync(s => s.Id == stayId): 
    ///   Checks if the stay exists in the database.
    /// - if (!stayExists): Returns NotFound error if stay doesn't exist.
    /// - var query = ApplyFilters(stayId, filters): Builds the filtered query.
    /// - var bookings = await query.Where(b => b.UserId == userId).Include(b => b.Stay).ToListAsync(): 
    ///   Filters to only the current user's bookings and includes the Stay entity.
    /// - var bookingDtos = mapper.Map<List<GetBookingDto>>(bookings): Maps entities to DTOs.
    /// - var pagedResult = new PagedResult<GetBookingDto> { ... }: Creates paginated result.
    /// - Items = bookingDtos.Skip(...).Take(...): Applies pagination to the results.
    /// - TotalCount = bookings.Count: Sets total count of user's bookings.
    /// - PageNumber = paginationParameters.PageNumber: Sets current page.
    /// - PageSize = paginationParameters.PageSize: Sets page size.
    /// - return Result<PagedResult<GetBookingDto>>.Success(pagedResult): Returns success with data.
    /// </summary>
    public async Task<Result<PagedResult<GetBookingDto>>> GetUserBookingsForStayAsync(
        int stayId,
        PaginationParameters paginationParameters,
        BookingFilterParameters filters)
    {
        var userId = UserId;

        var stayExists = await context.Stays.AnyAsync(s => s.Id == stayId);
        if (!stayExists)
            return Result<PagedResult<GetBookingDto>>.Failure(
                new Error(ErrorCodes.NotFound, $"Stay '{stayId}' was not found."));

        var query = ApplyFilters(stayId, filters);
        var bookings = await query
            .Where(b => b.UserId == userId)
            .Include(b => b.Stay)
            .ToListAsync();

        var bookingDtos = mapper.Map<List<GetBookingDto>>(bookings);
        var pagedResult = new PagedResult<GetBookingDto>
        {
            Items = bookingDtos.Skip((paginationParameters.PageNumber - 1) * paginationParameters.PageSize)
                               .Take(paginationParameters.PageSize),
            TotalCount = bookings.Count,
            PageNumber = paginationParameters.PageNumber,
            PageSize = paginationParameters.PageSize
        };

        return Result<PagedResult<GetBookingDto>>.Success(pagedResult);
    }

    /// <summary>
    /// Creates a new booking.
    /// 
    /// General: Creates a booking for the currently authenticated user.
    /// Validates date overlap and calculates total price.
    /// 
    /// Technical: Checks for date conflicts, calculates price based on stay rate,
    /// and saves the booking with Pending status.
    /// 
    /// Line-by-line:
    /// - var userId = UserId: Gets the current authenticated user's ID.
    /// - bool overlaps = await IsOverlap(dto.StayId, userId, dto.CheckIn, dto.CheckOut): 
    ///   Checks if the requested dates overlap with existing bookings for this user.
    /// - if (overlaps): Returns a Conflict error if dates overlap.
    /// - var stay = await context.Stays.Where(s => s.Id == dto.StayId).FirstOrDefaultAsync(): 
    ///   Retrieves the stay entity to get the per-night charge.
    /// - if (stay is null): Returns NotFound error if stay doesn't exist.
    /// - var nights = dto.CheckOut.DayNumber - dto.CheckIn.DayNumber: 
    ///   Calculates the number of nights (DayNumber is the day of year).
    /// - var totalPrice = stay.PerNightCharge * nights: Calculates total booking price.
    /// - var booking = mapper.Map<Booking>(dto): Maps the DTO to a Booking entity.
    /// - booking.UserId = userId: Sets the booking owner to the current user.
    /// - booking.TotalPrice = totalPrice: Sets the calculated total price.
    /// - booking.Status = BookingStatus.Pending: Sets initial status to Pending.
    /// - context.Bookings.Add(booking): Adds the booking to the DbContext.
    /// - await context.SaveChangesAsync(): Saves changes to the database.
    /// - var result = mapper.Map<GetBookingDto>(booking): Maps the saved entity back to DTO.
    /// - return Result<GetBookingDto>.Success(result): Returns success with the created booking.
    /// </summary>
    public async Task<Result<GetBookingDto>> CreateBookingAsync(CreateBookingDto dto)
    {
        var userId = UserId;
        bool overlaps = await IsOverlap(dto.StayId, userId, dto.CheckIn, dto.CheckOut);

        if (overlaps)
            return Result<GetBookingDto>.Failure(
                new Error(ErrorCodes.Conflict, "The selected dates overlap with an existing booking."));

        var stay = await context.Stays
            .Where(s => s.Id == dto.StayId)
            .FirstOrDefaultAsync();

        if (stay is null)
            return Result<GetBookingDto>.Failure(
                new Error(ErrorCodes.NotFound, $"Stay '{dto.StayId}' was not found."));

        var nights = dto.CheckOut.DayNumber - dto.CheckIn.DayNumber;
        var totalPrice = stay.PerNightCharge * nights;
        
        var booking = mapper.Map<Booking>(dto);
        booking.UserId = userId;
        booking.TotalPrice = totalPrice;
        booking.Status = BookingStatus.Pending;

        context.Bookings.Add(booking);
        await context.SaveChangesAsync();

        var result = mapper.Map<GetBookingDto>(booking);

        return Result<GetBookingDto>.Success(result);
    }

    /// <summary>
    /// Updates an existing booking.
    /// 
    /// General: Modifies an existing booking's dates and guest count.
    /// Only the booking owner can update their own booking.
    /// 
    /// Technical: Validates ownership, checks for date conflicts (excluding current booking),
    /// and recalculates the total price.
    /// 
    /// Line-by-line:
    /// - var userId = UserId: Gets the current authenticated user's ID.
    /// - bool overlaps = await IsOverlap(stayId, userId, dto.CheckIn, dto.CheckOut, bookingId): 
    ///   Checks for date conflicts, passing bookingId to exclude the current booking from the check.
    /// - if (overlaps): Returns Conflict error if new dates would overlap with other bookings.
    /// - var booking = await context.Bookings.Include(b => b.Stay).FirstOrDefaultAsync(...): 
    ///   Retrieves the booking if it belongs to the user and stay, includes Stay entity.
    /// - if (booking is null): Returns NotFound error if booking doesn't exist or user doesn't own it.
    /// - if (booking.Status == BookingStatus.Cancelled): 
    ///   Returns Conflict error if trying to modify a cancelled booking.
    /// - mapper.Map(dto, booking): Maps the DTO properties to the existing booking entity.
    /// - var perNight = booking.Stay!.PerNightCharge: Gets the per-night charge from the stay.
    /// - var nights = dto.CheckOut.DayNumber - dto.CheckIn.DayNumber: Calculates number of nights.
    /// - booking.TotalPrice = perNight * nights: Recalculates total price with new dates.
    /// - booking.UpdatedAtUtc = DateTime.UtcNow: Updates the timestamp to indicate modification.
    /// - await context.SaveChangesAsync(): Saves changes to the database.
    /// - var updated = mapper.Map<GetBookingDto>(booking): Maps the updated entity to DTO.
    /// - return Result<GetBookingDto>.Success(updated): Returns success with updated booking.
    /// </summary>
    public async Task<Result<GetBookingDto>> UpdateBookingAsync(
        int stayId,
        int bookingId,
        UpdateBookingDto dto)
    {
        var userId = UserId;

        bool overlaps = await IsOverlap(stayId, userId, dto.CheckIn, dto.CheckOut, bookingId);

        if (overlaps)
            return Result<GetBookingDto>.Failure(
                new Error(ErrorCodes.Conflict, "The selected dates overlap with an existing booking."));

        var booking = await context.Bookings
            .Include(b => b.Stay)
            .FirstOrDefaultAsync(b =>
                b.Id == bookingId
                && b.StayId == stayId
                && b.UserId == userId);

        if (booking is null)
            return Result<GetBookingDto>.Failure(
                new Error(ErrorCodes.NotFound, $"Booking '{bookingId}' was not found."));

        if (booking.Status == BookingStatus.Cancelled)
            return Result<GetBookingDto>.Failure(
                new Error(ErrorCodes.Conflict, "Cancelled bookings cannot be modified."));

        mapper.Map(dto, booking);
        var perNight = booking.Stay!.PerNightCharge;
        var nights = dto.CheckOut.DayNumber - dto.CheckIn.DayNumber;
        booking.TotalPrice = perNight * nights;
        booking.UpdatedAtUtc = DateTime.UtcNow;

        await context.SaveChangesAsync();

        var updated = mapper.Map<GetBookingDto>(booking);

        return Result<GetBookingDto>.Success(updated);
    }

    /// <summary>
    /// Checks if a booking would overlap with existing bookings.
    /// 
    /// General: Validates that the requested dates don't conflict with existing bookings.
    /// 
    /// Technical: Uses date overlap logic: two date ranges overlap if the start of one
    /// is before the end of the other AND the end of one is after the start of the other.
    /// 
    /// Line-by-line:
    /// - var query = context.Bookings.Where(...): Builds a query for bookings on this stay.
    /// - b => b.StayId == stayId: Filters to the specific stay.
    /// - && b.Status != BookingStatus.Cancelled: Excludes cancelled bookings from overlap check.
    /// - && checkIn < b.CheckOut: New check-in must be before existing check-out (overlap condition 1).
    /// - && checkOut > b.CheckIn: New check-out must be after existing check-in (overlap condition 2).
    /// - && b.UserId == userId: Only check the user's own bookings for conflicts.
    /// - .AsQueryable(): Converts to IQueryable for further LINQ operations.
    /// - if (bookingId.HasValue): If updating an existing booking, exclude it from the check.
    /// - query = query.Where(q => q.Id != bookingId.Value): Removes the current booking from the query.
    /// - return await query.AnyAsync(): Returns true if any overlapping booking exists.
    /// </summary>
    private async Task<bool> IsOverlap(int stayId, string userId, DateOnly checkIn, DateOnly checkOut, int? bookingId = null)
    {
        var query = context.Bookings
            .Where(
                    b => b.StayId == stayId
                    && b.Status != BookingStatus.Cancelled
                    && checkIn < b.CheckOut
                    && checkOut > b.CheckIn
                    && b.UserId == userId)
            .AsQueryable();

        if (bookingId.HasValue)
        {
            query = query.Where(q => q.Id != bookingId.Value);
        }

        return await query.AnyAsync();
    }

    /// <summary>
    /// Cancels a booking (user action).
    /// 
    /// General: Allows a user to cancel their own booking.
    /// Only the booking owner can cancel their booking.
    /// 
    /// Technical: Validates ownership and current status, then sets status to Cancelled.
    /// 
    /// Line-by-line:
    /// - var userId = UserId: Gets the current authenticated user's ID.
    /// - var booking = await context.Bookings.FirstOrDefaultAsync(...): 
    ///   Retrieves the booking if it belongs to the user and stay.
    /// - b.Id == bookingId: Matches the booking ID.
    /// - && b.StayId == stayId: Ensures the booking is for the specified stay.
    /// - && b.UserId == userId: Ensures the user owns the booking.
    /// - if (booking is null): Returns NotFound error if booking doesn't exist or user doesn't own it.
    /// - if (booking.Status == BookingStatus.Cancelled): 
    ///   Returns Conflict error if booking is already cancelled.
    /// - booking.Status = BookingStatus.Cancelled: Changes status to Cancelled.
    /// - booking.UpdatedAtUtc = DateTime.UtcNow: Updates the timestamp.
    /// - await context.SaveChangesAsync(): Saves changes to the database.
    /// - return Result.Success(): Returns success result without data.
    /// </summary>
    public async Task<Result> CancelBookingAsync(int stayId, int bookingId)
    {
        var userId = UserId;

        var booking = await context.Bookings
            .FirstOrDefaultAsync(b =>
                b.Id == bookingId
                && b.StayId == stayId
                && b.UserId == userId);

        if (booking is null)
            return Result.Failure(
                new Error(ErrorCodes.NotFound, $"Booking '{bookingId}' was not found."));

        if (booking.Status == BookingStatus.Cancelled)
            return Result.Failure(
                new Error(ErrorCodes.Conflict, "This booking has already been cancelled."));

        booking.Status = BookingStatus.Cancelled;
        booking.UpdatedAtUtc = DateTime.UtcNow;
        await context.SaveChangesAsync();

        return Result.Success();
    }

    /// <summary>
    /// Cancels a booking (admin action).
    /// 
    /// General: Allows an administrator to cancel any booking for a stay.
    /// Does not require the admin to be the booking owner.
    /// 
    /// Technical: Similar to CancelBookingAsync but doesn't check user ownership.
    /// 
    /// Line-by-line:
    /// - var booking = await context.Bookings.FirstOrDefaultAsync(...): 
    ///   Retrieves the booking for the stay (no user filter).
    /// - b.Id == bookingId: Matches the booking ID.
    /// - && b.StayId == stayId: Ensures the booking is for the specified stay.
    /// - if (booking is null): Returns NotFound error if booking doesn't exist.
    /// - if (booking.Status == BookingStatus.Cancelled): 
    ///   Returns Conflict error if booking is already cancelled.
    /// - booking.Status = BookingStatus.Cancelled: Changes status to Cancelled.
    /// - booking.UpdatedAtUtc = DateTime.UtcNow: Updates the timestamp.
    /// - await context.SaveChangesAsync(): Saves changes to the database.
    /// - return Result.Success(): Returns success result without data.
    /// </summary>
    public async Task<Result> AdminCancelBookingAsync(int stayId, int bookingId)
    {
        var booking = await context.Bookings
            .FirstOrDefaultAsync(b =>
                b.Id == bookingId
                && b.StayId == stayId);

        if (booking is null)
            return Result.Failure(
                new Error(ErrorCodes.NotFound, $"Booking '{bookingId}' was not found."));

        if (booking.Status == BookingStatus.Cancelled)
            return Result.Failure(
                new Error(ErrorCodes.Conflict, "This booking has already been cancelled."));

        booking.Status = BookingStatus.Cancelled;
        booking.UpdatedAtUtc = DateTime.UtcNow;
        await context.SaveChangesAsync();

        return Result.Success();
    }

    /// <summary>
    /// Confirms a booking (admin action).
    /// 
    /// General: Allows an administrator to confirm a pending booking.
    /// Changes the status from Pending to Confirmed.
    /// 
    /// Technical: Validates the booking exists and is not cancelled, then confirms it.
    /// 
    /// Line-by-line:
    /// - var booking = await context.Bookings.FirstOrDefaultAsync(...): 
    ///   Retrieves the booking for the stay.
    /// - b.Id == bookingId: Matches the booking ID.
    /// - && b.StayId == stayId: Ensures the booking is for the specified stay.
    /// - if (booking is null): Returns NotFound error if booking doesn't exist.
    /// - if (booking.Status == BookingStatus.Cancelled): 
    ///   Returns Conflict error if booking is already cancelled.
    /// - booking.Status = BookingStatus.Confirmed: Changes status to Confirmed.
    /// - booking.UpdatedAtUtc = DateTime.UtcNow: Updates the timestamp.
    /// - await context.SaveChangesAsync(): Saves changes to the database.
    /// - return Result.Success(): Returns success result without data.
    /// </summary>
    public async Task<Result> AdminConfirmBookingAsync(int stayId, int bookingId)
    {
        var booking = await context.Bookings
            .FirstOrDefaultAsync(b =>
                b.Id == bookingId
                && b.StayId == stayId);

        if (booking is null)
            return Result.Failure(
                new Error(ErrorCodes.NotFound, $"Booking '{bookingId}' was not found."));

        if (booking.Status == BookingStatus.Cancelled)
            return Result.Failure(
                new Error(ErrorCodes.Conflict, "This booking has already been cancelled."));

        booking.Status = BookingStatus.Confirmed;
        booking.UpdatedAtUtc = DateTime.UtcNow;
        await context.SaveChangesAsync();

        return Result.Success();
    }

    /// <summary>
    /// Applies filters to a booking query.
    /// 
    /// General: Builds a dynamic LINQ query based on the provided filter parameters.
    /// 
    /// Technical: Conditionally adds Where clauses based on which filter parameters
    /// are provided. Also handles sorting based on SortBy and SortDescending.
    /// 
    /// Line-by-line:
    /// - var query = context.Bookings.Where(b => b.StayId == stayId): 
    ///   Starts with a query filtered to the specific stay.
    /// - if (filters.Status.HasValue): Checks if status filter is provided.
    /// - query = query.Where(b => b.Status == filters.Status.Value): Adds status filter.
    /// - if (filters.CheckInFrom.HasValue): Checks if check-in start date filter is provided.
    /// - query = query.Where(b => b.CheckIn >= filters.CheckInFrom.Value): Adds check-in from filter.
    /// - if (filters.CheckInTo.HasValue): Checks if check-in end date filter is provided.
    /// - query = query.Where(b => b.CheckIn <= filters.CheckInTo.Value): Adds check-in to filter.
    /// - if (filters.MinPrice.HasValue): Checks if minimum price filter is provided.
    /// - query = query.Where(b => b.TotalPrice >= filters.MinPrice.Value): Adds minimum price filter.
    /// - if (filters.MaxPrice.HasValue): Checks if maximum price filter is provided.
    /// - query = query.Where(b => b.TotalPrice <= filters.MaxPrice.Value): Adds maximum price filter.
    /// - if (filters.MinGuests.HasValue): Checks if minimum guests filter is provided.
    /// - query = query.Where(b => b.Guests >= filters.MinGuests.Value): Adds minimum guests filter.
    /// - if (filters.MaxGuests.HasValue): Checks if maximum guests filter is provided.
    /// - query = query.Where(b => b.Guests <= filters.MaxGuests.Value): Adds maximum guests filter.
    /// - if (filters.CreatedAfter.HasValue): Checks if created after filter is provided.
    /// - query = query.Where(b => b.CreatedAtUtc >= filters.CreatedAfter.Value): Adds created after filter.
    /// - if (filters.CreatedBefore.HasValue): Checks if created before filter is provided.
    /// - query = query.Where(b => b.CreatedAtUtc <= filters.CreatedBefore.Value): Adds created before filter.
    /// - query = filters.SortBy?.ToLower() switch: Applies sorting based on SortBy parameter.
    /// - "checkin" => filters.SortDescending ? query.OrderByDescending(...) : query.OrderBy(...): 
    ///   Sorts by check-in date, ascending or descending based on SortDescending.
    /// - "checkout" => ...: Sorts by check-out date.
    /// - "price" => ...: Sorts by total price.
    /// - "created" => ...: Sorts by creation timestamp.
    /// - _ => query.OrderBy(b => b.CheckIn): Default sort by check-in date ascending.
    /// - return query: Returns the fully filtered and sorted query.
    /// </summary>
    private IQueryable<Booking> ApplyFilters(int stayId, BookingFilterParameters filters)
    {
        var query = context.Bookings.Where(b => b.StayId == stayId);

        if (filters.Status.HasValue)
            query = query.Where(b => b.Status == filters.Status.Value);

        if (filters.CheckInFrom.HasValue)
            query = query.Where(b => b.CheckIn >= filters.CheckInFrom.Value);

        if (filters.CheckInTo.HasValue)
            query = query.Where(b => b.CheckIn <= filters.CheckInTo.Value);

        if (filters.MinPrice.HasValue)
            query = query.Where(b => b.TotalPrice >= filters.MinPrice.Value);

        if (filters.MaxPrice.HasValue)
            query = query.Where(b => b.TotalPrice <= filters.MaxPrice.Value);

        if (filters.MinGuests.HasValue)
            query = query.Where(b => b.Guests >= filters.MinGuests.Value);

        if (filters.MaxGuests.HasValue)
            query = query.Where(b => b.Guests <= filters.MaxGuests.Value);

        if (filters.CreatedAfter.HasValue)
            query = query.Where(b => b.CreatedAtUtc >= filters.CreatedAfter.Value);

        if (filters.CreatedBefore.HasValue)
            query = query.Where(b => b.CreatedAtUtc <= filters.CreatedBefore.Value);

        query = filters.SortBy?.ToLower() switch
        {
            "checkin" => filters.SortDescending ?
                query.OrderByDescending(b => b.CheckIn) : query.OrderBy(b => b.CheckIn),
            "checkout" => filters.SortDescending ?
                query.OrderByDescending(b => b.CheckOut) : query.OrderBy(b => b.CheckOut),
            "price" => filters.SortDescending ?
                query.OrderByDescending(b => b.TotalPrice) : query.OrderBy(b => b.TotalPrice),
            "created" => filters.SortDescending ?
                query.OrderByDescending(b => b.CreatedAtUtc) : query.OrderBy(b => b.CreatedAtUtc),
            _ => query.OrderBy(b => b.CheckIn)
        };

        return query;
    }
}
