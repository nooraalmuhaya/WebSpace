# Database Design Decisions

**Status:** Decisions approved on 2026-10-01 (see *Approved decisions* below). The sections after that table record the original analysis and options; where a section's recommendation differs from the approved decision, **the approved decision wins**.
**Design authority:** [webspace_erd_organized.mermaid](../diagrams/webspace_erd_organized.mermaid) (32 entities, 43 relationships, 202 attributes). The ERD is frozen, and none of these decisions change it.
**Related:** [erd-implementation-mapping.md](erd-implementation-mapping.md)

## Approved decisions (2026-10-01)

| ID | Decision | Basis |
|----|----------|-------|
| G2 | Create exactly six additional unique indexes: ENROLMENT(student_user_id, course_id), LESSON_PROGRESS(student_user_id, lesson_id), CHALLENGE_PROGRESS(student_user_id, challenge_id), SUBMISSION(assignment_id, student_user_id, attempt_number), QUIZ_ATTEMPT_ANSWER(attempt_id, question_id), QUIZ_OPTION(question_id, display_order). No others. | Data-integrity rules implied by the ERD workflow |
| G3 | `FEEDBACK.required_improvement` is **nullable**. `FEEDBACK.decision` is **NOT NULL**, following the ERD literally, so a decision is chosen even when saving a draft. All other fields follow the ERD's "nullable" notes exactly. | ERD note "required if NeedsImprovement"; ERD has no nullable note on `decision` |
| G4 | Use the reviewed physical type mapping. Re-check it against the generated migration. | Technical review |
| G5 | Use the reviewed strategy: Restrict 19, Cascade 22, SetNull 2. Users with academic history are deactivated, not hard-deleted. | Protects academic history |
| G5-a | ACTIVITY_LOG → USER_ACCOUNT uses **Restrict**. Activity history is never deleted automatically. | ERD: null means "System" |
| G6-a | **Not enforced.** A course is not required to appear in its primary roadmap's sequence. | Not specified in any project document |
| G6-b | The assignment becomes available once the student has **completed (submitted)** the week's quiz. **Passing is not required.** The pass mark is shown as assessment/result information only. | Proposal §5: "After completing the required learning activities and quiz…"; no document requires passing |
| G7 | Students create posts and replies. Tutors reply, pin and close. Authors edit their own **posts** only. Guests have no access. Editing replies is **not** implemented. Admin deletion of discussion content is **not** a required feature at this stage. | Proposal §2, §5 (posting and replying). Pin, close and edit-own-post are *implementation decisions*, not assignment requirements. |
| G8 | Keep ACTIVITY_LOG, because it's in the ERD. Write through a small helper for meaningful events only. No log management, editing or deleting. A read-only recent-activity list may be added only if the final admin wireframe shows one. | Not required by the brief or the proposal; present in the ERD |

In the report, the G7 pin, close and edit permissions and all of G8 must be described as **implementation decisions**, not as assignment requirements.

The final ERD is a *conceptual/logical* design. Turning it into MySQL tables through EF Core needs some *physical* decisions that the ERD doesn't state. This document lists them (G2 to G8), recommends an option for each, and says whether the option changes the ERD structure (entities, attributes, keys, relationships or cardinalities).

None of the recommendations add or remove entities, attributes, keys or relationships.

| ID | Topic | Changes ERD structure? | Implemented in |
|----|-------|------------------------|----------------|
| G2 | Unique constraints | No (adds indexes only) | EF Core Fluent API (`HasIndex().IsUnique()`) |
| G3 | Nullability | No. Two annotations would be clarified (see G3) | EF Core Fluent API and application validation |
| G4 | VARCHAR lengths | No (physical sizing only) | EF Core Fluent API (`HasMaxLength`) and view-model validation |
| G5 | Delete behaviour | No | EF Core Fluent API (`OnDelete`) |
| G6 | Cross-entity rules | No | Application validation |
| G7 | Discussion permissions | No | ASP.NET Core authorization and controller checks |
| G8 | Activity log behaviour | No | A small application helper and one read-only list |

