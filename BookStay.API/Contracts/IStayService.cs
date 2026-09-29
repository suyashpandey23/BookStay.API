using BookStay.API.DTOs.Stay;
using BookStay.API.Results;

namespace BookStay.API.Services;

/// <summary>
/// Interface for stay/accommodation-related business operations.
/// 
/// General: This interface defines the contract for all operations related to
/// stays (accommodations like hotels, apartments, etc.) in the BookStay application.
/// It follows the CRUD pattern (Create, Read, Update, Delete).
/// 
/// Technical: Similar to ICountriesService, this interface enables dependency injection
/// and testability. All methods return Result types for explicit error handling.
/// Stays are likely associated with countries (a stay belongs to a country).
/// </summary>
public interface IStayService 
{
    /// <summary>
    /// Retrieves all stays from the database.
    /// 
    /// General: Gets a list of all accommodations available in the system.
    /// </summary>
    /// <returns>Result containing a list of stays or errors</returns>
    Task<Result<IEnumerable<GetStaysDto>>> GetStays();

    /// <summary>
    /// Retrieves a specific stay by its ID.
    /// 
    /// General: Gets detailed information about a single accommodation.
    /// </summary>
    /// <param name="id">The unique identifier of the stay</param>
    /// <returns>Result containing the stay data or NotFound error</returns>
    Task<Result<GetStayDto>> GetStay(int id);

    /// <summary>
    /// Updates an existing stay.
    /// 
    /// General: Modifies a stay's information (name, price, address, etc.).
    /// </summary>
    /// <param name="id">The ID of the stay to update</param>
    /// <param name="stayDto">The updated stay data</param>
    /// <returns>Result indicating success or failure</returns>
    Task<Result> UpdateStay(int id, UpdateStayDto stayDto);

    /// <summary>
    /// Creates a new stay.
    /// 
    /// General: Adds a new accommodation to the system.
    /// </summary>
    /// <param name="stayDto">The stay data to create</param>
    /// <returns>Result containing the created stay or errors</returns>
    Task<Result<GetStayDto>> CreateStay(CreateStayDto stayDto);

    /// <summary>
    /// Deletes a stay from the system.
    /// 
    /// General: Removes an accommodation from the database.
    /// </summary>
    /// <param name="id">The ID of the stay to delete</param>
    /// <returns>Result indicating success or failure</returns>
    Task<Result> DeleteStay(int id);
}