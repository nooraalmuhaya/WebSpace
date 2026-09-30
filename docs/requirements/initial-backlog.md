# Initial Backlog

High-level issues to open on GitHub when the repository is created. Each one becomes a GitHub issue using the matching issue template, and can later be split into smaller issues per feature branch.

Responsible members are `TBD` until each member's GitHub username is confirmed.

| # | Issue | Template | Related Module | Branch |
|---|-------|----------|----------------|--------|
| 1 | Authentication and profile | Feature | Authentication & Profile | `feature/authentication` |
| 2 | Student learning | Feature | Student Learning | `feature/student-learning` |
| 3 | Quiz and assessment | Feature | Quiz & Assessment | `feature/assessment` |
| 4 | Tutor review and feedback | Feature | Tutor & Submission/Feedback | `feature/tutor` |
| 5 | Admin content management | Feature | Admin & Content Management | `feature/admin` |
| 6 | Database schema and seed data | Feature | Database | `feature/database` |
| 7 | Shared UI/UX layout | Feature | UI/UX | `feature/ui-layout` |
| 8 | Testing strategy | Testing | Testing | `test/<short-description>` |
| 9 | Project documentation | Documentation | Documentation | `docs/<short-description>` |
| 10 | Module integration | Feature | Integration / Cross-cutting | `feature/integration` |

---

## 1. Authentication and profile

**Description:** Account registration, login, logout, email verification, password reset and user profile management for Students, Tutors and Administrators. It follows the authentication wireframes (login, register, register success, verify email, forgot password, reset password, failed login).

**Acceptance Criteria**
- [ ] A student can register with full name, email, password and confirm password. Duplicate emails, weak passwords and mismatched passwords are rejected with field-level messages.
- [ ] A new account starts as *Pending verification* and becomes *Active* after the 6-digit email code is verified. The code is valid for 15 minutes.
- [ ] Login rejects invalid credentials with a generic message. It pauses login after 5 failed attempts for 15 minutes.
- [ ] Forgot password sends a reset link valid for 30 minutes without revealing whether the email exists. Expired links show the expired state.
- [ ] A successful password reset signs out other sessions.
- [ ] Passwords are stored only as hashes.
- [ ] Role-based authorization restricts Student, Tutor and Administrator areas.
- [ ] A user can view and edit their own profile.

**Responsible Member:** TBD

**Related Module:** Authentication & Profile

**Testing Requirements:** Unit tests for validation and token expiry. Integration tests for register → verify → login, lockout, and the reset flow. Manual test of every wireframe state.

## 2. Student learning

**Description:** Browsing roadmaps and courses, enrolment, weekly lessons, learning resources, coding challenges, discussions and progress tracking for students.

**Acceptance Criteria**
- [ ] A student can browse published roadmaps and courses and enrol in a course.
- [ ] An enrolled student can view course weeks, lessons and learning resources in order.
- [ ] Lesson progress is recorded and shown on a student dashboard.
- [ ] A student can attempt coding challenges, and attempts are stored.
- [ ] A student can create discussion posts and replies for a course or lesson.

**Responsible Member:** TBD

**Related Module:** Student Learning

**Testing Requirements:** Tests for enrolment rules, progress calculation and access control (unenrolled users cannot open course content).

## 3. Quiz and assessment

**Description:** Weekly quizzes with multiple-choice questions, attempts, automatic scoring, pass marks and attempt limits.

**Acceptance Criteria**
- [ ] A student can start and submit a quiz attempt, and answers are saved per question.
- [ ] Score and total marks are calculated automatically, and the pass mark is applied.
- [ ] `max_attempts` is enforced (NULL = unlimited).
- [ ] Explanations are shown after submission where provided.

**Responsible Member:** TBD

**Related Module:** Quiz & Assessment

**Testing Requirements:** Unit tests for scoring and the attempt limit. Integration test for the full attempt flow.

## 4. Tutor review and feedback

**Description:** Assignment submissions (text and files) by students, and review, feedback and decisions by the assigned tutors.

**Acceptance Criteria**
- [ ] A student can submit an assignment with text and/or files, and resubmissions are numbered.
- [ ] A tutor sees submissions only for courses they are assigned to.
- [ ] A tutor can record comments, required improvements and a decision, and the student can see them.
- [ ] File uploads are validated for type and size.

**Responsible Member:** TBD

**Related Module:** Tutor & Submission/Feedback

**Testing Requirements:** Authorization tests (a tutor cannot review an unassigned course). Tests for upload validation and the submission → feedback flow.

## 5. Admin content management

**Description:** Administrator management of users, roles, roadmaps, courses, weeks, lessons, resources, challenges, quizzes, assignments and tutor assignments.

**Acceptance Criteria**
- [ ] An administrator can create, edit, publish and unpublish roadmaps and courses.
- [ ] An administrator can manage course weeks, lessons, resources, challenges, quizzes and assignments.
- [ ] An administrator can assign tutors to courses and manage user account status.
- [ ] Only administrators can reach these pages.

**Responsible Member:** TBD

**Related Module:** Admin & Content Management

**Testing Requirements:** CRUD tests per entity, publish-state tests and admin-only authorization tests.

## 6. Database schema and seed data

**Description:** Implement the MySQL schema from `docs/diagrams/webspace_erd_v2.mermaid` and provide seed data for development and demonstration.

**Acceptance Criteria**
- [ ] All ERD tables, keys, unique constraints and relationships exist.
- [ ] Seed data includes the roles, one account per role and a sample roadmap/course.
- [ ] The whole team uses one agreed schema approach (SQL scripts or EF Core migrations).

**Responsible Member:** TBD

**Related Module:** Database

**Testing Requirements:** The schema builds on a clean MySQL database, and the seed script runs without errors.

## 7. Shared UI/UX layout

**Description:** Shared layout, navigation, styles and reusable components used by all modules, based on the team's wireframes.

**Acceptance Criteria**
- [ ] A shared layout and role-aware navigation exist.
- [ ] Forms show consistent validation messages.
- [ ] Pages are usable at mobile and desktop widths.

**Responsible Member:** TBD

**Related Module:** UI/UX

**Testing Requirements:** Manual responsive and accessibility checks across the main pages.

## 8. Testing strategy

**Description:** Set up the test projects and define the testing approach (unit, integration, user acceptance) for all modules.

**Acceptance Criteria**
- [ ] A test project is added to the solution and runs in CI.
- [ ] The test plan is documented in `docs/testing/`.

**Responsible Member:** TBD

**Related Module:** Testing

**Testing Requirements:** CI runs the test suite on every pull request.

## 9. Project documentation

**Description:** Maintain the requirements, design, diagrams, testing and user guide documentation in `docs/`.

**Acceptance Criteria**
- [ ] Use case, activity, workflow, authentication flow and state diagrams are added to `docs/diagrams/` as editable sources.
- [ ] A user guide exists for each role.

**Responsible Member:** TBD

**Related Module:** Documentation

**Testing Requirements:** Peer review of each document.

## 10. Module integration

**Description:** Integrate the five workstreams into one coherent WebSpace application: shared authentication, navigation, database context and end-to-end flows.

**Acceptance Criteria**
- [ ] All modules run in the single WebSpace application against one database.
- [ ] End-to-end flow works: register → enrol → learn → quiz → submit assignment → tutor feedback.

**Responsible Member:** TBD

**Related Module:** Integration / Cross-cutting

**Testing Requirements:** An end-to-end test of the full flow on `develop` before merging to `main`.