---

## G2: Unique constraints

**Current ERD situation.** The ERD states these unique rules:
- **Single column:** `ROLE.name`, `USER_ACCOUNT.email`, `PASSWORD_RESET_TOKEN.token_hash`, `TOPIC.name`, `QUIZ.course_week_id`, `ASSIGNMENT.course_week_id`, `FEEDBACK.submission_id`
- **Composite:** "unique per …" rules on `COURSE_ROADMAP.sequence_position`, `COURSE_WEEK.week_number`, `LESSON.lesson_order`, `QUIZ_QUESTION.display_order` and `QUIZ_ATTEMPT.attempt_number`

These are implemented exactly as stated.

**Problem.** Some workflows depend on "one row per student per item", but the ERD doesn't mark those combinations unique. Without a constraint, a double-click or two browser tabs can insert duplicate enrolment or progress rows. The student dashboard would then double-count progress.

**Recommended implementation.** Add a database-level unique index only where it protects a genuine integrity rule:

| Entity | Unique columns | Why it's a genuine rule |
|--------|----------------|-------------------------|
| ENROLMENT | (student_user_id, course_id) | A student enrols in a course once. `status` only has InProgress/Completed, with no "withdrawn/re-enrolled" state that would need a second row. |
| LESSON_PROGRESS | (student_user_id, lesson_id) | Progress is one state per lesson (InProgress → Completed) |
| CHALLENGE_PROGRESS | (student_user_id, challenge_id) | `saved_code` is "auto-saved", which means one working copy per challenge that gets overwritten |
| SUBMISSION | (assignment_id, student_user_id, attempt_number) | Same pattern the ERD already applies to QUIZ_ATTEMPT ("unique per student + quiz") |
| QUIZ_ATTEMPT_ANSWER | (attempt_id, question_id) | One recorded answer per question per attempt; otherwise the score could be counted twice |
| QUIZ_OPTION | (question_id, display_order) | The ERD labels the order "A, B, C, D", so two options can't both be "B". Mirrors the ERD's rule for QUIZ_QUESTION. |

**Not recommended (unnecessary uniqueness):**
- `COURSE.title`, `ROADMAP.title`
- `display_order` on COURSE_LEARNING_OUTCOME, ROADMAP_OBJECTIVE and ASSIGNMENT_REQUIREMENT (simple lists, where uniqueness would make re-ordering harder)
- `EMAIL_VERIFICATION.code_hash` (6-digit codes can legitimately repeat between users)
- `SUBMISSION_FILE.stored_file_path` (the application generates unique names anyway)

**Changes ERD structure?** No. Only indexes are added.
**Implementable without changing the ERD?** Yes, through `HasIndex(...).IsUnique()`. The application also checks before inserting so it can show a friendly message; the index is the last line of defence.

---

## G3: Nullability

**Current ERD situation.** The ERD marks nullable fields with the note "nullable". Every other field is treated as NOT NULL.

**Problem.** Four fields are NOT NULL by that rule, but the ERD's own notes or workflow states suggest a legitimate "no value yet" state:

| Field | ERD note | Workflow state | Recommendation |
|-------|----------|----------------|----------------|
| `FEEDBACK.required_improvement` | "required if NeedsImprovement" | When the decision is **Approved**, there is no improvement text | **Nullable.** Validation requires it when `decision = NeedsImprovement`. The ERD note already implies this. |
| `FEEDBACK.decision` | (none) | `FEEDBACK.status` has a **Draft** state, and a tutor may save a draft before choosing a decision | *Original recommendation:* nullable. **Approved decision: NOT NULL** (ERD literal); a decision is chosen even when saving a draft. |
| `QUIZ_ATTEMPT_ANSWER.is_correct`, `marks_awarded` | "set on submit" | Only matters if answers are saved *before* submit | **Keep NOT NULL.** Answer rows are inserted in one transaction when the attempt is submitted, and scored at the same time. No requirement asks for resuming a half-answered quiz. |
| `CHALLENGE_PROGRESS.saved_code` | "auto-saved" | The row is created on the first save, so code always exists | **Keep NOT NULL.** Create the row on first save; an empty editor is stored as `''`. |

