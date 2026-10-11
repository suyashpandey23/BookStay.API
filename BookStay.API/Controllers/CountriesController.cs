using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using BookStay.API.DTOs.Country;
using BookStay.API.Results;
using BookStay.API.Services;
using BookStay.API.Constants;
using Microsoft.AspNetCore.Authorization;

namespace BookStay.API.Controllers
{
    /// <summary>
    /// Controller for country-related HTTP endpoints.
    /// 
    /// General: This controller exposes REST API endpoints for managing countries.
    /// It follows REST conventions: GET for reading, POST for creating,
    /// PUT for updating, DELETE for removing.
    /// 
    /// Technical: Inherits from BaseApiController which provides the ToActionResult
    /// helper method for consistent error handling. Uses dependency injection to get
    /// the ICountriesService for business logic.
    /// 
    /// Attributes:
    /// - [Route]: Defines the URL pattern as "api/Countries"
    /// - [ApiController]: Enables automatic model validation and other API features
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CountriesController : BaseApiController
    {
        // Service for country business logic.
        // General: This service handles all the actual work for country operations.
        // Technical: readonly ensures the reference cannot be changed after injection.
        private readonly ICountriesService _countriesService;

        /// <summary>
        /// Constructor with dependency injection.
        /// 
        /// General: ASP.NET Core automatically provides the ICountriesService implementation.
        /// Technical: Constructor injection is the recommended way to inject dependencies.
        /// </summary>
        /// <param name="countriesService">The countries service implementation</param>
        public CountriesController(ICountriesService countriesService)
        {
            _countriesService = countriesService;
        }

        // GET: api/Countries
        /// <summary>
        /// Gets all countries.
        /// 
        /// General: Returns a list of all countries in the system.
        /// 
        /// Technical: [HttpGet] attribute maps HTTP GET requests to this method.
        /// The URL will be /api/Countries.
        /// </summary>
        /// <returns>200 OK with list of countries or error status</returns>
        [HttpGet]
  
        public async Task<ActionResult<IEnumerable<GetCountriesDto>>> GetCountries()
        {
            // Call the service to get all countries.
            var result = await _countriesService.GetCountries();
            // Convert Result to ActionResult using base class helper.
            return ToActionResult(result);
        }

        // GET: api/Countries/5
        /// <summary>
        /// Gets a specific country by ID.
        /// 
        /// General: Returns details of a single country.
        /// 
        /// Technical: [HttpGet("{id}")] maps GET requests with an ID parameter.
        /// The URL will be /api/Countries/5 for country with ID 5.
        /// The {id} in the route is bound to the id parameter.
        /// </summary>
        /// <param name="id">The country ID</param>
        /// <returns>200 OK with country data, 404 if not found</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<GetCountryDto>> GetCountry(int id)
        {
            var result = await _countriesService.GetCountry(id);
            return ToActionResult(result);
        }

        // PUT: api/Countries/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        /// <summary>
        /// Updates an existing country.
        /// 
        /// General: Modifies a country's information.
        /// 
        /// Technical: [HttpPut("{id}")] maps PUT requests to this method.
        /// PUT is the standard HTTP method for updating resources.
        /// The comment about overposting refers to a security concern where
        /// clients might send more data than intended. DTOs help mitigate this.
        /// </summary>
        /// <param name="id">The country ID to update</param>
        /// <param name="countryDto">The updated country data</param>
        /// <returns>204 No Content on success, error status on failure</returns>
        [HttpPut("{id}")]
        [Authorize(Roles = RoleNames.Administrator)]
        public async Task<IActionResult> PutCountry(int id, UpdateCountryDto countryDto)
        {
            var result = await _countriesService.UpdateCountry(id, countryDto);
            return ToActionResult(result);
        }

        // POST: api/Countries
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        /// <summary>
        /// Creates a new country.
        /// 
        /// General: Adds a new country to the system.
        /// 
        /// Technical: [HttpPost] maps POST requests to this method.
        /// POST is the standard HTTP method for creating resources.
        /// On success, returns 201 Created with a Location header pointing
        /// to the GET endpoint for the new resource.
        /// </summary>
        /// <param name="countryDto">The country data to create</param>
        /// <returns>201 Created with Location header on success, error status on failure</returns>
        [HttpPost]
        [Authorize(Roles = RoleNames.Administrator)]
        public async Task<ActionResult<GetCountryDto>> PostCountry(CreateCountryDto countryDto)
        {
            var result = await _countriesService.CreateCountry(countryDto);
            
            // Handle failure case.
            if (!result.IsSuccess)
            {
                // Cast to ActionResult<GetCountryDto> to match return type.
                return (ActionResult<GetCountryDto>)MapErrorsToResponse(result.Errors);
            }
            
            // Return 201 Created with Location header.
            // General: Tell the client where to find the new resource.
            // Technical: CreatedAtAction returns 201 status with a Location header.
            // "GetCountry" is the name of the action to reference in the URL.
            // new { id = result.Value.Id } provides the route parameters.
            // result.Value is the created country data.
            return CreatedAtAction("GetCountry", new { id = result.Value.Id }, result.Value);
        }

        // DELETE: api/Countries/5
        /// <summary>
        /// Deletes a country.
        /// 
        /// General: Removes a country from the system.
        /// 
        /// Technical: [HttpDelete("{id}")] maps DELETE requests to this method.
        /// DELETE is the standard HTTP method for removing resources.
        /// </summary>
        /// <param name="id">The country ID to delete</param>
        /// <returns>204 No Content on success, error status on failure</returns>
        ///
        
        [HttpDelete("{id}")]
        [Authorize(Roles = RoleNames.Administrator)]
        public async Task<IActionResult> DeleteCountry(int id)
        {
            var result = await _countriesService.DeleteCountry(id);
            return ToActionResult(result);
        }
    }
}
