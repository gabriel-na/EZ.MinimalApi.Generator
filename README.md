# EZ.MinimalApi.Generator

**EZ.MinimalApi.Generator** is a C# Source Generator that automatically maps Minimal API endpoints using attributes.  
It allows developers to organize their API endpoints in clean classes, without manually writing `MapGet`, `MapPost`, etc.

This library focuses on:
- Automatic endpoint registration  
- Inline method-body generation (your method code becomes the request delegate)  
- Support for HTTP verbs, groups, filters, authorization, tags, and route names  
- Zero runtime dependencies (compile-time only)

---

## Where to use?
Some attributes can only be used at class level (`[C]`), method level (`[M]`) or both (`[C|M]`).

## ✨ Features

- `Get`, `Post`, `Put`, `Patch`, `Delete`, `Fallback` attributes for routing  
- `Endpoint` attribute to register endpoint classes  
- Automatic group mapping via `GroupEndpoint`  
- Inline delegate generation (no instance creation required)  
- Supports:
  - Authorization with: policies, IAuthorizeData types, and AuthorizationPolicy
  - Route naming with: Name, Summary, Description and DisplayName
  - Filters
  - Tags
  - Accepts
  - AllowAnonymous
  - Produces / ProducesProblem
  - Cors
  - ExcludeFromDescription
  - Rate Limiting (EnableRateLimiting / DisableRateLimiting)
  - Output Caching (CacheOutput / DisableOutputCache)
  - Request Size Limit (RequestSizeLimit / DisableRequestSizeLimit)
  - Endpoint Group Name
  - SkipStatusCodePages
  - Request Timeout (RequestTimeout / DisableRequestTimeout)
- Fully compile-time, no reflection

---

## 📁 Sample Project

A complete sample project is available at [`samples/EZ.MinimalApi.Sample`](./samples/EZ.MinimalApi.Sample) demonstrating all attributes in a real Minimal API application.

Run it with:
```sh
dotnet run --project samples/EZ.MinimalApi.Sample
```

Tests are in [`tests/EZ.MinimalApi.Generator.Tests`](./tests/EZ.MinimalApi.Generator.Tests) with 9+ test cases covering endpoint generation, attributes, and compilation validation:

```sh
dotnet test tests/EZ.MinimalApi.Generator.Tests
```

## 📦 Installation

Install via NuGet:

```sh
dotnet add package EZ.MinimalApi.Generator
```
Or via Package Manager

```sh
Install-Package EZ.MinimalApi.Generator
```

## 🚀 Getting Started
1 - Mark a `static` class with [Endpoint]

```csharp
namespace MyAwesomeApi.Endpoints;

[Endpoint]
public static class UserEndpoints
{
    [Get]
    public static IResult GetById() =>
        Results.Ok();

    [Post]
    public static IResult CreateUser() =>
        Results.Created();
}
```

2. Call the generated extension method in `Program.cs`

```csharp
var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapCustomEndpoints(); // Generated automatically

app.Run();
```

## Generated Output

The code above will produce:

Aggregator class that wraps all the generated endpoints in one method call.
```csharp
namespace Microsoft.AspNetCore.Builder
{
	public static class WebApplicationAggregatorExtensions
	{
		public static WebApplication MapCustomEndpoints(this WebApplication app)
		{
			EZ.MinimalApi.GeneratedEndpoints.UserEndpointsExtensions.MapUserEndpoints(app);
			
			return app;
		}
	}
}
```

And the class that maps all endpoints.
```csharp
namespace EZ.MinimalApi.GeneratedEndpoints
{
	public static class UserEndpointsExtensions
	{
		public static WebApplication MapUserEndpoints(this WebApplication app)
		{
			app.MapGet("/{id}", MyAwesomeApi.Endpoints.UserEndpoints.GetUserById);

			return app;
		}
	}
}
```

## Grouping<sup>`[C]`</sup>
```csharp
[Endpoint]
[GroupEndpoint("/users")]
public static class UserEndpoints
{
    [Get("/{id}")]
    public static IResult Get(Guid id, IUserRepository repo) => 
        Results.Ok();

    [Post]
    public static IResult Create() => 
        Results.Created();
}
```
Generated:

```csharp
var group = app.MapGroup("/users");

group.MapGet("/{id}", ...);
group.MapPost("/", ...);
```

## Authorization<sup>`[C|M]`</sup>
````csharp
[Get("/admin")]
[Authorization("AdminPolicy")] //By policy name
[Authorization("Admin", "SuperUser")] //Multiple policies
[Authorization(typeof(MyCustomAuthorizeData))] //IAuthorizeData types
[Authorization(typeof(AdminPolicy))] //AuthorizationPolicy type
public static IResult GetAdmin() => 
    Results.Ok();
````

Generated:

