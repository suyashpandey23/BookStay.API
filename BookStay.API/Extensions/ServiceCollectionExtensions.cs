using BookStay.API.Data;
using Microsoft.Extensions.DependencyInjection;

namespace BookStay.API.Extensions;

/// <summary>
/// Extension methods for IServiceCollection to simplify service registration.
/// 
/// General: This class provides extension methods that make it easier to register
/// services in the dependency injection container. Extension methods allow us to
/// add functionality to existing types without modifying them.
/// 
/// Technical: This is a static class with extension methods (indicated by 'this'
/// parameter). Extension methods are called as if they were instance methods on
/// the extended type. For example: services.AddBookStayServices().
/// 
/// This pattern is commonly used in ASP.NET Core to organize service registration
/// into logical groups, making Program.cs cleaner and more readable.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers all BookStay application services.
    /// 
    /// General: This method registers repositories, controllers, and other services
   /// needed by the application. It groups related service registrations together.
    /// 
    /// Technical: 'this IServiceCollection services' makes this an extension method.
    /// IServiceCollection is the interface used to register services in ASP.NET Core's
    /// dependency injection container. The method returns IServiceCollection to enable
    /// method chaining (fluent API pattern).
    /// 
    /// Note: This method is currently commented out in Program.cs, suggesting it may
    /// be legacy code or replaced by individual service registrations.
    /// </summary>
    /// <param name="services">The service collection to add services to</param>
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection AddBookStayServices(this IServiceCollection services)
    {
        // Register application services
        // General: Register the country repository with dependency injection.
        // Technical: AddScoped registers ICountryRepository with InMemoryCountryRepository.
        // Scoped lifetime means a new instance is created per HTTP request.
        // This is using an in-memory repository (likely for testing or development).
        services.AddScoped<ICountryRepository, InMemoryCountryRepository>();

        // Register controllers
        // General: Add MVC controllers to the application.
        // Technical: AddControllers registers controller-related services and
        // enables controller discovery and routing.
        services.AddControllers();

        // Register OpenAPI
        // General: Add OpenAPI/Swagger support for API documentation.
        // Technical: AddOpenApi registers services for generating OpenAPI documentation.
        // This provides a UI for exploring and testing the API endpoints.
        services.AddOpenApi();

        // Return the service collection for method chaining.
        return services;
    }
}
