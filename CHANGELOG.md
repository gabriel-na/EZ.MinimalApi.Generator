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