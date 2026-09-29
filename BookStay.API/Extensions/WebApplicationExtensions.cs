namespace BookStay.API.Extensions;

/// <summary>
/// Extension methods for WebApplication to configure middleware.
/// 
/// General: This class provides extension methods that simplify the configuration
/// of the HTTP request pipeline (middleware). Extension methods allow us to add
/// functionality to WebApplication without modifying it.
/// 
/// Technical: This is a static class with extension methods (indicated by 'this'
/// parameter). The middleware pipeline is the sequence of components that process
/// each HTTP request in order. Each middleware can either handle the request or
/// pass it to the next middleware in the pipeline.
/// 
/// Middleware runs in the order they're added, so the order matters significantly.
/// </summary>
public static class WebApplicationExtensions
{
    /// <summary>
    /// Configures the BookStay middleware pipeline.
    /// 
    /// General: This method sets up all the middleware components that process
    /// HTTP requests. It's called in Program.cs after building the application.
    /// 
    /// Technical: 'this WebApplication app' makes this an extension method.
    /// WebApplication represents the configured application with all services registered.
    /// The method returns WebApplication for method chaining.
    /// </summary>
    /// <param name="app">The web application to configure</param>
    /// <returns>The configured web application</returns>
    public static WebApplication UseBookStayMiddleware(this WebApplication app)
    {
        // Configure the HTTP request pipeline
        // General: Set up different middleware based on the environment.
        // Technical: app.Environment provides information about the runtime environment.
        // IsDevelopment() checks if we're running in Development mode.
        if (app.Environment.IsDevelopment())
        {
            // Enable OpenAPI/Swagger UI in development only.
            // General: Expose API documentation UI for developers.
            // Technical: MapOpenApi creates endpoints for the OpenAPI specification.
            // This is typically only enabled in development for security reasons.
            app.MapOpenApi();
        }

        // Redirect HTTP requests to HTTPS.
        // General: Automatically redirect insecure HTTP requests to secure HTTPS.
        // Technical: UseHttpsRedirection adds middleware that redirects HTTP to HTTPS.
        // This is a security best practice to ensure all communications are encrypted.
        app.UseHttpsRedirection();

        // Enable authorization middleware.
        // General: Enforce authorization rules for protected endpoints.
        // Technical: UseAuthorization adds middleware that checks if the user
        // is authorized to access the requested resource. It works with the
        // [Authorize] attribute on controllers or actions.
        app.UseAuthorization();

        // Map controller routes.
        // General: Enable routing for MVC controllers.
        // Technical: MapControllers adds endpoint routing for controllers.
        // This maps HTTP requests to controller actions based on route attributes.
        // This must come after UseAuthorization so authorization is checked before
        // the controller action executes.
        app.MapControllers();

        // Return the configured application for chaining.
        return app;
    }
}