```csharp
app.MapGet("/admin", ...)
    .RequireAuthorization("AdminPolicy");
//(or the appropriate overload)
```

## Filters<sup>`[C|M]`</sup>
Filters can be applied in two places:

- Class-level filters → Applied to the group
- Method-level filters → Applied to individual endpoints

```csharp
[Endpoint]
[GroupEndpoint("/users")]
[Filter<GlobalFilter>]      // applied to the group
public static class UserEndpoints
{
    [Get]
    [Filter<EndpointFilter>] // applied only to this endpoint
    public static IResult Get() =>
        Results.Ok();
}
```

Generated:

```csharp
var group = app.MapGroup("/users")
    .AddEndpointFilter<GlobalFilter>();

group.MapGet("/{id}", ...)
    .AddEndpointFilter<EndpointFilter>();
```

## Tags<sup>`[C|M]`</sup>
Can be used at class level or method level
```csharp
[Get]
[Tags("Users", "Read")]
public static IResult Get() =>
    Results.Ok();
```

Generated:

```csharp
app.MapGet("/", ...)
    .WithTags("Users", "Read");
```

## Route Names<sup>`[C|M]`</sup>
Can be used at class level or method level
```csharp
[Get]
[Name("Get user by ID")]
[DisplayName("GetUserById")]
[Description("Get user information by ID")]
[Summary("Gets user information by ID or returns 404 if not found")]
public static IResult Get() =>
    Results.Ok();
```

Generated:

```csharp
app.MapGet("/", ...)
    .WithName("GetUserById")
    .WithDisplayName("GetUserById")
    .WithDescription("Get user information by ID")
    .WithSummary("Gets user information by ID or returns 404 if not found");
```

## Accepts<sup>`[M]`</sup>
Specifies which content-type will be accepted based on the type of request body.
Can be used multiple times.
```csharp
[Get]
[Accpets<User>("application/json")]
//or [Accepts(typeof(User), "application/json")]
public static IResult Get() =>
    Results.Ok();
```

Generated:
```csharp
app.MapGet("/", ...)
    .Accepts<User>("application/json");
```

## AllowAnonymous<sup>`[C|M]`</sup>
Specifies if the group or endpoint can be accessed without authentication/authorization
```csharp
[Get]
[AllowAnonymous]
public static IResult Get() =>
    Results.Ok();
```

Generated:
```csharp
ap.MapGet("/", ...)
    .AllowAnonymous();
```

## Fallback<sup>`[M]`</sup>
Maps a catch-all route for requests that don't match any other endpoint.
When no route is specified, it generates `MapFallback(handler)` without a route parameter (catch-all for any path).
```csharp
[Fallback]                  // catch-all for any path
public static IResult CatchAll() =>
    Results.Ok();

[Fallback("/{**rest}")]     // catch-all with custom pattern
public static IResult CatchAllWithPattern(string rest) =>
    Results.Ok($"Fallback: {rest}");
```

Generated:
```csharp
app.MapFallback(MyNamespace.MyEndpoints.CatchAll);
app.MapFallback("/{**rest}", MyNamespace.MyEndpoints.CatchAllWithPattern);
```

## Produces<sup>`[C|M]`</sup>
Specifies which status code the endpoint/group will return and for which response type.
Can be used  multiple times for diferent status codes.
The contentType and responseType parameters are optionals.
```csharp
[Get]
[Produces<User>(200, "application/json")]
//[Produces(200, typeof(User), "application/json")]
//[Produces(200, typeof(User))]
//[Produces(200)]
public static IResult Get() =>
    Results.Ok();
```

Generated:
```csharp
ap.MapGet("/", ...)
    .Produces<User>(200, "application/json");
    //.Produces(200, "application/json");
    //.Produces(200);
```

## Cors<sup>`[C|M]`</sup>
Specifies one or more cors policies that will be applied to endpoint/group.
If not specified the default policy will be applied.
Can be used multiple times.
```csharp
[Get]
[Cors]                       // default policy
[Cors("MyCorsPolicy")]       // single policy
[Cors("Policy1", "Policy2")] // multiple policies
public static IResult Get() =>
    Results.Ok();
```

Generated:
```csharp
ap.MapGet("/", ...)
    .RequireCors()
    .RequireCors("MyCorsPolicy")
    .RequireCors("Policy1")
    .RequireCors("Policy2");
```

## ExcludeFromDescription<sup>`[C|M]`</sup>
Hides the endpoint or group from OpenAPI/Swagger documentation.
```csharp
[Get]
[ExcludeFromDescription]
public static IResult Get() =>
    Results.Ok();
```

Generated:
```csharp
ap.MapGet("/", ...)
    .ExcludeFromDescription();
```

