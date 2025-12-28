---
Title: Use Entity Framework Core as ORM and migration tool
Date: 2025-12-28
Status: Accepted
---

## Context

The application is built on .NET 10. We need an ORM to map domain entities to the relational database, provide developer productivity (LINQ), and a repeatable migration system for schema evolution.

## Decision

Adopt Entity Framework Core (EF Core) as the primary ORM and use EF Core migrations for schema management. Use the Npgsql EF Core provider when targeting PostgreSQL.

## Rationale

- EF Core integrates tightly with .NET, DI, logging and has broad community and Microsoft support.
- EF Core migrations provide a standard, repeatable way to evolve schema alongside code.
- Provider support (Npgsql) offers mapping for PostgreSQL-specific types and recent support for `DateOnly`/`TimeOnly`.

## Alternatives considered

- Dapper or hand-written SQL — simpler mapping and better performance in some scenarios, but more manual migration and mapping work.
- NHibernate — powerful but heavier and less mainstream in new .NET projects.

## Consequences

- Add package `Microsoft.EntityFrameworkCore` and provider-specific package (`Npgsql.EntityFrameworkCore.PostgreSQL`).
- Implement `DbContext` and configure migrations to be part of CI/deployment workflow.
- Consider light-weight repositories or query objects to keep EF usage testable and maintainable.

## Implementation notes

- Map `DateOnly` and `TimeOnly` either via provider support or small value converters.
- Keep migrations in source control and apply them from CI or during deployment in a migration-only step.
- Provide sample `DbContext` and a `docker-compose.yml` for local integration tests.

---
