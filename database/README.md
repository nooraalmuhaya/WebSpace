# Database

WebSpace uses **MySQL**. The data model is defined in [docs/diagrams/webspace_erd_v2.mermaid](../docs/diagrams/webspace_erd_v2.mermaid).

| Folder | Contents |
|--------|----------|
| `schema/` | SQL scripts that create the tables, keys and constraints |
| `seed/` | SQL scripts with development/demo data (roles, sample accounts, sample courses) |

## Status

The database has not been implemented yet. The team still needs to decide how the schema is managed:

- **SQL scripts** in `schema/` and `seed/`, run in order against a fresh MySQL database, or
- **Entity Framework Core migrations** kept in the application project, with `seed/` used for demo data only.

Whichever approach is chosen, document the exact setup steps here once they exist.

## Rules

- Never commit real passwords or production connection strings. Use `src/WebSpace/appsettings.example.json` as the template.
- Do not commit local database dumps (`database/dumps/` and `database/local/` are ignored).
- Seed data must not contain real personal data.
