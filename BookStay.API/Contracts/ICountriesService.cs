using BookStay.API.DTOs.Country;
using BookStay.API.Results;

namespace BookStay.API.Services;

/// <summary>
/// Interface for country-related business operations.
/// 
/// General: This interface defines the contract for all operations related to countries
/// in the BookStay application. It follows the CRUD pattern (Create, Read, Update, Delete).
/// 
/// Technical: Using an interface allows for dependency injection, loose coupling,
/// and easier testing. We can mock this interface for unit tests without needing
/// a real database. All methods return Result types for explicit error handling.
/// </summary>
public interface ICountriesService 
{
    /// <summary>
    /// Retrieves all countries from the database.
    /// 
    /// General: Gets a list of all countries available in the system.
    /// </summary>
    /// <returns>Result containing a list of countries or errors</returns>
    Task<Result<IEnumerable<GetCountriesDto>>> GetCountries();

    /// <summary>
    /// Retrieves a specific country by its ID.
    /// 
    /// General: Gets detailed information about a single country.
    /// </summary>
    /// <param name="id">The unique identifier of the country</param>
    /// <returns>Result containing the country data or NotFound error</returns>
    Task<Result<GetCountryDto>> GetCountry(int id);

    /// <summary>
    /// Updates an existing country.
    /// 
    /// General: Modifies a country's information (name, code, continent).
    /// </summary>
    /// <param name="id">The ID of the country to update</param>
    /// <param name="countryDto">The updated country data</param>
    /// <returns>Result indicating success or failure</returns>
    Task<Result> UpdateCountry(int id, UpdateCountryDto countryDto);

    /// <summary>
    /// Creates a new country.
    /// 
    /// General: Adds a new country to the system.
    /// </summary>
    /// <param name="countryDto">The country data to create</param>
    /// <returns>Result containing the created country or errors</returns>
    Task<Result<GetCountryDto>> CreateCountry(CreateCountryDto countryDto);

    /// <summary>
    /// Deletes a country from the system.
    /// 
    /// General: Removes a country. This may fail if the country has associated stays.
    /// </summary>
    /// <param name="id">The ID of the country to delete</param>
    /// <returns>Result indicating success or failure</returns>
    Task<Result> DeleteCountry(int id);
}