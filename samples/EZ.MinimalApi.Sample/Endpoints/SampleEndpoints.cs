using EZ.MinimalApi.Attributes;

namespace EZ.MinimalApi.Sample.Endpoints;

[Endpoint]
[GroupEndpoint("/api/sample")]
[EZ.MinimalApi.Attributes.Tags("Sample")]
public static class SampleEndpoints
{
    [Get]
    [Summary("Gets all items")]
    [Produces<User>(200)]
    public static IResult GetAll() =>
        Results.Ok(new[] { new User("John", "john@email.com") });

    [Get("/{id}")]
    [DisplayName("GetUserById")]
    [Produces<User>(200)]
    [ProducesProblem(404)]
    public static IResult GetById(int id) =>
        id > 0 ? Results.Ok(new User("John", "john@email.com")) : Results.NotFound();

    [Post]
    [AllowAnonymous]
    [Accepts<User>("application/json")]
    [Produces<User>(201)]
    public static IResult Create(User user) =>
        Results.Created($"/api/sample/1", user);

    [Put("/{id}")]
    [Authorization("AdminPolicy")]
    public static IResult Update(int id, User user) =>
        Results.NoContent();

    [Delete("/{id}")]
    [Authorization("Admin", "SuperUser")]
    [Filter<RequestValidationFilter>]
    public static IResult Delete(int id) =>
        Results.NoContent();

    [Patch("/{id}")]
    [Cors("AdminCorsPolicy")]
    [EZ.MinimalApi.Attributes.ExcludeFromDescription]
    public static IResult Patch(int id, User user) =>
        Results.NoContent();

    [Fallback]
    public static IResult CatchAll() =>
        Results.NotFound();

    [Fallback("/{**rest}")]
    public static IResult CatchAllWithPattern(string rest) =>
        Results.NotFound();
}

[Endpoint]
[EZ.MinimalApi.Attributes.Tags("Health")]
[EZ.MinimalApi.Attributes.ExcludeFromDescription]
public static class HealthEndpoints
{
    [Get("/health")]
    [AllowAnonymous]
    public static IResult Health() =>
        Results.Ok(new { status = "healthy" });
}

public record User(string Name, string Email);

public class RequestValidationFilter : IEndpointFilter
{
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        return await next(context);
    }
}