`FEEDBACK.feedback_text` stays NOT NULL. The "min 20 characters" rule is checked when a draft is **submitted**, not while it's saved as a draft.

**Changes ERD structure?** No entities, attributes, keys or relationships change. Two attributes (`required_improvement`, `decision`) would carry a "nullable" note in the ERD to match. That clarifies an annotation and needs the team's approval before the ERD file is edited.
**Implementable without changing the ERD?** Yes, with `IsRequired(false)` in Fluent API plus conditional validation.

---

## G4: VARCHAR lengths

**Current ERD situation.** The ERD uses `varchar` and `text` without lengths.

**Problem.** MySQL needs a length for every `VARCHAR`. InnoDB with `utf8mb4` limits an index key to 3072 bytes (768 characters), so indexed columns must fit within that. Without explicit lengths, Pomelo maps `string` to `longtext`, which can't be indexed.

**Recommended lengths.** The lengths are based on what each field holds.

| Purpose | Fields | Length |
|---------|--------|--------|
| Enumerated values | account_status, status (all), difficulty, resource_type, assignment_type, decision | 30 (longest value is `PendingVerification`, 19 characters) |
| Role name | ROLE.name | 50 |
| Topic name | TOPIC.name | 50 |
| Person name | full_name | 100 |
| Email | email, target_email | 254 (the maximum length of an email address, RFC 5321) |
| Password hash | password_hash | 255 (ASP.NET Core `PasswordHasher` output is about 84 characters; this leaves room for future formats) |
| Security stamp | security_stamp | 64 |
| Token/code hash | code_hash, token_hash | 128 (SHA-256 hex is 64 characters) |
| Titles | all `title` fields | 150 |
| Short list text | outcome_text, objective_text, requirement_text, option_text, LEARNING_RESOURCE.description, ROLE.description | 500 (ROLE.description 255) |
| Audit text | ACTIVITY_LOG.action 100, entity_type 50, record_label 100 | as listed |
| Stored file paths | thumbnail_path, file_path, stored_file_path | 500 |
| Original file name | original_file_name | 255 |
| External URL | resource_url | 2048 |
| Accepted file types | accepted_file_types | 200 |

The ERD's `text` type maps to MySQL `TEXT` (up to 64 KB). Four fields that may hold long code or lesson bodies map to `MEDIUMTEXT` instead: `LESSON.content`, `CODING_CHALLENGE.starter_code`, `CODING_CHALLENGE.expected_output` and `CHALLENGE_PROGRESS.saved_code`.

**Changes ERD structure?** No. These are physical sizes of the ERD's types.
**Implementable without changing the ERD?** Yes, with `HasMaxLength(n)` or `HasColumnType(...)`. The same limits appear as `[StringLength]` on form view models, so users see a validation message instead of a database error.

---

## G5: Delete behaviour

**Current ERD situation.** Only one delete rule is stated: `FEEDBACK.tutor_user_id` is "nullable after tutor deleted", which means **SET NULL**.

**Problem.** EF Core defaults to **Cascade** for required relationships. Left as the default, deleting a user or a course would delete enrolments, quiz attempts, submissions and feedback: academic records the proposal says the system must keep ("learning activities and progress are recorded by the system", Proposal §5).

**Recommended principles:**
1. **Academic records are never deleted by cascade.** These are ENROLMENT, LESSON_PROGRESS, CHALLENGE_PROGRESS, QUIZ_ATTEMPT, QUIZ_ATTEMPT_ANSWER, SUBMISSION, SUBMISSION_FILE and FEEDBACK. Every FK that points *from* them to a user or to content is **Restrict**.
2. **Content that exists only inside its parent cascades** (Course → Week → Lesson → Resource). An administrator can then delete a draft course in one action. Because of rule 1, MySQL rejects the whole delete if any student record exists anywhere underneath. The application then offers **Unpublish** (`is_published = false`) instead.
3. **Users are deactivated, not deleted,** once they have any academic or discussion history (`account_status = Inactive`). Hard delete stays available for accounts with no history, such as an unverified registration. This matches the proposal's admin objective "deleting … user data" without losing records.