## ProducesProblem<sup>`[C|M]`</sup>
Specifies that the endpoint returns a ProblemDetails response for the given status code.
Can be used multiple times for different status codes.
```csharp
[Get]
[ProducesProblem(400)]
[ProducesProblem<ValidationError>(422)]
[ProducesProblem(500, typeof(ErrorResponse), "application/json")]
public static IResult Get() =>
    Results.Ok();
```

Generated:
```csharp
ap.MapGet("/", ...)
    .ProducesProblem(400)
    .ProducesProblem<ValidationError>(422)
    .ProducesProblem<ErrorResponse>(500, "application/json");
```

## Rate Limiting<sup>`[C|M]`</sup>
Applies rate limiting policies to endpoints or groups.
```csharp
[EnableRateLimiting("FixedWindow")]  // applies a named policy
[DisableRateLimiting]                // disables rate limiting
public static IResult Get() =>
    Results.Ok();
```

Generated:
```csharp
ap.MapGet("/", ...)
    .RequireRateLimiting("FixedWindow");
// or
ap.MapGet("/", ...)
    .DisableRateLimiting();
```

**Note:** Rate limiting requires the `Microsoft.AspNetCore.RateLimiting` package in your project.

## Output Caching<sup>`[C|M]`</sup>
Caches responses for the endpoint or group.
```csharp
[CacheOutput]                       // default cache profile
[CacheOutput("MyPolicy")]           // named policy
[DisableOutputCache]                // disable caching
public static IResult Get() =>
    Results.Ok();
```

Generated:
```csharp
ap.MapGet("/", ...)
    .CacheOutput()
    .CacheOutput("MyPolicy")
    .DisableOutputCache();
```

**Note:** Output caching requires the `Microsoft.AspNetCore.OutputCaching` package in your project.

## Request Size Limit<sup>`[C|M]`</sup>
Limits the request body size for endpoints or groups.
```csharp
[RequestSizeLimit(30_000_000)]       // 30 MB limit
[DisableRequestSizeLimit]            // no limit
public static IResult Upload() =>
    Results.Ok();
```

Generated:
```csharp
ap.MapGet("/", ...)
    .RequireRequestSizeLimit(30000000)
    .DisableRequestSizeLimit();
```

## Endpoint Group Name<sup>`[C|M]`</sup>
Sets the endpoint group name for OpenAPI/Swagger organization.
Unlike `[GroupEndpoint]`, this does not affect routing — it only tags the endpoint with a group name for documentation purposes.
```csharp
[EndpointGroupName("Users")]
public static IResult Get() =>
    Results.Ok();
```

Generated:
```csharp
ap.MapGet("/", ...)
    .WithGroupName("Users");
```

## SkipStatusCodePages<sup>`[C|M]`</sup>
Skips the status code pages middleware for this endpoint or group.
Useful for API endpoints that return error status codes and don't want them intercepted by error pages.
```csharp
[SkipStatusCodePages]
public static IResult Get() =>
    Results.NotFound();
```

Generated:
```csharp
ap.MapGet("/", ...)
    .SkipStatusCodePages();
```

## Request Timeout<sup>`[C|M]`</sup>
Sets a per-endpoint request timeout. Available in .NET 8+.
```csharp
[RequestTimeout(5000)]                 // 5 seconds (in milliseconds)
[RequestTimeout("MyPolicy")]           // named timeout policy
[DisableRequestTimeout]                // disable timeout
public static IResult Get() =>
    Results.Ok();
```

Generated:
```csharp
ap.MapGet("/", ...)
    .RequireRequestTimeout(TimeSpan.FromMilliseconds(5000))
    .RequireRequestTimeout("MyPolicy")
    .DisableRequestTimeout();
```

**Note:** Request timeout requires .NET 8 or higher.

## 🧪 Requirements

- .NET 7 or higher
- Source generator built with .NET Standard 2.1
- Roslyn 4.x analyzers included automatically

---

## 🛠️ Fixes & Improvements in this version

- **MethodMustBeStaticRule (EZ0003)** — Now correctly validates that endpoint methods are `static`
- **AuthorizationAttribute** — Added `params string[]` constructor for multiple policies: `[Authorization("Admin", "SuperUser")]`
- **Typos fixed**: `AddtionalContentTypes` → `AdditionalContentTypes`, `ProducessAttribute.cs` → `ProducesAttribute.cs`
- **New attributes**: `[Fallback]`, `[ExcludeFromDescription]`, `[ProducesProblem]`, `[EnableRateLimiting]`, `[DisableRateLimiting]`, `[CacheOutput]`, `[DisableOutputCache]`, `[RequestSizeLimit]`, `[DisableRequestSizeLimit]`, `[EndpointGroupName]`, `[SkipStatusCodePages]`, `[RequestTimeout]`, `[DisableRequestTimeout]`
- **CorsAttribute** — Added `params string[]` constructor: `[Cors("P1", "P2")]`