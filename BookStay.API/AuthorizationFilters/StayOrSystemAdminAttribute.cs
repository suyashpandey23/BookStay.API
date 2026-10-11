using BookStay.API.Constants;
using BookStay.API.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace BookStay.API.AuthorizationFilters;

/// <summary>
/// Authorization attribute that allows access to either system administrators
/// or stay-specific administrators.
/// 
/// General: This attribute can be applied to controllers or action methods to ensure
/// that only authorized users can access them. Authorization is granted if the user
/// is a global Administrator OR if they are a StayAdmin for the specific stay.
/// 
/// Technical: Uses TypeFilterAttribute to apply the StayOrSystemAdminFilter.
/// The filter runs during the authorization pipeline before the action executes.
/// 
/// Usage: [StayOrSystemAdmin] on controller or action methods that require
/// stay admin or system admin access.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
public sealed class StayOrSystemAdminAttribute : TypeFilterAttribute
{
    /// <summary>
    /// Constructor that registers the filter type.
    /// 
    /// General: Creates the attribute and tells ASP.NET Core to use
    /// StayOrSystemAdminFilter for authorization.
    /// 
    /// Technical: TypeFilterAttribute allows dependency injection into the filter.
    /// The filter class is instantiated by the DI container.
    /// </summary>
    public StayOrSystemAdminAttribute() : base(typeof(StayOrSystemAdminFilter))
    {
    }
}

/// <summary>
/// Authorization filter that checks if the user is either a system administrator
/// or a stay-specific administrator.
/// 
/// General: This filter implements the authorization logic. It first checks if the user
/// is authenticated, then checks if they are a global Administrator. If not, it checks
/// if they are a StayAdmin for the specific stay specified in the route.
/// 
/// Technical: Implements IAsyncAuthorizationFilter for async authorization checks.
/// Constructor uses primary constructor syntax (C# 12) for dependency injection.
/// 
/// StayListingDBContext: Used to query the StayAdmins table to check if the user
/// is an admin for the specific stay.
/// </summary>
public class StayOrSystemAdminFilter(StayListingDBContext dbContext) : IAsyncAuthorizationFilter
{
    /// <summary>
    /// Performs the authorization check.
    /// 
    /// General: This method is called by ASP.NET Core during the authorization pipeline.
    /// It checks if the user is authorized to access the resource.
    /// 
    /// Technical: If authorization fails, sets context.Result to short-circuit the request
    /// and return an appropriate HTTP status code (401 Unauthorized or 403 Forbidden).
    /// 
    /// Line-by-line:
    /// - var httpUser = context.HttpContext.User: Gets the ClaimsPrincipal representing the current user.
    /// - if (httpUser?.Identity?.IsAuthenticated == false): Checks if the user is not authenticated.
    /// - context.Result = new UnauthorizedResult(): Sets the result to 401 Unauthorized if not authenticated.
    /// - return: Short-circuits the request, preventing further execution.
    /// - if (httpUser!.IsInRole(RoleNames.Administrator)): Checks if user is in the Administrator role.
    /// - return: Allows access immediately if user is a global administrator.
    /// - var userId = httpUser.FindFirst(JwtRegisteredClaimNames.Sub)?.Value: 
    ///   Attempts to get user ID from the JWT "sub" claim (standard JWT subject claim).
    /// - ?? httpUser.FindFirst(ClaimTypes.NameIdentifier)?.Value: 
    ///   Fallback to NameIdentifier claim if Sub claim is not found.
    /// - if (string.IsNullOrWhiteSpace(userId)): Checks if user ID was not found in claims.
    /// - context.Result = new ForbidResult(): Sets result to 403 Forbidden if user ID is missing.
    /// - return: Short-circuits the request.
    /// - context.RouteData.Values.TryGetValue("stayId", out var stayIdObj): 
    ///   Attempts to extract the stayId parameter from the route data.
    /// - int.TryParse(stayIdObj?.ToString(), out int stayId): 
    ///   Parses the stayId value to an integer.
    /// - if (stayId == 0): Checks if parsing failed or stayId is invalid.
    /// - context.Result = new ForbidResult(): Sets result to 403 Forbidden if stayId is invalid.
    /// - return: Short-circuits the request.
    /// - var isStayAdminUser = await dbContext.StayAdmins.AnyAsync(...): 
    ///   Queries the database to check if user is a StayAdmin for this stay.
    /// - q => q.UserId == userId && q.StayId == stayId: Filters for matching user and stay.
    /// - if (!isStayAdminUser): Checks if the user is not a stay admin.
    /// - context.Result = new ForbidResult(): Sets result to 403 Forbidden if not a stay admin.
    /// - return: Short-circuits the request.
    /// </summary>
    /// <param name="context">The authorization filter context</param>
    public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
    {
        var httpUser = context.HttpContext.User;

        // Check if user is authenticated
        if (httpUser?.Identity?.IsAuthenticated == false)
        {
            context.Result = new UnauthorizedResult();
            return;
        }

        // If user is a global Administrator, allow immediately
        if (httpUser!.IsInRole(RoleNames.Administrator))
        {
            return;
        }

        // Get the user ID from JWT claims
        // General: JWT tokens can use different claim names for the subject/user ID.
        // We check both JwtRegisteredClaimNames.Sub and ClaimTypes.NameIdentifier.
        var userId = httpUser.FindFirst(JwtRegisteredClaimNames.Sub)?.Value 
                   ?? httpUser.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrWhiteSpace(userId))
        {
            context.Result = new ForbidResult();
            return;
        }

        // Try to get stayId from route values
        // General: The route parameter name is "stayId" as defined in the controller route.
        context.RouteData.Values.TryGetValue("stayId", out var stayIdObj);
        int.TryParse(stayIdObj?.ToString(), out int stayId);
        
        if (stayId == 0)
        {
            context.Result = new ForbidResult();
            return;
        }

        // Check if user is an admin for this specific stay
        // General: Query the StayAdmins table to see if there's a relationship
        // between this user and this stay.
        var isStayAdminUser = await dbContext.StayAdmins
            .AnyAsync(q => q.UserId == userId && q.StayId == stayId);

        if (!isStayAdminUser)
        {
            context.Result = new ForbidResult();
            return;
        }
    }
}
