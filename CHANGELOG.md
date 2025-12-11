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