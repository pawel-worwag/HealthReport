Preference: Primary Constructors in C#
=====================================

Summary
-------
In this repository we prefer using C# primary constructors (constructor parameters declared directly on the class declaration) for simple service/application classes (for example handlers) instead of manually declaring private fields and a separate constructor.

Why
---
- Less boilerplate — classes are more concise and readable.
- A clear, consistent convention for DI-style code (handlers and services).
- The codebase already contains examples of this style (for example `UsersListHandler`).

Example — before (traditional constructor)

```csharp
public class UsersListHandler : IUsersListHandler
{
    private readonly UserManager<ApplicationUser> _userManager;

    public UsersListHandler(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<ICollection<UserDto>> ListAsync(CancellationToken cancellationToken = default)
    {
        return await _userManager.Users
            .OrderBy(u => u.Email)
            .Select(...)
            .ToArrayAsync(cancellationToken);
    }
}
```

After (primary constructor)

```csharp
public class UsersListHandler(UserManager<ApplicationUser> userManager) : IUsersListHandler
{
    public async Task<ICollection<UserDto>> ListAsync(CancellationToken cancellationToken = default)
    {
        return await userManager.Users
            .OrderBy(u => u.Email)
            .Select(...)
            .ToArrayAsync(cancellationToken);
    }
}
```

Guidelines and best practices
-----------------------------
- Use primary constructors for simple classes, especially handlers and services where constructor parameters are primarily DI dependencies.
- Use camelCase for parameter names (for example `userManager`). Do not prefix constructor parameters with `_` — if you need a backing field, declare and assign it explicitly.
- If a class requires multiple constructors, complex initialization, parameter validation, or must be usable by frameworks that require a parameterless constructor (for example some serializers or reflection-based libraries), prefer the traditional constructor form.
- Primary constructors are a stylistic convention — DI registration is unchanged (register the concrete class as usual).

Exceptions
----------
- EF Core entities and domain types that require a parameterless constructor or special initialization logic — avoid primary constructors when they conflict with framework requirements.
- If you need to expose a dependency as a public property or field, consider explicitly defining the property/field and assigning it in the constructor.

Questions / concerns
-------------------
If you encounter tooling or IDE compatibility issues, raise them in a PR or issue and we can add specific exceptions or guidance for the particular case.
