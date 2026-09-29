using BookStay.API.Constants;
using BookStay.API.Data;
using BookStay.API.DTOs.Country;
using BookStay.API.Results;
using Microsoft.EntityFrameworkCore;

namespace BookStay.API.Services;

/// <summary>
/// Implementation of the ICountriesService interface.
/// 
/// General: This service handles all business logic for country operations.
/// It interacts with the database through Entity Framework Core and returns
/// Result objects for consistent error handling.
/// 
/// Technical: Uses primary constructor syntax (C# 12) to inject the database context.
/// The context is stored in a readonly field to ensure it's not reassigned.
/// All methods are async to avoid blocking threads during database operations.
/// </summary>
public class CountriesService(StayListingDBContext context) : ICountriesService 
{
    // Store the context in a readonly field for clarity.
    // General: This makes the code more readable by using _context instead of context.
    // Technical: readonly ensures the reference cannot be changed after construction.
    private readonly StayListingDBContext _context = context;
    
    /// <summary>
    /// Retrieves all countries from the database.
    /// </summary>
    public async Task<Result<IEnumerable<GetCountriesDto>>> GetCountries() 
    {
        try 
        {
            // Query the database for all countries and project to DTOs.
            // General: Get all countries and convert them to a simplified format for the API.
            // Technical: _context.Countries accesses the Countries DbSet (table).
            // Select projects each Country entity to a GetCountriesDto (transformation).
            // ToAsyncList executes the query asynchronously and returns a List.
            var countries = await _context.Countries
                .Select(c => new GetCountriesDto(c.Id, c.Name, c.Code, c.Continent))
                .ToListAsync();
            
            // Return success with the list of countries.
            return Result<IEnumerable<GetCountriesDto>>.Success(countries);
        }
        catch (Exception ex)
        {
            // Catch any database errors and return a failure result.
            // General: If something goes wrong with the database, return an error.
            // Technical: Catching Exception is broad but appropriate here as we're
            // converting any database error to a generic DatabaseError.
            return Result<IEnumerable<GetCountriesDto>>.Failure(new Error(ErrorCodes.DatabaseError, ex.Message));
        }
    }

    /// <summary>
    /// Retrieves a specific country by its ID.
    /// </summary>
    public async Task<Result<GetCountryDto>> GetCountry(int id) 
    {
        // Find the country by ID, including its related stays.
        // General: Get the country and also load its associated stays.
        // Technical: Include loads related entities (eager loading).
        // FirstOrDefaultAsync returns the first match or null if not found.
        var country = await _context.Countries.Include(c => c.Stays).FirstOrDefaultAsync(c => c.Id == id);
        
        // If country doesn't exist, return NotFound.
        if (country == null) 
        {
            return Result<GetCountryDto>.NotFound();
        }
        
        // Return success with the country data.
        // General: Convert the database entity to a DTO and return it.
        // Technical: We map the entity to a DTO to control what data is exposed.
        return Result<GetCountryDto>.Success(new GetCountryDto(country.Id, country.Name, country.Code, country.Continent));
    }

    /// <summary>
    /// Updates an existing country.
    /// </summary>
    public async Task<Result> UpdateCountry(int id, UpdateCountryDto countryDto) 
    {
        // Find the country by ID.
        // General: Look up the country we want to update.
        // Technical: FindAsync is optimized for finding by primary key.
        var country = await _context.Countries.FindAsync(id);
        
        // If country doesn't exist, return NotFound.
        if (country == null) 
        {
            return Result.Failure(new Error(ErrorCodes.NotFound, "Country not found"));
        }

        // Validation: Check if another country with same code exists.
        // General: Prevent duplicate country codes (business rule).
        // Technical: Query for any country with the same code but different ID.
        // This prevents two countries from having the same code.
        var duplicateCountry = await _context.Countries
            .FirstOrDefaultAsync(c => c.Code == countryDto.Code && c.Id != id);
        
        if (duplicateCountry != null) 
        {
            return Result.Failure(new Error(ErrorCodes.BadRequest, $"A country with code '{countryDto.Code}' already exists"));
        }

        // Update the country's properties.
        // General: Copy the new values from the DTO to the entity.
        // Technical: Entity Framework tracks these changes automatically.
        country.Name = countryDto.Name;
        country.Code = countryDto.Code;
        country.Continent = countryDto.Continent;

        // Mark the entity as modified.
        // General: Tell Entity Framework that this entity has changed.
        // Technical: Setting the state to Modified ensures EF Core will update
        // this entity when SaveChangesAsync is called.
        _context.Entry(country).State = EntityState.Modified;

        try 
        {
            // Save changes to the database.
            // General: Persist the changes to the database.
            // Technical: SaveChangesAsync generates and executes the SQL UPDATE statement.
            await _context.SaveChangesAsync();
            return Result.Success();
        }
        catch (DbUpdateConcurrencyException)
        {
            // Handle concurrency conflicts.
            // General: Someone else modified this country while we were working on it.
            // Technical: DbUpdateConcurrencyException is thrown when the row was
            // modified by another process since we read it (optimistic concurrency).
            if (!await CountryExistsAsync(id)) 
            {
                // Country was deleted by someone else.
                return Result.Failure(new Error(ErrorCodes.NotFound, "Country not found"));
            }
            // Country still exists but was modified.
            return Result.Failure(new Error(ErrorCodes.ConcurrencyError, "The country was modified by another user"));
        }
        catch (Exception ex)
        {
            // Handle other database errors.
            return Result.Failure(new Error(ErrorCodes.DatabaseError, ex.Message));
        }
    }

