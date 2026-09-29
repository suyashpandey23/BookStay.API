using BookStay.API.Constants;
using BookStay.API.Data;
using BookStay.API.Extensions;
using BookStay.API.Handlers;
using BookStay.API.MappingProfiles;
using BookStay.API.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

/// <summary>
/// The entry point of the ASP.NET Core application.
/// 
/// General: This file configures and starts the web application. It sets up services,
/// middleware, and the request processing pipeline. In modern .NET (6+), Program.cs
/// combines the functionality of both Program.cs and Startup.cs from earlier versions.
/// 
/// Technical: The application follows the builder pattern:
/// 1. CreateBuilder: Initializes the application with default configuration
/// 2. ConfigureServices (builder.Services): Register dependencies and configure services
/// 3. Build: Creates the WebApplication instance
/// 4. Configure (app): Sets up the HTTP request pipeline (middleware)
/// 5.Run: Starts the application and begins listening for requests
/// </summary>

// Create the WebApplication builder.
// General: This initializes the application with default settings,
// loads configuration from appsettings.json, environment variables, etc.
// Technical: WebApplication.CreateBuilder returns a WebApplicationBuilder
// which provides access to Configuration, Services, Logging, etc.
var builder = WebApplication.CreateBuilder(args);


// Get the database connection string from configuration.
// General: Reads the connection string from appsettings.json or environment variables.
// Technical: builder.Configuration provides access to all configuration sources.
// GetConnectionString looks for a "ConnectionStrings" section in the config.
var connectionString = builder.Configuration.GetConnectionString("StayListingDBContextConnection");

//----------------------------------------------------------------
// Configure the database context.
// General: Set up Entity Framework Core to work with SQL Server database.
// Technical: AddDbContext registers the StayListingDBContext as a scoped service.
// Scoped means a new instance is created for each HTTP request.
// UseSqlServer specifies that we're using SQL Server as the database provider.
builder.Services.AddDbContext<StayListingDBContext>(options =>
{
    options.UseSqlServer(connectionString);
});

// Configure ASP.NET Core Identity for user management.
// General: Adds built-in user authentication and authorization features.
// Technical: AddIdentityApiEndpoints adds minimal API endpoints for user management
// (register, login, logout, etc.). AddEntityFrameworkStores tells Identity to use
// Entity Framework for storing user data in our database.
builder.Services.AddIdentityApiEndpoints<ApplicationUser>().AddEntityFrameworkStores<StayListingDBContext>();

// Configure custom authentication.
// General: Set up our custom Basic Authentication handler.
// Technical: AddAuthentication configures the authentication system.
// DefaultScheme: Specifies which authentication scheme to use by default.
// DefaultChallengeScheme: Which scheme to use when authentication is challenged (401).
// AddScheme registers our custom BasicAuthenticationHandler with the name "Basic".
builder.Services.AddAuthentication(options => {
    options.DefaultScheme = AuthenticationDefaults.BasicScheme;
    options.DefaultChallengeScheme = AuthenticationDefaults.BasicScheme;
}).AddScheme<AuthenticationSchemeOptions, BasicAuthenticationHandler>(AuthenticationDefaults.BasicScheme, _ => { });

// Add authorization services.
// General: Enables authorization features like [Authorize] attribute.
// Technical: This is required to use authorization policies and role-based access control.
builder.Services.AddAuthorization();

// Register application services for dependency injection.
// General: These services can be injected into controllers and other classes.
// Technical: AddScoped means a new instance is created per HTTP request.
// This is the most common lifetime for services that use the database context.
// The interface is registered, so we can depend on the abstraction (IAuthService)
// rather than the implementation (AuthService) - this promotes loose coupling.
builder.Services.AddScoped<ICountriesService, CountriesService>();
builder.Services.AddScoped<IStayService, StayService>();
builder.Services.AddScoped<IAuthService, AuthService>();

// Configure AutoMapper for object mapping.
// General: AutoMapper is a library that maps objects of different types.
// It's commonly used to map database entities to DTOs.
// Technical: AddAutoMapper registers AutoMapper services.
// We add mapping profiles that define how to map between types.
// cfg.AddProfile registers each profile containing mapping configurations.
builder.Services.AddAutoMapper(cfg => {
    cfg.AddProfile<StayMappingProfile>();
    cfg.AddProfile<CountryMappingProfile>();
});
//----------------------------------------------------------------

// Configure MVC controllers.
// General: Add support for traditional MVC controllers.
// Technical: AddControllers registers controller-related services.
// AddJsonOptions configures JSON serialization options.
// ReferenceHandler.IgnoreCycles prevents infinite loops when serializing
// objects with circular references (e.g., Country -> Stays -> Country).
builder.Services.AddControllers().AddJsonOptions(opt => {
    opt.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
});

// // Add services to the container.
// // This is commented out - likely an extension method that would add all services at once.
// // builder.Services.AddBookStayServices();

// Build the application.
// General: Creates the WebApplication instance after all services are configured.
// Technical: This finalizes the service provider and prepares the app to run.
var app = builder.Build();

// Map Identity API endpoints.
// General: Exposes default Identity endpoints for user management.
// Technical: MapIdentityApi adds endpoints like /register, /login, /logout
// under the specified route prefix ("api/defaultauth").
app.MapGroup("api/defaultauth").MapIdentityApi<ApplicationUser>();

// Configure the HTTP request pipeline (middleware).
// General: Sets up the sequence of middleware that processes each HTTP request.
// Technical: UseBookStayMiddleware is a custom extension method that likely
// adds common middleware like authentication, authorization, exception handling, etc.
// Middleware runs in the order they're added.
app.UseBookStayMiddleware();

// Start the application and begin listening for HTTP requests.
// General: This blocks the main thread and starts the web server.
// Technical: Run starts the Kestrel web server and begins accepting requests.
// The application will continue running until it's stopped.
app.Run();