| Relationship (parent → child) | On delete | Reason |
|-------------------------------|-----------|--------|
| ROLE → USER_ACCOUNT | Restrict | A role can't be removed while users hold it |
| USER_ACCOUNT → EMAIL_VERIFICATION | Cascade | Security tokens have no meaning without the user |
| USER_ACCOUNT → PASSWORD_RESET_TOKEN | Cascade | Same as above |
| USER_ACCOUNT → ACTIVITY_LOG | Restrict | In the ERD, null means "System", so SET NULL would falsely relabel a user's actions as system actions |
| USER_ACCOUNT → COURSE_TUTOR | Cascade | A tutor assignment is not an academic record |
| USER_ACCOUNT → ENROLMENT, LESSON_PROGRESS, CHALLENGE_PROGRESS, QUIZ_ATTEMPT, SUBMISSION | Restrict | Academic records (rule 1) |
| USER_ACCOUNT → DISCUSSION_POST, DISCUSSION_REPLY | Restrict | Keeps other students' discussion threads intact |
| USER_ACCOUNT → FEEDBACK | **Set Null** | Stated in the ERD |
| ROADMAP → COURSE (primary) | Restrict | A course must always have a primary roadmap |
| ROADMAP → COURSE_ROADMAP, ROADMAP_OBJECTIVE, ROADMAP_TOPIC | Cascade | Parts of the roadmap |
| TOPIC → ROADMAP_TOPIC | Cascade | Removing a topic removes its tags |
| COURSE → COURSE_ROADMAP, COURSE_LEARNING_OUTCOME, COURSE_TUTOR, COURSE_WEEK | Cascade | Parts of the course (rule 2) |
| COURSE → ENROLMENT | Restrict | Academic record |
| COURSE → DISCUSSION_POST | Restrict | Community content isn't removed silently |
| COURSE_WEEK → LESSON, QUIZ, ASSIGNMENT | Cascade | Parts of the week |
| COURSE_WEEK → DISCUSSION_POST | Set Null | Post falls back to "General", which the ERD defines as null |
| LESSON → LEARNING_RESOURCE, CODING_CHALLENGE | Cascade | Parts of the lesson |
| LESSON → LESSON_PROGRESS | Restrict | Academic record |
| CODING_CHALLENGE → CHALLENGE_PROGRESS | Restrict | Academic record |
| QUIZ → QUIZ_QUESTION | Cascade | Parts of the quiz |
| QUIZ → QUIZ_ATTEMPT | Restrict | Academic record |
| QUIZ_QUESTION → QUIZ_OPTION | Cascade | Parts of the question |
| QUIZ_QUESTION → QUIZ_ATTEMPT_ANSWER | Restrict | Academic record |
| QUIZ_OPTION → QUIZ_ATTEMPT_ANSWER | Restrict | In the ERD, null means "not answered", so SET NULL would falsify the answer |
| QUIZ_ATTEMPT → QUIZ_ATTEMPT_ANSWER | Cascade | Answers are part of the attempt (attempts aren't deleted by any workflow) |
| ASSIGNMENT → ASSIGNMENT_REQUIREMENT | Cascade | Parts of the assignment |
| ASSIGNMENT → SUBMISSION | Restrict | Academic record |
| SUBMISSION → SUBMISSION_FILE | Cascade | Files are part of the submission (submissions aren't deleted by any workflow) |
| SUBMISSION → FEEDBACK | Restrict | Academic record |
| DISCUSSION_POST → DISCUSSION_REPLY | Cascade | A moderator deleting a post removes its thread |

The table covers all 43 relationships. Most rows group several of them.

**Changes ERD structure?** No.
**Implementable without changing the ERD?** Yes, with `OnDelete(DeleteBehavior.Restrict | Cascade | SetNull)`. MySQL allows several cascade paths to the same table, so no workaround is needed.

---

## G6: Role and course validation rules

**Current ERD situation.** Several FKs point to USER_ACCOUNT under a *role*: tutor_user_id, student_user_id and author_user_id. Several child rows must belong to the *same* parent as a sibling FK.

**Problem.** A foreign key can only check that a referenced row exists. It can't check that the row has the right role, or that two FKs agree with each other.

**Recommended implementation.** Enforce these rules in application validation, before `SaveChanges`, and show a user-facing message:

| Rule | Where it applies |
|------|------------------|
| `COURSE_TUTOR.tutor_user_id` must be a user with role **Tutor** | Admin: assign tutor |
| `student_user_id` (ENROLMENT, LESSON_PROGRESS, CHALLENGE_PROGRESS, QUIZ_ATTEMPT, SUBMISSION) must be a **Student** | Student workflows. The ID always comes from the signed-in user, never from a form. |
| `FEEDBACK.tutor_user_id` must be assigned to the submission's course through COURSE_TUTOR | Tutor: review |
| `DISCUSSION_POST.course_week_id` must belong to `DISCUSSION_POST.course_id` | Create post |
| `QUIZ_ATTEMPT_ANSWER.question_id` must belong to the attempt's quiz, and `selected_option_id` to that question | Quiz submit |
| Exactly one `QUIZ_OPTION.is_correct = true` per question (ERD note) | Admin: question editor |
| `LEARNING_RESOURCE`: `resource_url` required for Video/Link and `file_path` for File (ERD notes) | Admin: resource form |
| `required_improvement` required when the decision is NeedsImprovement; `feedback_text` at least 20 characters (ERD notes) | Tutor: submit feedback |
| `QUIZ_ATTEMPT` count limited by `max_attempts` (null = unlimited) | Start quiz |
| `attempt_number` = previous highest + 1 for QUIZ_ATTEMPT and SUBMISSION | Quiz start, resubmission |
| Resubmission is allowed only when the latest submission is NeedsImprovement (Proposal §5) | Submit assignment |
| Lockout after 5 failures for 15 min; codes valid 15 min, reset links 30 min (ERD notes and wireframes) | Account |

**Open sub-decisions (these are requirement interpretations):**
- **G6-a.** Should the course's `primary_roadmap_id` also appear in COURSE_ROADMAP? The ERD doesn't say. *Original recommendation:* enforce it in the admin course form. **Approved decision: not enforced** (not specified in the documents).
- **G6-b.** Proposal §5 says the assignment opens "after completing the required learning activities and quiz". Does completing mean **submitted**, or **passed** (`score ≥ pass_mark_percent`)? *Original recommendation:* passed. **Approved decision: submitted.** The pass mark is result information only.

**Changes ERD structure?** No.
**Implementable without changing the ERD?** Yes, in application validation only.

---

## G7: Discussion permissions

**Current ERD situation.**
- DISCUSSION_POST has `is_pinned`, `is_closed` and `updated_at`.
- DISCUSSION_REPLY has **no** `updated_at`.
- A post belongs to a course, and optionally to a week.

**What the documents say:**
- **Proposal §2:** "Students can post questions or comments … and receive responses from other students or perhaps even tutors."
- **Proposal §5:** "students can ask questions, share comments and receive … responses from other students or tutors."
- **Backlog #2:** "A student can create discussion posts and replies for a course."
- **Assignment brief:** lists "asynchronous online discussions" as an example learning activity.
- No discussion wireframe is available.

**Problem.** The requirements cover creating and reading posts. They don't say who pins or closes posts, or who can edit or delete them, even though the ERD has fields for pinning and closing.

**Recommended permissions.** Rows marked *Decision* are implementation decisions, not requirements:

| Action | Student (enrolled) | Tutor (assigned to the course) | Administrator | Basis |
|--------|--------------------|--------------------------------|---------------|-------|
| Read course discussions | ✔ | ✔ | ✔ | Requirement |
| Create post | ✔ | ✔ | — | Requirement (students). *Decision* (tutors, for announcements that can be pinned) |
| Reply | ✔ (not on closed posts) | ✔ | ✔ | Requirement |
| Edit own post | ✔ | ✔ | — | *Decision*, supported by `updated_at` on the post |
| Edit reply | — | — | — | Not supported: the ERD has no `updated_at` on replies |
| Pin / close post | — | ✔ | ✔ | *Decision*: someone must set these ERD fields; moderation belongs to staff |
| Delete post or reply | — | — | ✔ | *Decision*: moderation. Gives the admin a genuine Delete operation on discussion content. |
| Guests | no access | | | Proposal §2 restricts learning features to registered users |

**Changes ERD structure?** No.
**Implementable without changing the ERD?** Yes, with `[Authorize(Roles = …)]` plus checks for "is the author", "is enrolled" and "is an assigned tutor" in the controller.

---

## G8: Activity log behaviour

**Current ERD situation.** ACTIVITY_LOG has `action` ("e.g. Created Submission"), `entity_type`, `record_label` ("e.g. SUB-00482"), a nullable `user_id` ("nullable = System") and `created_at`.

**Problem.** The proposal and the assignment brief don't mention an audit log or activity screen. The examples in the ERD suggest an admin dashboard "recent activity" panel, but that wireframe hasn't been provided. Building a full audit UI would add scope that nothing requires.

**Recommended implementation (minimal scope):**
1. **Write:** one helper method, `ActivityLogger.LogAsync(userId, action, entityType, recordLabel)`, called explicitly from a short list of important write actions:
   - account registered or verified
   - submission created
   - feedback submitted
   - admin create, update or delete of roadmaps, courses, weeks, lessons, quizzes and assignments
   - tutor assigned

   No interceptors and no automatic change tracking.
2. **Read:** a read-only "Recent activity" list (latest 10–20 rows) on the **Administrator dashboard** only, **if** the admin dashboard wireframe includes it. Otherwise the table is written but not displayed.
3. **No** create, edit or delete UI for log rows. The log is append-only.

**Changes ERD structure?** No.
**Implementable without changing the ERD?** Yes.
**Decision needed:** confirm whether the admin dashboard wireframe shows a recent-activity panel.

---

## G9: Wireframe coverage (traceability basis)

The database can be implemented from the ERD and requirements. Missing wireframes don't block Stages 3 to 9. They matter for the page-level traceability matrix (Stage 10 onwards) and for the report's *Storyboard & Design Documents* section.

| Area | Screens | Wireframe available? | Traceability basis until provided |
|------|---------|----------------------|-----------------------------------|
| Authentication | Login, failed login, Register, Register success, Verify email, Forgot password, Reset password | **Yes** (authentication.pdf, 7 pages) | **Wireframe-verified (W)** |
| Public catalogue | Home, roadmap list/detail, course list/detail | No | Requirement (R): Proposal §2, §5 |
| Student | Dashboard, course/week view, lesson view, coding challenge, quiz take/result, assignment submit/status, discussions | No | R: Proposal §2, §5 |
| Tutor | Review queue, submission review/feedback form | No | R: Proposal §2, §5. ERD notes (FB-00219, min 20 characters) suggest a wireframe exists. |
| Administrator | Dashboard, CRUD forms for users, roadmaps, courses, weeks, lessons, resources, challenges, quizzes/questions, assignments, tutor assignment | No | R: Proposal §2, §5. The ERD note "missing from admin form" (ASSIGNMENT.due_at) suggests a wireframe exists. |

**Most needed, in priority order:**
1. Admin assignment form (resolves `due_at`)
2. Tutor feedback form
3. Quiz take/result
4. Coding challenge (resolves `expected_output`, which the ERD marks "needs confirmation")
5. Admin dashboard (resolves G8)
6. Student dashboard
