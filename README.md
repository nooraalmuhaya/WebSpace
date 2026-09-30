# WebSpace

WebSpace is an interactive programming learning platform where students follow roadmaps, study weekly lessons, practise with coding challenges, take quizzes and receive tutor feedback on their assignments.

## Project Overview

WebSpace supports three roles:

- **Students** enrol in courses, work through weekly lessons and resources, attempt coding challenges and quizzes, submit assignments and take part in course discussions.
- **Tutors** review the submissions for the courses they are assigned to and give feedback.
- **Administrators** manage users, roadmaps, courses and all learning content.

The system is built as **one ASP.NET Core MVC application with a single MySQL database**. The five team workstreams are modules of this one application, not separate systems.

## Features

| Area | Status |
|------|--------|
| Authentication and authorization | Planned |
| User profiles | Planned |
| Student learning | Planned |
| Courses and lessons | Planned |
| Learning resources | Planned |
| Coding challenges | Planned |
| Quizzes and assessments | Planned |
| Assignments | Planned |
| Tutor review and feedback | Planned |
| Discussions | Planned |
| Administrator content management | Planned |
| Progress tracking | Planned |

Status values: **Planned** → **In Development** → **Completed**. Update this table as features move through development.

## Technology Stack

- **ASP.NET Core MVC** (C#): web application
- **MySQL**: relational database
- **GitHub Actions**: continuous integration

Libraries (e.g. the MySQL provider, the test framework) will be listed here once they are added to the solution.

## Project Structure

```
WebSpace/
├── src/WebSpace/        ASP.NET Core MVC application (single project for all modules)
├── tests/               Automated test projects
├── database/
│   ├── schema/          Schema scripts
│   └── seed/            Development/demo seed data
├── docs/
│   ├── requirements/    Requirements and backlog
│   ├── design/          Architecture and UI design
│   ├── diagrams/        ERD and other diagram sources
│   ├── testing/         Test plan and results
│   └── user-guide/      End-user documentation
└── .github/
    ├── workflows/       CI pipeline
    ├── ISSUE_TEMPLATE/  Issue forms
    └── pull_request_template.md
```

### Workstreams

| # | Workstream | Main branch |
|---|------------|-------------|
| 1 | Authentication + Profile | `feature/authentication` |
| 2 | Student Learning | `feature/student-learning` |
| 3 | Quiz + Assessment | `feature/assessment` |
| 4 | Tutor + Submission/Feedback | `feature/tutor` |
| 5 | Admin + Content Management | `feature/admin` |

All workstreams share the same application project, database and authentication. Shared code (layout, database context, entity models) should be changed through small, separately reviewed pull requests so that it doesn't block other workstreams.

## Team Development

### Branch strategy

| Branch | Purpose |
|--------|---------|
| `main` | Stable, tested, submission-ready code. No direct commits. |
| `develop` | Integration branch. All feature work is merged here through pull requests. |
| `feature/<short-description>` | New features, branched from `develop` |
| `fix/<short-description>` | Bug fixes, branched from `develop` |
| `docs/<short-description>` | Documentation changes |
| `refactor/<short-description>` | Code restructuring without behaviour change |
| `test/<short-description>` | Adding or improving tests |

`develop` is merged into `main` only when it is stable and all tests pass.

### Commit messages

Use the [Conventional Commits](https://www.conventionalcommits.org/) style:

```
feat: add student course dashboard
fix: validate assignment submission
docs: update database documentation
refactor: improve course service structure
test: add authentication tests
chore: update dependencies
```

Avoid vague messages such as "update", "changes" or "final".

## Running the Project

The ASP.NET Core solution has not been created yet. Setup and run instructions will be added here once the solution exists and has been verified.

Configuration will follow `src/WebSpace/appsettings.example.json`. Copy it to `appsettings.Development.json` (git-ignored) and fill in your local values. Never commit real credentials.

## Database Setup

The database schema is defined in the [ERD](docs/diagrams/webspace_erd_v2.mermaid). Once the database is implemented, initialisation steps (creating the schema and loading seed data into a local MySQL instance) will be documented in [database/README.md](database/README.md).

## Contribution Guidelines

1. Pick or create an issue, then create a branch from `develop`:
   ```
   git checkout develop
   git pull
   git checkout -b feature/<short-description>
   ```
2. Implement the feature.
3. Test locally.
4. Commit with a meaningful message (see [Commit messages](#commit-messages)).
5. Push the branch: `git push -u origin feature/<short-description>`
6. Open a pull request into `develop` and complete the template checklist.
7. Another team member reviews and approves it.
8. Merge into `develop`.
9. Only tested, stable code is merged from `develop` into `main`.

Before every commit, check that no secrets, local configuration files or build output are included (`git status` and `git diff --staged`).

## Licence

This project is licensed under the [MIT Licence](LICENSE).
