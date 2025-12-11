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
    [Get("/users/{id}")]
    public static async Task<User> GetById(Guid id, IUserRepository repository)
    {
        return await repository.GetByIdAsync(id);
    }

    [Post] // default path is '/'
    public static async Task CreateUser(UserDTO request, IUserRepository repository)
    {
        //ommited...
    } 
}
```

2. Call the generated extension method in `Program.cs`

```csharp
using EZ.MinimalApi.Extensions;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapCustomEndpoints(); // Generated automatically

app.Run();
```

## 🛠️ Generated Output

The code above will produce:

Aggregator class that wraps all the generated endpoints in one method call.
```csharp
namespace EZ.MinimalApi.Extensions
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

## 📂 Grouping Endpoints <sup>`[C]`</sup>
```csharp
[Endpoint]
[GroupEndpoint("/users")]
public class UserEndpoints
{
    [Get("/{id}")]
    public async Task<User> Get(Guid id, IUserRepository repo)
        => await repo.GetByIdAsync(id);

    [Post("/")]
    public async Task<User> Create(CreateUserRequest req, IUserRepository repo)
        => await repo.CreateAsync(req);
}
```
Generated:

```csharp
var group = app.MapGroup("/users");

group.MapGet("/{id}", ...);
group.MapPost("/", ...);
```

## 🔐 Authorization Examples <sup>`[C|M]`</sup>
````csharp
[Get("/admin")]

//By policy name
[Authorization("AdminPolicy")]

//Multiple policies
[Authorization("Admin", "SuperUser")]

//IAuthorizeData types
[Authorization(typeof(MyCustomAuthorizeData))]

//AuthorizationPolicy type
[Authorization(typeof(AdminPolicy))]
public IResult GetAdmin() => Results.Ok();
````

Generated:

```csharp
.RequireAuthorization("AdminPolicy");
//(or the appropriate overload)
```

## 🧩 Filters <sup>`[C|M]`</sup>
Filters can be applied in two places:

- Class-level filters → Applied to the group
- Method-level filters → Applied to individual endpoints

```csharp
[Endpoint]
[GroupEndpoint("/users")]
[Filter<GlobalFilter>]      // applied to the group
public class UserEndpoints
{
    [Get("/{id}")]
    [Filter<EndpointFilter>] // applied only to this endpoint
    public Task<User> GetUser(Guid id, IUserRepository repo)
        => repo.GetUserAsync(id);
}
```

Generated:

```csharp
var group = app.MapGroup("/users")
    .AddEndpointFilter<GlobalFilter>(); // <-- class-level filter

group.MapGet("/{id}", async (Guid id, IUserRepository repo) =>
{
    return await repo.GetUserAsync(id);
})
.AddEndpointFilter<EndpointFilter>(); // <-- method-level filter
```

## 🏷️ Tags <sup>`[C|M]`</sup>
Can be used at class level or method level
```csharp
[Tags("Users", "Read")]
```

Generated:

```csharp
.WithTags("Users", "Read");
```

## 🏷 Route Names <sup>`[C|M]`</sup>
Can be used at class level or method level
```csharp
[Name("Get user by ID")]
[DisplayName("GetUserById")]
[Description("Get user information by ID")]
[Summary("Gets user information by ID or returns 404 if not found")]
```

Generated:

```csharp
.WithName("GetUserById")
.WithDisplayName("GetUserById")
.WithDescription("Get user information by ID")
.WithSummary("Gets user information by ID or returns 404 if not found");
```

## Accepts <sup>`[M]`</sup>
Specifies which content-type will be accepted based on the type of request body.
Can be used multiple times.
```csharp
[Accpets<User>("application/json")]
//or [Accepts(typeof(User), "application/json)]
```

Generated:
```csharp
.Accepts<User>("application/json");
```

## 🧪 Requirements

- .NET 7 or higher
- Source generator built with .NET Standard 2.1
- Roslyn 4.x analyzers included automatically