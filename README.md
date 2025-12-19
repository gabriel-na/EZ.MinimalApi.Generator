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

- `Get`, `Post`, `Put`, `Patch`, `Delete` attributes for routing  
- `Endpoint` attribute to register endpoint classes  
- Automatic group mapping via `GroupEndpoint`  
- Inline delegate generation (no instance creation required)  
- Supports:
  - Authorization with: policies, IAuthorizeData types, and AuthorizationPolicy
  - Route naming with: Name, Summary, Description and DisplayName
  - Filters
  - Tags
  - Accepts
  - AllowAnonynous
- Fully compile-time, no reflection

---

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
Can be used  multiple times for diferent policies.
```csharp
[Get]
[Cors("MyCorsPolicy")]
//[Cors]
public static IResult Get() =>
    Results.Ok();
```

Generated:
```csharp
ap.MapGet("/", ...)
    .RequireCors("MyCorsPolicy");
    //.RequireCors();
```

## 🧪 Requirements

- .NET 7 or higher
- Source generator built with .NET Standard 2.1
- Roslyn 4.x analyzers included automatically