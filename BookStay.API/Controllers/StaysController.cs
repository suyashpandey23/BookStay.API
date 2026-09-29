using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using BookStay.API.DTOs.Stay;
using BookStay.API.Results;
using BookStay.API.Services;

namespace BookStay.API.Controllers
{
    /// <summary>
    /// Controller for stay/accommodation-related HTTP endpoints.
    /// 
    /// General: This controller exposes REST API endpoints for managing stays (accommodations).
    /// It follows the same REST conventions as CountriesController.
    /// 
    /// Technical: Inherits from BaseApiController for consistent error handling.
    /// Uses dependency injection to get IStayService for business logic.
    /// The structure mirrors CountriesController as they both implement CRUD operations.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class StaysController : BaseApiController
    {
        // Service for stay business logic.
        private readonly IStayService _stayService;

        /// <summary>
        /// Constructor with dependency injection.
        /// </summary>
        /// <param name="stayService">The stay service implementation</param>
        public StaysController(IStayService stayService)
        {
            _stayService = stayService;
        }

        // GET: api/Stays
        /// <summary>
        /// Gets all stays.
        /// 
        /// General: Returns a list of all accommodations in the system.
        /// 
        /// Technical: [HttpGet] maps GET requests to /api/Stays.
        /// </summary>
        /// <returns>200 OK with list of stays or error status</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<GetStaysDto>>> GetStays()
        {
            var result = await _stayService.GetStays();
            return ToActionResult(result);
        }

        // GET: api/Stays/5
        /// <summary>
        /// Gets a specific stay by ID.
        /// 
        /// General: Returns details of a single accommodation.
        /// 
        /// Technical: [HttpGet("{id}")] maps GET requests to /api/Stays/{id}.
        /// </summary>
        /// <param name="id">The stay ID</param>
        /// <returns>200 OK with stay data, 404 if not found</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<GetStayDto>> GetStay(int id)
        {
            var result = await _stayService.GetStay(id);
            return ToActionResult(result);
        }

        // PUT: api/Stays/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        /// <summary>
        /// Updates an existing stay.
        /// 
        /// General: Modifies an accommodation's information.
        /// 
        /// Technical: [HttpPut("{id}")] maps PUT requests to /api/Stays/{id}.
        /// </summary>
        /// <param name="id">The stay ID to update</param>
        /// <param name="stayDto">The updated stay data</param>
        /// <returns>204 No Content on success, error status on failure</returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> PutStay(int id, UpdateStayDto stayDto)
        {
            var result = await _stayService.UpdateStay(id, stayDto);
            return ToActionResult(result);
        }

        // POST: api/Stays
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        /// <summary>
        /// Creates a new stay.
        /// 
        /// General: Adds a new accommodation to the system.
        /// 
        /// Technical: [HttpPost] maps POST requests to /api/Stays.
        /// Returns 201 Created with Location header on success.
        /// </summary>
        /// <param name="stayDto">The stay data to create</param>
        /// <returns>201 Created with Location header on success, error status on failure</returns>
        [HttpPost]
        public async Task<ActionResult<GetStayDto>> PostStay(CreateStayDto stayDto)
        {
            var result = await _stayService.CreateStay(stayDto);
            
            // Handle failure case.
            if (!result.IsSuccess)
            {
                return (ActionResult<GetStayDto>)MapErrorsToResponse(result.Errors);
            }
            
            // Return 201 Created with Location header.
            // General: Tell the client where to find the new resource.
            // Technical: Same pattern as CountriesController.
            return CreatedAtAction("GetStay", new { id = result.Value.Id }, result.Value);
        }

        // DELETE: api/Stays/5
        /// <summary>
        /// Deletes a stay.
        /// 
        /// General: Removes an accommodation from the system.
        /// 
        /// Technical: [HttpDelete("{id}")] maps DELETE requests to /api/Stays/{id}.
        /// </summary>
        /// <param name="id">The stay ID to delete</param>
        /// <returns>204 No Content on success, error status on failure</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteStay(int id)
        {
            var result = await _stayService.DeleteStay(id);
            return ToActionResult(result);
        }
    }
}
