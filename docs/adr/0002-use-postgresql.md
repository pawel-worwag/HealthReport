---
Title: Use PostgreSQL as primary database
Date: 2025-12-28
Status: Accepted
---

## Context

HealthReport requires a production-ready relational database for storing measurements, users and application metadata. The system targets .NET 10 and will run in containerized and cloud environments. We need a database with good tooling and reliable migration/backup capabilities.

## Decision

Use PostgreSQL as the primary production database.

## Rationale

- PostgreSQL is mature, open-source, and widely supported on major cloud providers and container platforms.
- Advanced features available when needed: `JSONB`, arrays, extensions (e.g. `pg_trgm`), robust SQL compliance.
- Strong tooling ecosystem for backups, monitoring and high-availability (replication, WAL shipping).
- Good performance and stability for expected workloads.

## Alternatives considered

- SQLite — good for local development and tests, but not suitable for production concurrency and scale.
- MySQL/MariaDB — viable but trade-offs around SQL dialect and some advanced features.
- MS SQL Server — powerful but heavier operationally and less common on Linux containers.

## Consequences

- Add PostgreSQL to CI and deployment plans (Docker Compose for local dev, managed instances for production).
- Choose client libraries and providers that work well with .NET 10.
- Maintain backup and restore procedures (e.g. `pg_dump`, snapshots) and a plan for applying patches.

## Implementation notes

- Recommended PostgreSQL versions: 14+ or 15+ depending on provider.
- Example connection string:

```
Host=localhost;Port=5432;Database=healthreport;Username=hr_user;Password=secret
```

---
