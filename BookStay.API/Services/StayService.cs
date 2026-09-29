using AutoMapper;
using AutoMapper.QueryableExtensions;
using BookStay.API.Constants;
using BookStay.API.Data;
using BookStay.API.DTOs.Stay;
using BookStay.API.Results;
using Microsoft.EntityFrameworkCore;

namespace BookStay.API.Services;

/// <summary>
/// Implementation of the IStayService interface.
/// 
/// General: This service handles all business logic for stay/accommodation operations.
/// It interacts with the database through Entity Framework Core and uses AutoMapper
/// for object mapping between DTOs and entities.
/// 
/// Technical: Uses primary constructor syntax (C# 12) to inject both the database
/// context and AutoMapper's IMapper. AutoMapper is used to automatically map
/// between DTOs and entities, reducing manual mapping code.
/// </summary>
public class StayService(StayListingDBContext context, IMapper mapper) : IStayService 
{
    // Store dependencies in readonly fields.
    // General: These fields hold the database context and mapper for use in methods.
    // Technical: readonly ensures the references cannot be changed after construction.
    private readonly StayListingDBContext _context = context;
    private readonly IMapper _mapper = mapper;

    /// <summary>
    /// Retrieves all stays from the database.
    /// </summary>
    public async Task<Result<IEnumerable<GetStaysDto>>> GetStays() 
    {
        try 
        {
            // Query the database for all stays and project to DTOs.
            // General: Get all accommodations and convert them to a simplified format.
            // Technical: Similar to CountriesService, we use Select to transform
            // each Stay entity to a GetStaysDto.
            var stays = await _context.Stays
                .Select(s => new GetStaysDto(s.Id, s.Name, s.Address, s.City, s.Rating, s.ImageUrl, s.CountryId))
                .ToListAsync();
            
            return Result<IEnumerable<GetStaysDto>>.Success(stays);
        }
        catch (Exception ex)
        {
            return Result<IEnumerable<GetStaysDto>>.Failure(new Error(ErrorCodes.DatabaseError, ex.Message));
        }
    }

    /// <summary>
    /// Retrieves a specific stay by its ID.
    /// </summary>
    public async Task<Result<GetStayDto>> GetStay(int id) 
    {
        // Query the stay including its related country, using AutoMapper projection.
        // General: Get the stay and its country information, mapped to a DTO.
        // Technical: Include loads the Country navigation property (eager loading).
        // ProjectTo is an AutoMapper extension that automatically maps the entity
        // to the DTO based on the mapping profile configuration.
        // This is more efficient than manual mapping as it generates optimized SQL.
        var stay = await _context.Stays
            .Include(h => h.Country)
            .ProjectTo<GetStayDto>(_mapper.ConfigurationProvider)
            .Where(h => h.Id == id)
            .FirstOrDefaultAsync();

        if (stay == null) 
        {
            return Result<GetStayDto>.NotFound();
        }
        return Result<GetStayDto>.Success(stay);
    }

    /// <summary>
    /// Updates an existing stay.
    /// </summary>
    public async Task<Result> UpdateStay(int id, UpdateStayDto stayDto) 
    {
        // Find the stay by ID.
        var stay = await _context.Stays.FindAsync(id);
        
        if (stay == null) 
        {
            return Result.Failure(new Error(ErrorCodes.NotFound, "Stay not found"));
        }

        // Validation: Check if the referenced country exists.
        // General: Ensure we're not assigning a stay to a non-existent country.
        // Technical: AnyAsync checks if any country with the specified ID exists.
        // This maintains referential integrity at the application level.
        var countryExists = await _context.Countries.AnyAsync(c => c.Id == stayDto.CountryId);
        
        if (!countryExists) 
        {
            return Result.Failure(new Error(ErrorCodes.BadRequest, "Country not found"));
        }

        // Update the stay's properties.
        // General: Copy the new values from the DTO to the entity.
        // Technical: Entity Framework automatically tracks these property changes.
        stay.Name = stayDto.Name;
        stay.Address = stayDto.Address;
        stay.City = stayDto.City;
        stay.Rating = stayDto.Rating;
        stay.ImageUrl = stayDto.ImageUrl;
        stay.CountryId = stayDto.CountryId;

        // Mark the entity as modified.
        _context.Entry(stay).State = EntityState.Modified;

        try 
        {
            await _context.SaveChangesAsync();
            return Result.Success();
        }
        catch (DbUpdateConcurrencyException)
        {
            // Handle concurrency conflicts.
            if (!await StayExistsAsync(id)) 
            {
                return Result.Failure(new Error(ErrorCodes.NotFound, "Stay not found"));
            }
            return Result.Failure(new Error(ErrorCodes.ConcurrencyError, "The stay was modified by another user"));
        }
        catch (Exception ex)
        {
            return Result.Failure(new Error(ErrorCodes.DatabaseError, ex.Message));
        }
    }

    /// <summary>
    /// Creates a new stay.
    /// </summary>
    public async Task<Result<GetStayDto>> CreateStay(CreateStayDto stayDto) 
    {
        // Validation: Check if the referenced country exists.
        // General: Ensure the country we're assigning the stay to exists.
        var countryExists = await _context.Countries.AnyAsync(c => c.Id == stayDto.CountryId);
        
        if (!countryExists) 
        {
            return Result<GetStayDto>.BadRequest("Country not found");
        }

        try 
        {
            // Use AutoMapper to map DTO to entity.
            // General: Automatically convert the DTO to a database entity.
            // Technical: Map<Stay>(stayDto) uses AutoMapper's configuration
            // to copy matching properties from the DTO to the entity.
            // This eliminates manual property assignment.
            var stay = _mapper.Map<Stay>(stayDto);

            // Add the stay to the database context.
            _context.Stays.Add(stay);
            
            // Save changes to generate the ID.
            await _context.SaveChangesAsync();
            
            // Map the entity back to DTO for the response.
            // General: Convert the created entity (with its new ID) back to a DTO.
            // Technical: This ensures the response includes the database-generated ID.
            var returnObj = _mapper.Map<GetStayDto>(stay);
            return Result<GetStayDto>.Success(returnObj);
        }
        catch (Exception ex)
        {
            return Result<GetStayDto>.Failure(new Error(ErrorCodes.DatabaseError, ex.Message));
        }
    }

    /// <summary>
    /// Deletes a stay from the system.
    /// </summary>
    public async Task<Result> DeleteStay(int id) 
    {
        // Find the stay by ID.
        var stay = await _context.Stays.FindAsync(id);
        
        if (stay == null) 
        {
            return Result.Failure(new Error(ErrorCodes.NotFound, "Stay not found"));
        }

        try 
        {
            // Remove the stay from the database.
            _context.Stays.Remove(stay);
            
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
    /// Helper method to check if a stay exists.
    /// </summary>
    /// <param name="id">The stay ID to check</param>
    /// <returns>True if the stay exists, false otherwise</returns>
    private async Task<bool> StayExistsAsync(int id) 
    {
        // Check if any stay with this ID exists.
        // Technical: AnyAsync is efficient for existence checks.
        return await _context.Stays.AnyAsync(e => e.Id == id);
    }
}