    /// <summary>
    /// Creates a new country.
    /// </summary>
    public async Task<Result<GetCountryDto>> CreateCountry(CreateCountryDto countryDto) 
    {
        // Validation: Check if country with same code already exists.
        // General: Prevent duplicate country codes.
        // Technical: Query for any country with the same code.
        var existingCountry = await _context.Countries.FirstOrDefaultAsync(c => c.Code == countryDto.Code);
        
        if (existingCountry != null) 
        {
            return Result<GetCountryDto>.BadRequest($"A country with code '{countryDto.Code}' already exists");
        }

        try 
        {
            // Create a new Country entity from the DTO.
            // General: Convert the DTO to a database entity.
            // Technical: Object initializer syntax sets the properties.
            var country = new Country 
            {
                Name = countryDto.Name,
                Code = countryDto.Code,
                Continent = countryDto.Continent
            };

            // Add the country to the database context.
            // General: Mark this entity for insertion.
            // Technical: Add adds the entity to the context in the "Added" state.
            _context.Countries.Add(country);
            
            // Save changes to generate the ID.
            // General: Save to get the database-generated ID.
            // Technical: SaveChangesAsync executes the INSERT and populates the Id field.
            await _context.SaveChangesAsync();

            // Return the created country with its new ID.
            return Result<GetCountryDto>.Success(new GetCountryDto(country.Id, country.Name, country.Code, country.Continent));
        }
        catch (Exception ex)
        {
            return Result<GetCountryDto>.Failure(new Error(ErrorCodes.DatabaseError, ex.Message));
        }
    }

    /// <summary>
    /// Deletes a country from the system.
    /// </summary>
    public async Task<Result> DeleteCountry(int id) 
    {
        // Find the country including its stays.
        // General: Get the country and its associated stays.
        // Technical: Include loads the Stays navigation property.
        var country = await _context.Countries.Include(c => c.Stays).FirstOrDefaultAsync(c => c.Id == id);
        
        // If country doesn't exist, return NotFound.
        if (country == null) 
        {
            return Result.Failure(new Error(ErrorCodes.NotFound, "Country not found"));
        }

        // Validation: Cannot delete country that has stays.
        // General: Prevent orphaning stays by deleting their country.
        // Technical: Check if the Stays collection has any elements.
        // Any() is more efficient than Count() for checking existence.
        if (country.Stays != null && country.Stays.Any()) 
        {
            return Result.Failure(new Error(ErrorCodes.BadRequest, "Cannot delete country that has associated stays"));
        }

        try 
        {
            // Remove the country from the database.
            // General: Mark the country for deletion.
            // Technical: Remove marks the entity as "Deleted" in the context.
            _context.Countries.Remove(country);
            
            // Save changes to execute the DELETE.
            await _context.SaveChangesAsync();
            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure(new Error(ErrorCodes.DatabaseError, ex.Message));
        }
    }

    /// <summary>
    /// Helper method to check if a country exists.
    /// </summary>
    /// <param name="id">The country ID to check</param>
    /// <returns>True if the country exists, false otherwise</returns>
    private async Task<bool> CountryExistsAsync(int id) 
    {
        // Check if any country with this ID exists.
        // General: Query to see if the country is still in the database.
        // Technical: AnyAsync is more efficient than Count() > 0 as it stops
        // at the first match rather than counting all matches.
        return await _context.Countries.AnyAsync(e => e.Id == id);
    }
} 