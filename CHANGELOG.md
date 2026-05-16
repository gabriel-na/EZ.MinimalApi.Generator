# 🚀 v1.3.0

**[New]**
- `FallbackAttribute`: Catch-all route via `MapFallback(handler)` (no route parameter when default)
- `ExcludeFromDescriptionAttribute`: Hides endpoint from OpenAPI/Swagger
- `ProducesProblemAttribute`: ProblemDetails response configuration
- `EnableRateLimitingAttribute` / `DisableRateLimitingAttribute`: Rate limiting support
- `CacheOutputAttribute` / `DisableOutputCacheAttribute`: Output caching support
- `RequestSizeLimitAttribute` / `DisableRequestSizeLimitAttribute`: Request body size limits
- `EndpointGroupNameAttribute`: Sets OpenAPI group name without affecting routing
- `SkipStatusCodePagesAttribute`: Skips status code pages middleware per endpoint
- `RequestTimeoutAttribute` / `DisableRequestTimeoutAttribute`: Per-endpoint request timeouts (.NET 8+)
- Sample project at `samples/EZ.MinimalApi.Sample` with all attributes in use
- Test project at `tests/EZ.MinimalApi.Generator.Tests` with 9+ tests

**[Fix]**
- **EZ0003** (`MethodMustBeStaticRule`): Now correctly validates that endpoint methods are `static` (was defined but never checked)
- `AuthorizationAttribute`: Added `params string[]` constructor for multiple policies (`[Authorization("Admin", "SuperUser")]`)
- `CorsAttribute`: Replaced single-policy constructor with `params string[]` (`[Cors("P1", "P2")]`)
- `AcceptsAttribute.AddtionalContentTypes`: Fixed typo → `AdditionalContentTypes`
- `ProducessAttribute.cs`: Renamed to `ProducesAttribute.cs`

**[Changed]**
- Generated files now conditionally add `using Microsoft.AspNetCore.RateLimiting;` when rate limiting attributes are used
- Generated files now conditionally add `using Microsoft.AspNetCore.OutputCaching;` when output caching attributes are used
- Generated files now conditionally add `using Microsoft.AspNetCore.Http.Timeouts;` when request timeout attributes are used

## ⬇️ How to update

```bash
dotnet add package EZ.MinimalApi --version 1.3.0
```

# 🚀 v1.2.0

**[New]** 
- `ProducesAttribute`: Specifies which status code the endpoint/group will return and for which response type.
- `CorsAttribute`: Specifies one or more cors policies that will be applied to endpoint/group

- Diagnostic errors and warnings. 

    ### Errors
    - EZ001: The class '`class name`' that contains [Endpoint] must be declared as 'static'

    - EZ004: The method '`method name`' must have a block body or be an expression body

    - EZ007: The method '`method name`' must have only one HTTP method attribute

    ### Warnings
    - EZ002: The class '`class name`' does not contain any annotated methods and will be ignored

More validations will be avaible in future releases.

**[Fix]**
- Corrected README.md with missing attributes or wrong code

**[Changed]**
- namespace `EZ.MinimalApi.Extensions` is no longer required in `Program.cs`;

## ⬇️ How to update

```bash
dotnet add package EZ.MinimalApi --version 1.2.0
```

# 🚀 v1.1.0

**[New]** 

- `[Accepts(Type requestType, string contentType)]` or `[Accepts<TRequestType>(string contentType)]`
- `[AllowAnonymous]`
- `[DisplayName]`
- `[Description]`
- `[Summary]`

**[Fix]**
- `[Name]`: Now can be used at class level
- `[Tags]`: Now can be used at class level

**[Changed]**
- `[Get|Post|Put|Delete|Patch]`: **'/'** is the default value if no url pattern is supplied
- Other minor code improvements;

## ⬇️ How to update

```bash
dotnet add package EZ.MinimalApi --version 1.1.0
```