# ERD → EF Core Implementation Mapping (Stage 3 plan)

**Status:** Mapping approved on 2026-10-01. Stage 4 (entity classes and enums) is implemented; the DbContext, migration and tables don't exist yet.
**Design authority:** [webspace_erd_organized.mermaid](../diagrams/webspace_erd_organized.mermaid): 32 entities, 43 relationships, 202 attributes.
**Decisions:** items tagged **[G2]**, **[G3]**, **[G4]** … are resolved in the *Approved decisions* table of [database-design-decisions.md](database-design-decisions.md).

**Technology (verified in `C:\dev\WebSpace`):**
- .NET SDK 10.0.401, `net10.0`
- Pomelo.EntityFrameworkCore.MySql **9.0.0**, which brings in EF Core 9.0.0 and MySqlConnector 2.4.0
- MySQL Community Server 26.7.0

---

## Naming conventions (applies to A–E)

| Layer | Rule | Example |
|-------|------|---------|
| MySQL table | ERD entity name in lower case | `USER_ACCOUNT` → `user_account` |
| MySQL column | ERD attribute name, unchanged | `full_name` |
| C# class | PascalCase of the ERD entity, singular | `UserAccount` |
| C# property | PascalCase of the ERD attribute | `FullName` |
| C# navigation | Descriptive name for the related entity; no database column | `Student`, `Enrolments` |

Converting the PascalCase names back to snake_case reproduces every ERD name exactly. The mapping is therefore applied **once**, as a naming loop in `ApplicationDbContext.OnModelCreating`, with no third-party package. The generated migration is then checked against the ERD, name by name.

Tables are lower case because MySQL on Windows stores table names in lower case (`lower_case_table_names=1`). Using lower case everywhere keeps the schema identical on team members' macOS and Linux machines.

**No entity or attribute is renamed, added or removed.** The only C#-only members are navigation properties. They are object references that EF Core uses to load related rows and don't create columns.

Planned folders (inside the existing `src/WebSpace` project):

```
src/WebSpace/
├── Data/ApplicationDbContext.cs      (one DbContext)
├── Data/Migrations/                  (EF Core migrations)
├── Models/Entities/                  (32 entity classes, one per ERD entity)
├── Models/Enums/                     (10 enums, see H)
└── Models/ErrorViewModel.cs          (existing, untouched)
```

Relationship configuration lives in `OnModelCreating`, grouped by the ERD's eight sections. There are no repositories or extra service layers.

---

## A. ERD entity → C# class → DbSet

| # | ERD section | ERD entity | C# class | MySQL table | DbSet |
|---|-------------|------------|----------|-------------|-------|
| 1 | Accounts | ROLE | `Role` | `role` | `Roles` |
| 2 | Accounts | USER_ACCOUNT | `UserAccount` | `user_account` | `UserAccounts` |
| 3 | Accounts | EMAIL_VERIFICATION | `EmailVerification` | `email_verification` | `EmailVerifications` |
| 4 | Accounts | PASSWORD_RESET_TOKEN | `PasswordResetToken` | `password_reset_token` | `PasswordResetTokens` |
| 5 | Accounts | ACTIVITY_LOG | `ActivityLog` | `activity_log` | `ActivityLogs` |
| 6 | Membership | COURSE_TUTOR | `CourseTutor` | `course_tutor` | `CourseTutors` |
| 7 | Membership | ENROLMENT | `Enrolment` | `enrolment` | `Enrolments` |
| 8 | Discussions | DISCUSSION_POST | `DiscussionPost` | `discussion_post` | `DiscussionPosts` |
| 9 | Discussions | DISCUSSION_REPLY | `DiscussionReply` | `discussion_reply` | `DiscussionReplies` |
| 10 | Catalogue | COURSE | `Course` | `course` | `Courses` |
| 11 | Catalogue | COURSE_LEARNING_OUTCOME | `CourseLearningOutcome` | `course_learning_outcome` | `CourseLearningOutcomes` |
| 12 | Catalogue | COURSE_ROADMAP | `CourseRoadmap` | `course_roadmap` | `CourseRoadmaps` |
| 13 | Catalogue | ROADMAP | `Roadmap` | `roadmap` | `Roadmaps` |
| 14 | Catalogue | ROADMAP_OBJECTIVE | `RoadmapObjective` | `roadmap_objective` | `RoadmapObjectives` |
| 15 | Catalogue | ROADMAP_TOPIC | `RoadmapTopic` | `roadmap_topic` | `RoadmapTopics` |
| 16 | Catalogue | TOPIC | `Topic` | `topic` | `Topics` |
| 17 | Content | COURSE_WEEK | `CourseWeek` | `course_week` | `CourseWeeks` |
| 18 | Content | LESSON | `Lesson` | `lesson` | `Lessons` |
| 19 | Content | LEARNING_RESOURCE | `LearningResource` | `learning_resource` | `LearningResources` |
| 20 | Content | CODING_CHALLENGE | `CodingChallenge` | `coding_challenge` | `CodingChallenges` |
| 21 | Progress | LESSON_PROGRESS | `LessonProgress` | `lesson_progress` | `LessonProgresses` |
| 22 | Progress | CHALLENGE_PROGRESS | `ChallengeProgress` | `challenge_progress` | `ChallengeProgresses` |
| 23 | Quizzes | QUIZ | `Quiz` | `quiz` | `Quizzes` |
| 24 | Quizzes | QUIZ_QUESTION | `QuizQuestion` | `quiz_question` | `QuizQuestions` |
| 25 | Quizzes | QUIZ_OPTION | `QuizOption` | `quiz_option` | `QuizOptions` |
| 26 | Quizzes | QUIZ_ATTEMPT | `QuizAttempt` | `quiz_attempt` | `QuizAttempts` |
| 27 | Quizzes | QUIZ_ATTEMPT_ANSWER | `QuizAttemptAnswer` | `quiz_attempt_answer` | `QuizAttemptAnswers` |
| 28 | Assignments | ASSIGNMENT | `Assignment` | `assignment` | `Assignments` |
| 29 | Assignments | ASSIGNMENT_REQUIREMENT | `AssignmentRequirement` | `assignment_requirement` | `AssignmentRequirements` |
| 30 | Assignments | SUBMISSION | `Submission` | `submission` | `Submissions` |
| 31 | Assignments | SUBMISSION_FILE | `SubmissionFile` | `submission_file` | `SubmissionFiles` |
| 32 | Assignments | FEEDBACK | `Feedback` | `feedback` | `Feedbacks` |

---

## B + I + L. Attribute → property, nullability and MySQL type (202 attributes)

Key to the columns:
- **Null:** NN = NOT NULL, NULL = nullable. A nullable C# type ends in `?`.
- **Key:** PK primary key, FK foreign key, UK unique.
- **AI:** auto-increment identity.
- **Default:** the database default taken from the ERD.
- Every `datetime` value is stored in **UTC** (`DateTime`, `DATETIME(6)`).
- Enum-typed properties are stored as their text value in `VARCHAR(30)` (see H).

### 1. Accounts, security and audit

**ROLE → `Role`** (3)
| ERD attribute | C# property | C# type | MySQL type | Null | Key / default |
|---|---|---|---|---|---|
| role_id | RoleId | int | INT AI | NN | PK |
| name | Name | string | VARCHAR(50) [G4] | NN | UK |
| description | Description | string? | VARCHAR(255) [G4] | NULL | |

**USER_ACCOUNT → `UserAccount`** (13)
| ERD attribute | C# property | C# type | MySQL type | Null | Key / default |
|---|---|---|---|---|---|
| user_id | UserId | int | INT AI | NN | PK |
| role_id | RoleId | int | INT | NN | FK → role |
| full_name | FullName | string | VARCHAR(100) | NN | |
| email | Email | string | VARCHAR(254) | NN | UK (the default `utf8mb4_0900_ai_ci` collation makes it case-insensitive) |
| password_hash | PasswordHash | string | VARCHAR(255) | NN | never plain text |
| account_status | AccountStatus | `AccountStatus` | VARCHAR(30) | NN | |
| email_verified_at | EmailVerifiedAt | DateTime? | DATETIME(6) | NULL | |
| failed_login_count | FailedLoginCount | int | INT | NN | default 0 |
| lockout_end_at | LockoutEndAt | DateTime? | DATETIME(6) | NULL | |
| security_stamp | SecurityStamp | string | VARCHAR(64) | NN | |
| last_login_at | LastLoginAt | DateTime? | DATETIME(6) | NULL | |
| created_at | CreatedAt | DateTime | DATETIME(6) | NN | |
| updated_at | UpdatedAt | DateTime? | DATETIME(6) | NULL | |

**EMAIL_VERIFICATION → `EmailVerification`** (7)
| ERD attribute | C# property | C# type | MySQL type | Null | Key |
|---|---|---|---|---|---|
| verification_id | VerificationId | int | INT AI | NN | PK |
| user_id | UserId | int | INT | NN | FK → user_account |
| target_email | TargetEmail | string | VARCHAR(254) | NN | |
| code_hash | CodeHash | string | VARCHAR(128) | NN | |
| expires_at | ExpiresAt | DateTime | DATETIME(6) | NN | |
| used_at | UsedAt | DateTime? | DATETIME(6) | NULL | |
| created_at | CreatedAt | DateTime | DATETIME(6) | NN | |

**PASSWORD_RESET_TOKEN → `PasswordResetToken`** (6)
| ERD attribute | C# property | C# type | MySQL type | Null | Key |
|---|---|---|---|---|---|
| reset_token_id | ResetTokenId | int | INT AI | NN | PK |
| user_id | UserId | int | INT | NN | FK → user_account |
| token_hash | TokenHash | string | VARCHAR(128) | NN | UK |
| expires_at | ExpiresAt | DateTime | DATETIME(6) | NN | |
| used_at | UsedAt | DateTime? | DATETIME(6) | NULL | |
| created_at | CreatedAt | DateTime | DATETIME(6) | NN | |

**ACTIVITY_LOG → `ActivityLog`** (6)
| ERD attribute | C# property | C# type | MySQL type | Null | Key |
|---|---|---|---|---|---|
| activity_id | ActivityId | int | INT AI | NN | PK |
| user_id | UserId | int? | INT | NULL | FK → user_account (null = System) |
| action | Action | string | VARCHAR(100) | NN | |
| entity_type | EntityType | string | VARCHAR(50) | NN | |
| record_label | RecordLabel | string | VARCHAR(100) | NN | |
| created_at | CreatedAt | DateTime | DATETIME(6) | NN | |

### 2. Course membership

**COURSE_TUTOR → `CourseTutor`** (3)
| ERD attribute | C# property | C# type | MySQL type | Null | Key |
|---|---|---|---|---|---|
| course_id | CourseId | int | INT | NN | PK (composite), FK → course |
| tutor_user_id | TutorUserId | int | INT | NN | PK (composite), FK → user_account |
| assigned_at | AssignedAt | DateTime | DATETIME(6) | NN | |

**ENROLMENT → `Enrolment`** (7)
| ERD attribute | C# property | C# type | MySQL type | Null | Key |
|---|---|---|---|---|---|
| enrolment_id | EnrolmentId | int | INT AI | NN | PK |
| student_user_id | StudentUserId | int | INT | NN | FK → user_account |
| course_id | CourseId | int | INT | NN | FK → course |
| status | Status | `EnrolmentStatus` | VARCHAR(30) | NN | |
| enrolled_at | EnrolledAt | DateTime | DATETIME(6) | NN | |
| completed_at | CompletedAt | DateTime? | DATETIME(6) | NULL | |
| last_accessed_at | LastAccessedAt | DateTime? | DATETIME(6) | NULL | |

### 3. Discussions

**DISCUSSION_POST → `DiscussionPost`** (10)
| ERD attribute | C# property | C# type | MySQL type | Null | Key / default |
|---|---|---|---|---|---|
| post_id | PostId | int | INT AI | NN | PK |
| course_id | CourseId | int | INT | NN | FK → course |
| course_week_id | CourseWeekId | int? | INT | NULL | FK → course_week (null = General) |
| author_user_id | AuthorUserId | int | INT | NN | FK → user_account |
| title | Title | string | VARCHAR(150) | NN | |
| content | Content | string | TEXT | NN | |
| is_pinned | IsPinned | bool | TINYINT(1) | NN | default 0 |
| is_closed | IsClosed | bool | TINYINT(1) | NN | default 0 |
| created_at | CreatedAt | DateTime | DATETIME(6) | NN | |
| updated_at | UpdatedAt | DateTime? | DATETIME(6) | NULL | |

**DISCUSSION_REPLY → `DiscussionReply`** (5)
| ERD attribute | C# property | C# type | MySQL type | Null | Key |
|---|---|---|---|---|---|
| reply_id | ReplyId | int | INT AI | NN | PK |
| post_id | PostId | int | INT | NN | FK → discussion_post |
| author_user_id | AuthorUserId | int | INT | NN | FK → user_account |
| content | Content | string | TEXT | NN | |
| created_at | CreatedAt | DateTime | DATETIME(6) | NN | |

### 4. Course catalogue and roadmaps

**COURSE → `Course`** (10)
| ERD attribute | C# property | C# type | MySQL type | Null | Key / default |
|---|---|---|---|---|---|
| course_id | CourseId | int | INT AI | NN | PK |
| primary_roadmap_id | PrimaryRoadmapId | int | INT | NN | FK → roadmap |
| title | Title | string | VARCHAR(150) | NN | |
| description | Description | string | TEXT | NN | |
| difficulty | Difficulty | `DifficultyLevel` | VARCHAR(30) | NN | |
| estimated_weeks | EstimatedWeeks | int? | INT | NULL | |
| thumbnail_path | ThumbnailPath | string? | VARCHAR(500) | NULL | |
| is_published | IsPublished | bool | TINYINT(1) | NN | default 0 (Draft) |
| created_at | CreatedAt | DateTime | DATETIME(6) | NN | |
| updated_at | UpdatedAt | DateTime? | DATETIME(6) | NULL | |

**COURSE_LEARNING_OUTCOME → `CourseLearningOutcome`** (4)
| ERD attribute | C# property | C# type | MySQL type | Null | Key |
|---|---|---|---|---|---|
| outcome_id | OutcomeId | int | INT AI | NN | PK |
| course_id | CourseId | int | INT | NN | FK → course |
| outcome_text | OutcomeText | string | VARCHAR(500) | NN | |
| display_order | DisplayOrder | int | INT | NN | |

**COURSE_ROADMAP → `CourseRoadmap`** (3)
| ERD attribute | C# property | C# type | MySQL type | Null | Key |
|---|---|---|---|---|---|
| roadmap_id | RoadmapId | int | INT | NN | PK (composite), FK → roadmap |
| course_id | CourseId | int | INT | NN | PK (composite), FK → course |
| sequence_position | SequencePosition | int | INT | NN | UK with roadmap_id |

**ROADMAP → `Roadmap`** (7)
| ERD attribute | C# property | C# type | MySQL type | Null | Key / default |
|---|---|---|---|---|---|
| roadmap_id | RoadmapId | int | INT AI | NN | PK |
| title | Title | string | VARCHAR(150) | NN | |
| description | Description | string | TEXT | NN | |
| difficulty | Difficulty | `DifficultyLevel` | VARCHAR(30) | NN | |
| is_published | IsPublished | bool | TINYINT(1) | NN | default 0 |
| created_at | CreatedAt | DateTime | DATETIME(6) | NN | |
| updated_at | UpdatedAt | DateTime? | DATETIME(6) | NULL | |

**ROADMAP_OBJECTIVE → `RoadmapObjective`** (4)
| ERD attribute | C# property | C# type | MySQL type | Null | Key |
|---|---|---|---|---|---|
| objective_id | ObjectiveId | int | INT AI | NN | PK |
| roadmap_id | RoadmapId | int | INT | NN | FK → roadmap |
| objective_text | ObjectiveText | string | VARCHAR(500) | NN | |
| display_order | DisplayOrder | int | INT | NN | |

**ROADMAP_TOPIC → `RoadmapTopic`** (2)
| ERD attribute | C# property | C# type | MySQL type | Null | Key |
|---|---|---|---|---|---|
| roadmap_id | RoadmapId | int | INT | NN | PK (composite), FK → roadmap |
| topic_id | TopicId | int | INT | NN | PK (composite), FK → topic |

**TOPIC → `Topic`** (2)
| ERD attribute | C# property | C# type | MySQL type | Null | Key |
|---|---|---|---|---|---|
| topic_id | TopicId | int | INT AI | NN | PK |
| name | Name | string | VARCHAR(50) | NN | UK |

### 5. Learning content

**COURSE_WEEK → `CourseWeek`** (5)
| ERD attribute | C# property | C# type | MySQL type | Null | Key |
|---|---|---|---|---|---|
| course_week_id | CourseWeekId | int | INT AI | NN | PK |
| course_id | CourseId | int | INT | NN | FK → course; UK with week_number |
| week_number | WeekNumber | int | INT | NN | |
| title | Title | string | VARCHAR(150) | NN | |
| description | Description | string? | TEXT | NULL | |

**LESSON → `Lesson`** (9)
| ERD attribute | C# property | C# type | MySQL type | Null | Key / default |
|---|---|---|---|---|---|
| lesson_id | LessonId | int | INT AI | NN | PK |
| course_week_id | CourseWeekId | int | INT | NN | FK → course_week; UK with lesson_order |
| title | Title | string | VARCHAR(150) | NN | |
| content | Content | string | MEDIUMTEXT [G4] | NN | |
| lesson_order | LessonOrder | int | INT | NN | |
| is_required | IsRequired | bool | TINYINT(1) | NN | default 1 |
| estimated_read_minutes | EstimatedReadMinutes | int? | INT | NULL | |
| created_at | CreatedAt | DateTime | DATETIME(6) | NN | |
| updated_at | UpdatedAt | DateTime? | DATETIME(6) | NULL | |

**LEARNING_RESOURCE → `LearningResource`** (8)
| ERD attribute | C# property | C# type | MySQL type | Null | Key |
|---|---|---|---|---|---|
| resource_id | ResourceId | int | INT AI | NN | PK |
| lesson_id | LessonId | int | INT | NN | FK → lesson |
| title | Title | string | VARCHAR(150) | NN | |
| resource_type | ResourceType | `ResourceType` | VARCHAR(30) | NN | |
| resource_url | ResourceUrl | string? | VARCHAR(2048) | NULL | Video/Link |
| file_path | FilePath | string? | VARCHAR(500) | NULL | File |
| file_size_bytes | FileSizeBytes | long? | BIGINT | NULL | |
| description | Description | string? | VARCHAR(500) | NULL | |

**CODING_CHALLENGE → `CodingChallenge`** (7)
| ERD attribute | C# property | C# type | MySQL type | Null | Key |
|---|---|---|---|---|---|
| challenge_id | ChallengeId | int | INT AI | NN | PK |
| lesson_id | LessonId | int | INT | NN | FK → lesson |
| title | Title | string | VARCHAR(150) | NN | |
| description | Description | string | TEXT | NN | |
| difficulty | Difficulty | `DifficultyLevel` | VARCHAR(30) | NN | |
| starter_code | StarterCode | string? | MEDIUMTEXT [G4] | NULL | |
| expected_output | ExpectedOutput | string? | MEDIUMTEXT [G4] | NULL | the ERD marks it "needs confirmation"; implemented as drawn |

### 6. Learning progress

**LESSON_PROGRESS → `LessonProgress`** (6)
| ERD attribute | C# property | C# type | MySQL type | Null | Key |
|---|---|---|---|---|---|
| lesson_progress_id | LessonProgressId | int | INT AI | NN | PK |
| student_user_id | StudentUserId | int | INT | NN | FK → user_account |
| lesson_id | LessonId | int | INT | NN | FK → lesson |
| status | Status | `LessonProgressStatus` | VARCHAR(30) | NN | |
| started_at | StartedAt | DateTime | DATETIME(6) | NN | |
| completed_at | CompletedAt | DateTime? | DATETIME(6) | NULL | |

**CHALLENGE_PROGRESS → `ChallengeProgress`** (7)
| ERD attribute | C# property | C# type | MySQL type | Null | Key / default |
|---|---|---|---|---|---|
| challenge_progress_id | ChallengeProgressId | int | INT AI | NN | PK |
| student_user_id | StudentUserId | int | INT | NN | FK → user_account |
| challenge_id | ChallengeId | int | INT | NN | FK → coding_challenge |
| saved_code | SavedCode | string | MEDIUMTEXT [G4] | NN [G3] | |
| is_completed | IsCompleted | bool | TINYINT(1) | NN | default 0 |
| last_saved_at | LastSavedAt | DateTime | DATETIME(6) | NN | |
| completed_at | CompletedAt | DateTime? | DATETIME(6) | NULL | |

### 7. Quizzes and attempts

**QUIZ → `Quiz`** (6)
| ERD attribute | C# property | C# type | MySQL type | Null | Key / default |
|---|---|---|---|---|---|
| quiz_id | QuizId | int | INT AI | NN | PK |
| course_week_id | CourseWeekId | int | INT | NN | FK → course_week, **UK** (one quiz per week) |
| title | Title | string | VARCHAR(150) | NN | |
| instructions | Instructions | string? | TEXT | NULL | |
| pass_mark_percent | PassMarkPercent | int | INT | NN | default 50 |
| max_attempts | MaxAttempts | int? | INT | NULL | null = unlimited |

**QUIZ_QUESTION → `QuizQuestion`** (7)
| ERD attribute | C# property | C# type | MySQL type | Null | Key |
|---|---|---|---|---|---|
| question_id | QuestionId | int | INT AI | NN | PK |
| quiz_id | QuizId | int | INT | NN | FK → quiz; UK with display_order |
| question_text | QuestionText | string | TEXT | NN | |
| code_snippet | CodeSnippet | string? | TEXT | NULL | |
| marks | Marks | int | INT | NN | |
| display_order | DisplayOrder | int | INT | NN | |
| explanation | Explanation | string? | TEXT | NULL | |

**QUIZ_OPTION → `QuizOption`** (5)
| ERD attribute | C# property | C# type | MySQL type | Null | Key |
|---|---|---|---|---|---|
| option_id | OptionId | int | INT AI | NN | PK |
| question_id | QuestionId | int | INT | NN | FK → quiz_question; UK with display_order [G2] |
| option_text | OptionText | string | VARCHAR(500) | NN | |
| is_correct | IsCorrect | bool | TINYINT(1) | NN | exactly one true per question [G6] |
| display_order | DisplayOrder | int | INT | NN | |

**QUIZ_ATTEMPT → `QuizAttempt`** (9)
| ERD attribute | C# property | C# type | MySQL type | Null | Key |
|---|---|---|---|---|---|
| attempt_id | AttemptId | int | INT AI | NN | PK |
| student_user_id | StudentUserId | int | INT | NN | FK → user_account; UK (student, quiz, attempt_number) |
| quiz_id | QuizId | int | INT | NN | FK → quiz |
| attempt_number | AttemptNumber | int | INT | NN | |
| status | Status | `QuizAttemptStatus` | VARCHAR(30) | NN | |
| started_at | StartedAt | DateTime | DATETIME(6) | NN | |
| submitted_at | SubmittedAt | DateTime? | DATETIME(6) | NULL | |
| score | Score | int? | INT | NULL | null until submitted |
| max_score | MaxScore | int | INT | NN | snapshot of total marks |

**QUIZ_ATTEMPT_ANSWER → `QuizAttemptAnswer`** (6)
| ERD attribute | C# property | C# type | MySQL type | Null | Key |
|---|---|---|---|---|---|
| answer_id | AnswerId | int | INT AI | NN | PK |
| attempt_id | AttemptId | int | INT | NN | FK → quiz_attempt; UK with question_id [G2] |
| question_id | QuestionId | int | INT | NN | FK → quiz_question |
| selected_option_id | SelectedOptionId | int? | INT | NULL | FK → quiz_option (null = not answered) |
| is_correct | IsCorrect | bool | TINYINT(1) | NN [G3] | set on submit |
| marks_awarded | MarksAwarded | int | INT | NN [G3] | set on submit |

### 8. Assignments, submissions and feedback

**ASSIGNMENT → `Assignment`** (9)
| ERD attribute | C# property | C# type | MySQL type | Null | Key |
|---|---|---|---|---|---|
| assignment_id | AssignmentId | int | INT AI | NN | PK |
| course_week_id | CourseWeekId | int | INT | NN | FK → course_week, **UK** (one assignment per week) |
| title | Title | string | VARCHAR(150) | NN | |
| instructions | Instructions | string | TEXT | NN | |
| assignment_type | AssignmentType | `AssignmentType` | VARCHAR(30) | NN | |
| due_at | DueAt | DateTime? | DATETIME(6) | NULL | the ERD notes it is "missing from admin form" |
| accepted_file_types | AcceptedFileTypes | string? | VARCHAR(200) | NULL | e.g. `.html,.css,.zip,.pdf` |
| created_at | CreatedAt | DateTime | DATETIME(6) | NN | |
| updated_at | UpdatedAt | DateTime? | DATETIME(6) | NULL | |

**ASSIGNMENT_REQUIREMENT → `AssignmentRequirement`** (4)
| ERD attribute | C# property | C# type | MySQL type | Null | Key |
|---|---|---|---|---|---|
| requirement_id | RequirementId | int | INT AI | NN | PK |
| assignment_id | AssignmentId | int | INT | NN | FK → assignment |
| requirement_text | RequirementText | string | VARCHAR(500) | NN | |
| display_order | DisplayOrder | int | INT | NN | |

**SUBMISSION → `Submission`** (7)
| ERD attribute | C# property | C# type | MySQL type | Null | Key |
|---|---|---|---|---|---|
| submission_id | SubmissionId | int | INT AI | NN | PK (displayed as `SUB-00482`, formatted in the view, not stored) |
| assignment_id | AssignmentId | int | INT | NN | FK → assignment; UK (assignment, student, attempt_number) [G2] |
| student_user_id | StudentUserId | int | INT | NN | FK → user_account |
| attempt_number | AttemptNumber | int | INT | NN | |
| submission_text | SubmissionText | string? | TEXT | NULL | |
| status | Status | `SubmissionStatus` | VARCHAR(30) | NN | |
| submitted_at | SubmittedAt | DateTime | DATETIME(6) | NN | |

**SUBMISSION_FILE → `SubmissionFile`** (6)
| ERD attribute | C# property | C# type | MySQL type | Null | Key |
|---|---|---|---|---|---|
| submission_file_id | SubmissionFileId | int | INT AI | NN | PK |
| submission_id | SubmissionId | int | INT | NN | FK → submission |
| original_file_name | OriginalFileName | string | VARCHAR(255) | NN | |
| stored_file_path | StoredFilePath | string | VARCHAR(500) | NN | |
| file_size_bytes | FileSizeBytes | long | BIGINT | NN | |
| uploaded_at | UploadedAt | DateTime | DATETIME(6) | NN | |

**FEEDBACK → `Feedback`** (9)
| ERD attribute | C# property | C# type | MySQL type | Null | Key |
|---|---|---|---|---|---|
| feedback_id | FeedbackId | int | INT AI | NN | PK (displayed as `FB-00219`) |
| submission_id | SubmissionId | int | INT | NN | FK → submission, **UK** (one review per submission) |
| tutor_user_id | TutorUserId | int? | INT | NULL | FK → user_account, SET NULL on delete |
| feedback_text | FeedbackText | string | TEXT | NN | at least 20 characters on submit [G6] |
| decision | Decision | `FeedbackDecision` | VARCHAR(30) | NN [G3: ERD literal] | chosen even when saving a draft |
| required_improvement | RequiredImprovement | string? | TEXT | NULL [G3] | required when decision = NeedsImprovement |
| status | Status | `FeedbackStatus` | VARCHAR(30) | NN | |
| created_at | CreatedAt | DateTime | DATETIME(6) | NN | |
| submitted_at | SubmittedAt | DateTime? | DATETIME(6) | NULL | null while Draft |

**Attribute total:** 3+13+7+6+6 + 3+7 + 10+5 + 10+4+3+7+4+2+2 + 5+9+8+7 + 6+7 + 6+7+5+9+6 + 9+4+7+6+9 = **202**. This matches the ERD.

---

## C. Primary key mapping

| Kind | Entities | EF Core configuration |
|------|----------|-----------------------|
| Single `int`, auto-increment (29) | every entity except the three below | `HasKey(e => e.XxxId)` plus `ValueGeneratedOnAdd()`. Keys are configured explicitly because the ERD key names (e.g. `user_id` on USER_ACCOUNT) don't follow EF's `<ClassName>Id` convention. |
| Composite (3) | COURSE_TUTOR (course_id, tutor_user_id); COURSE_ROADMAP (roadmap_id, course_id); ROADMAP_TOPIC (roadmap_id, topic_id) | `HasKey(e => new { … })` in the ERD's column order, with no auto-increment |

---

## D. Foreign key mapping (43 FKs, one per relationship)

| # | FK column (child) | References | Required | ERD notation |
|---|-------------------|------------|----------|--------------|
| 1 | user_account.role_id | role.role_id | Yes | `ROLE \|\|--o{ USER_ACCOUNT` |
| 2 | email_verification.user_id | user_account.user_id | Yes | `\|\|--o{` |
| 3 | password_reset_token.user_id | user_account | Yes | `\|\|--o{` |
| 4 | activity_log.user_id | user_account | **No** | `\|o--o{` |
| 5 | course_tutor.tutor_user_id | user_account | Yes | `\|\|--o{` |
| 6 | course_tutor.course_id | course | Yes | `\|\|--o{` |
| 7 | enrolment.student_user_id | user_account | Yes | `\|\|--o{` |
| 8 | enrolment.course_id | course | Yes | `\|\|--o{` |
| 9 | discussion_post.author_user_id | user_account | Yes | `\|\|--o{` |
| 10 | discussion_post.course_id | course | Yes | `\|\|--o{` |
| 11 | discussion_post.course_week_id | course_week | **No** | `\|o--o{` |
| 12 | discussion_reply.post_id | discussion_post | Yes | `\|\|--o{` |
| 13 | discussion_reply.author_user_id | user_account | Yes | `\|\|--o{` |
| 14 | course.primary_roadmap_id | roadmap | Yes | `\|\|--o{` |
| 15 | course_learning_outcome.course_id | course | Yes | `\|\|--o{` |
| 16 | course_roadmap.course_id | course | Yes | `\|\|--o{` |
| 17 | course_roadmap.roadmap_id | roadmap | Yes | `\|\|--o{` |
| 18 | roadmap_objective.roadmap_id | roadmap | Yes | `\|\|--o{` |
| 19 | roadmap_topic.roadmap_id | roadmap | Yes | `\|\|--o{` |
| 20 | roadmap_topic.topic_id | topic | Yes | `\|\|--o{` |
| 21 | course_week.course_id | course | Yes | `\|\|--o{` |
| 22 | lesson.course_week_id | course_week | Yes | `\|\|--o{` |
| 23 | learning_resource.lesson_id | lesson | Yes | `\|\|--o{` |
| 24 | coding_challenge.lesson_id | lesson | Yes | `\|\|--o{` |
| 25 | lesson_progress.lesson_id | lesson | Yes | `\|\|--o{` |
| 26 | lesson_progress.student_user_id | user_account | Yes | `\|\|--o{` |
| 27 | challenge_progress.challenge_id | coding_challenge | Yes | `\|\|--o{` |
| 28 | challenge_progress.student_user_id | user_account | Yes | `\|\|--o{` |
| 29 | quiz.course_week_id | course_week | Yes | `\|\|--o\|` (unique FK) |
| 30 | quiz_question.quiz_id | quiz | Yes | `\|\|--o{` |
| 31 | quiz_option.question_id | quiz_question | Yes | `\|\|--o{` |
| 32 | quiz_attempt.quiz_id | quiz | Yes | `\|\|--o{` |
| 33 | quiz_attempt.student_user_id | user_account | Yes | `\|\|--o{` |
| 34 | quiz_attempt_answer.attempt_id | quiz_attempt | Yes | `\|\|--o{` |
| 35 | quiz_attempt_answer.question_id | quiz_question | Yes | `\|\|--o{` |
| 36 | quiz_attempt_answer.selected_option_id | quiz_option | **No** | `\|o--o{` |
| 37 | assignment.course_week_id | course_week | Yes | `\|\|--o\|` (unique FK) |
| 38 | assignment_requirement.assignment_id | assignment | Yes | `\|\|--o{` |
| 39 | submission.assignment_id | assignment | Yes | `\|\|--o{` |
| 40 | submission.student_user_id | user_account | Yes | `\|\|--o{` |
| 41 | submission_file.submission_id | submission | Yes | `\|\|--o{` |
| 42 | feedback.submission_id | submission | Yes | `\|\|--o\|` (unique FK) |
| 43 | feedback.tutor_user_id | user_account | **No** | `\|o--o{` |

FK constraint names follow the pattern `fk_<child>_<parent>_<column>`, so MySQL error messages point to the ERD relationship.

---

## E. Navigation property mapping

Navigation properties are C#-only, never columns. Collections are `ICollection<T>` initialised to empty lists. Required references use `= null!`.

| Class | Reference navigations (→ one) | Collection navigations (→ many) |
|-------|-------------------------------|---------------------------------|
| Role | | `Users` (UserAccount) |
| UserAccount | `Role` | `EmailVerifications`, `PasswordResetTokens`, `ActivityLogs`, `CourseTutors`, `Enrolments`, `DiscussionPosts`, `DiscussionReplies`, `LessonProgresses`, `ChallengeProgresses`, `QuizAttempts`, `Submissions`, `FeedbackGiven` (Feedback via tutor_user_id) |
| EmailVerification | `User` | |
| PasswordResetToken | `User` | |
| ActivityLog | `User?` | |
| CourseTutor | `Course`, `Tutor` (UserAccount) | |
| Enrolment | `Student` (UserAccount), `Course` | |
| DiscussionPost | `Course`, `CourseWeek?`, `Author` (UserAccount) | `Replies` |
| DiscussionReply | `Post`, `Author` | |
| Course | `PrimaryRoadmap` (Roadmap) | `CourseRoadmaps`, `LearningOutcomes`, `Weeks`, `CourseTutors`, `Enrolments`, `DiscussionPosts` |
| CourseLearningOutcome | `Course` | |
| CourseRoadmap | `Roadmap`, `Course` | |
| Roadmap | | `PrimaryCourses` (Course via primary_roadmap_id), `CourseRoadmaps`, `Objectives`, `RoadmapTopics` |
| RoadmapObjective | `Roadmap` | |
| RoadmapTopic | `Roadmap`, `Topic` | |
| Topic | | `RoadmapTopics` |
| CourseWeek | `Course`, `Quiz?`, `Assignment?` | `Lessons`, `DiscussionPosts` |
| Lesson | `CourseWeek` | `LearningResources`, `CodingChallenges`, `LessonProgresses` |
| LearningResource | `Lesson` | |
| CodingChallenge | `Lesson` | `ChallengeProgresses` |
| LessonProgress | `Student`, `Lesson` | |
| ChallengeProgress | `Student`, `Challenge` (CodingChallenge) | |
| Quiz | `CourseWeek` | `Questions`, `Attempts` |
| QuizQuestion | `Quiz` | `Options`, `AttemptAnswers` |
| QuizOption | `Question` | `SelectedInAnswers` (QuizAttemptAnswer) |
| QuizAttempt | `Student`, `Quiz` | `Answers` |
| QuizAttemptAnswer | `Attempt`, `Question`, `SelectedOption?` | |
| Assignment | `CourseWeek` | `Requirements`, `Submissions` |
| AssignmentRequirement | `Assignment` | |
| Submission | `Assignment`, `Student`, `Feedback?` | `Files` (SubmissionFile) |
| SubmissionFile | `Submission` | |
| Feedback | `Submission`, `Tutor?` (UserAccount) | |

---

## F. Relationship mapping

| Type | Count | ERD representation | EF Core |
|------|-------|--------------------|---------|
| One-to-many, required parent | 36 (includes the 6 join-table FKs) | `\|\|--o{` | `HasOne(c => c.Parent).WithMany(p => p.Children).HasForeignKey(c => c.ParentId).IsRequired()` |
| One-to-many, optional parent | 4 (#4, #11, #36, #43) | `\|o--o{` | the same, with `IsRequired(false)` and a nullable FK |
| One-to-zero-or-one | 3 (#29 Quiz, #37 Assignment, #42 Feedback) | `\|\|--o\|` and a UK on the FK | `HasOne(p => p.Child).WithOne(c => c.Parent).HasForeignKey<Child>(c => c.ParentId)`, which creates a unique index on the FK |
| Many-to-many | 3 | a join entity with a composite PK | explicit join class with two one-to-many relationships (not EF skip-navigations), so every ERD entity has a class and payload columns such as `assigned_at` and `sequence_position` are kept |
| | | USER_ACCOUNT ↔ COURSE (tutors) through COURSE_TUTOR (#5, #6) | |
| | | ROADMAP ↔ COURSE through COURSE_ROADMAP (#16, #17) | |
| | | ROADMAP ↔ TOPIC through ROADMAP_TOPIC (#19, #20) | |
| Associative entity with a surrogate key | 1 | ENROLMENT (#7, #8): student ↔ course | two one-to-many relationships, exactly as drawn |

**Relationships that need explicit disambiguation:**
- COURSE ↔ ROADMAP appears twice: as the *primary roadmap* (#14) and *through COURSE_ROADMAP* (#16, #17). Each gets explicit navigations (`PrimaryRoadmap`/`PrimaryCourses` vs `CourseRoadmaps`).
- USER_ACCOUNT is the parent in 13 relationships and plays several roles (student, tutor, author, feedback tutor, log actor). Every one is configured with an explicit navigation pair, so EF can't guess wrongly.

---

## G. Constraint mapping

| ERD constraint | Source in ERD | Implementation |
|----------------|---------------|----------------|
| 29 single PKs, 3 composite PKs | `PK` markers | `HasKey` |
| 43 FKs | relationships | `HasForeignKey` with `OnDelete` (see K) |
| 7 single-column UKs | `UK` markers or "UK" notes | `HasIndex(x).IsUnique()` |
| 5 composite uniques | "unique per …" notes | `HasIndex(new { … }).IsUnique()` |
| 6 additional uniques | **[G2]** | `HasIndex(new { … }).IsUnique()`, only if approved |
| NOT NULL / NULL | "nullable" notes; **[G3]** for 2 FEEDBACK fields | nullable reference types plus `IsRequired()` |
| Defaults: failed_login_count 0; is_pinned, is_closed, is_published ×2, is_completed = 0; is_required = 1; pass_mark_percent 50 | "default …" notes | `HasDefaultValue(...)` plus a C# initialiser (see K.4) |
| FEEDBACK.tutor_user_id SET NULL | "nullable after tutor deleted" | `OnDelete(DeleteBehavior.SetNull)` |
| Exactly one correct option; `required_improvement` if NeedsImprovement; `feedback_text` ≥ 20; url/file by resource type; expiries; lockout | ERD notes | **application validation [G6]**. These aren't practical as MySQL constraints. |

No MySQL `CHECK` constraints are planned, because the ERD doesn't state any numeric ranges. Ranges such as `pass_mark_percent` 0–100 and `marks > 0` are enforced by form validation.

---

## H. Enum mapping

Every ERD `varchar` whose note lists fixed values (`A | B | C`) becomes a C# `enum`. It is stored as its **name** in `VARCHAR(30)` through `HasConversion<string>()`, so the column still holds the exact ERD text (`'PendingVerification'`) and the database stays readable in MySQL Workbench.

| C# enum | Values (ERD text, unchanged) | Used by |
|---------|------------------------------|---------|
| `AccountStatus` | PendingVerification, Active, Inactive | USER_ACCOUNT.account_status |
| `EnrolmentStatus` | InProgress, Completed | ENROLMENT.status |
| `LessonProgressStatus` | InProgress, Completed | LESSON_PROGRESS.status |
| `DifficultyLevel` | Beginner, Intermediate, Advanced | ROADMAP.difficulty, COURSE.difficulty, CODING_CHALLENGE.difficulty |
| `ResourceType` | Video, Link, File | LEARNING_RESOURCE.resource_type |
| `QuizAttemptStatus` | InProgress, Submitted | QUIZ_ATTEMPT.status |
| `AssignmentType` | FileUpload, FileUploadAndText, CodeUpload | ASSIGNMENT.assignment_type |
| `SubmissionStatus` | Submitted, UnderReview, NeedsImprovement, Approved | SUBMISSION.status |
| `FeedbackDecision` | Approved, NeedsImprovement | FEEDBACK.decision |
| `FeedbackStatus` | Draft, Submitted | FEEDBACK.status |

ENROLMENT and LESSON_PROGRESS have identical values but are separate enums, so each ERD attribute traces to its own type and either can change independently.

`ROLE.name` is **not** an enum: roles are rows in the ROLE table, as the ERD defines. A constants class, `RoleNames` (`Student`, `Tutor`, `Administrator`), holds the names used by `[Authorize(Roles = …)]` so they aren't typed out repeatedly. Guest is not stored, as the ERD notes; a guest is simply an unauthenticated request.

---

## J. Index and unique-constraint plan

| Index | Columns | Type | Basis |
|-------|---------|------|-------|
| ux_role_name | role(name) | Unique | ERD UK |
| ux_user_account_email | user_account(email) | Unique | ERD UK |
| ux_password_reset_token_token_hash | password_reset_token(token_hash) | Unique | ERD UK |
| ux_topic_name | topic(name) | Unique | ERD UK |
| ux_quiz_course_week_id | quiz(course_week_id) | Unique | ERD "UK – one quiz per week" |
| ux_assignment_course_week_id | assignment(course_week_id) | Unique | ERD "UK – one assignment per week" |
| ux_feedback_submission_id | feedback(submission_id) | Unique | ERD "UK – one review per submission" |
| ux_course_roadmap_roadmap_sequence | course_roadmap(roadmap_id, sequence_position) | Unique | ERD "unique per roadmap" |
| ux_course_week_course_week_number | course_week(course_id, week_number) | Unique | ERD "unique per course" |
| ux_lesson_week_order | lesson(course_week_id, lesson_order) | Unique | ERD "unique per week" |
| ux_quiz_question_quiz_order | quiz_question(quiz_id, display_order) | Unique | ERD "unique per quiz" |
| ux_quiz_attempt_student_quiz_number | quiz_attempt(student_user_id, quiz_id, attempt_number) | Unique | ERD "unique per student + quiz" |
| ux_enrolment_student_course | enrolment(student_user_id, course_id) | Unique | **[G2]** |
| ux_lesson_progress_student_lesson | lesson_progress(student_user_id, lesson_id) | Unique | **[G2]** |
| ux_challenge_progress_student_challenge | challenge_progress(student_user_id, challenge_id) | Unique | **[G2]** |
| ux_submission_assignment_student_number | submission(assignment_id, student_user_id, attempt_number) | Unique | **[G2]** |
| ux_quiz_attempt_answer_attempt_question | quiz_attempt_answer(attempt_id, question_id) | Unique | **[G2]** |
| ux_quiz_option_question_order | quiz_option(question_id, display_order) | Unique | **[G2]** |
| FK indexes | every FK column that isn't already the first column of a PK or unique index | Non-unique | Required by InnoDB for foreign keys; EF creates them automatically |

No other performance indexes are planned. At university-project data volumes they can't be justified, and they would be added later only if a measured query needs one.

---

## K. EF Core Fluent API configuration plan

`ApplicationDbContext.OnModelCreating` will contain, in this order:

1. **Naming loop.** For every entity: table name → snake_case, and each property's column → snake_case. The result is verified against the ERD when the migration is reviewed.
2. **Section blocks** in ERD order (Accounts → Membership → Discussions → Catalogue → Content → Progress → Quizzes → Assignments). Each entity block contains:
   - `HasKey(...)`
   - `Property(...).HasMaxLength(n)` or `HasColumnType("text" | "mediumtext")` [G4]
   - `Property(...).IsRequired()` or `IsRequired(false)` [G3]
   - `HasConversion<string>().HasMaxLength(30)` for enum properties
   - `HasIndex(...).IsUnique().HasDatabaseName(...)` (see J)
   - one `HasOne/WithMany` or `HasOne/WithOne` per relationship, with `HasForeignKey`, `IsRequired`, `OnDelete` [G5] and `HasConstraintName`
3. **Delete behaviours** follow G5: Restrict for academic records, Cascade inside content hierarchies, SetNull for `feedback.tutor_user_id` and `discussion_post.course_week_id`.
4. **Defaults.** `HasDefaultValue(x)` is combined with a matching C# initialiser and `HasSentinel(x)`. Without the sentinel, EF treats `false` or `0` as "not set" and lets MySQL's default replace it. That would make it impossible to save `is_required = false` or `pass_mark_percent = 0`.
5. **UTC timestamps.** A value converter sets `DateTimeKind.Utc` on values read back from MySQL, so the times display correctly on the verification-success page ("Verified 24 Sep 2026, 10:14") and on dashboards.
6. **Not configured:** no global query filters, no soft-delete framework, no repositories, no interceptors.

Registration in `Program.cs` (Stage 6):

```
builder.Services.AddDbContext<ApplicationDbContext>(o =>
    o.UseMySql(builder.Configuration.GetConnectionString("DefaultConnection"),
               <explicit MySqlServerVersion, see risk R2>));
```

The connection string comes from `appsettings.Development.json` (git-ignored) or `dotnet user-secrets`, never from `appsettings.json`.

---

## M. Requirements → ERD → C# implementation traceability

**Basis:** **W** = verified against a wireframe (authentication.pdf). **R** = based on a requirement only (Proposal/Brief/Backlog); page-level details are unconfirmed until the wireframe is provided (see G9). Controller names are *planned*.

| # | Requirement | Source | Basis | ERD entities → C# classes | Planned controller | CRUD | Role |
|---|-------------|--------|-------|---------------------------|--------------------|------|------|
| 1 | Guests browse published roadmaps and courses | Proposal §2, §5 | R | ROADMAP, ROADMAP_OBJECTIVE, ROADMAP_TOPIC, TOPIC, COURSE, COURSE_ROADMAP, COURSE_LEARNING_OUTCOME → `Roadmap`, `RoadmapObjective`, `RoadmapTopic`, `Topic`, `Course`, `CourseRoadmap`, `CourseLearningOutcome` | `RoadmapsController`, `CoursesController` | R | Guest, all |
| 2 | Register a student account | Brief; auth p2 | **W** | ROLE, USER_ACCOUNT → `Role`, `UserAccount` | `AccountController.Register` | C | Guest |
| 3 | Registration success, account pending verification | auth p3 | **W** | USER_ACCOUNT.account_status | `AccountController.RegisterSuccess` | R | Guest |
| 4 | Verify email with a 6-digit code (15 min), resend | auth p3, p7 | **W** | EMAIL_VERIFICATION, USER_ACCOUNT.email_verified_at → `EmailVerification` | `AccountController.VerifyEmail` | C, U | Guest |
| 5 | Log in, remember me, lock out after 5 failures for 15 min | auth p1, p6 | **W** | USER_ACCOUNT.failed_login_count, lockout_end_at, last_login_at | `AccountController.Login` | R, U | Guest |
| 6 | Forgot/reset password (30 min), expired link, sign out other sessions | auth p4, p5 | **W** | PASSWORD_RESET_TOKEN, USER_ACCOUNT.security_stamp → `PasswordResetToken` | `AccountController.ForgotPassword/ResetPassword` | C, U | Guest |
| 7 | Student dashboard showing enrolled courses, progress, quiz results and assignment status | Proposal §5 | R | ENROLMENT, LESSON_PROGRESS, QUIZ_ATTEMPT, SUBMISSION, FEEDBACK | `StudentController.Dashboard` | R | Student |
| 8 | Enrol in a course | Proposal §2 | R | ENROLMENT → `Enrolment` | `EnrolmentsController.Create` | C, U | Student |
| 9 | Study weekly lessons and resources (multimedia) | Proposal §2, §5; Brief | R | COURSE_WEEK, LESSON, LEARNING_RESOURCE → `CourseWeek`, `Lesson`, `LearningResource` | `LearnController` | R | Enrolled student |
| 10 | Progress updated as lessons are completed | Proposal §5 | R | LESSON_PROGRESS → `LessonProgress` | `LearnController.CompleteLesson` | C, U | Student |
| 11 | Coding challenges with auto-saved code | Proposal §1; ERD note | R | CODING_CHALLENGE, CHALLENGE_PROGRESS → `CodingChallenge`, `ChallengeProgress` | `ChallengesController` | C, U | Student |
| 12 | Weekly quiz unlocked after required lessons, immediate score | Proposal §2, §5 | R | QUIZ, QUIZ_QUESTION, QUIZ_OPTION, QUIZ_ATTEMPT, QUIZ_ATTEMPT_ANSWER, LESSON.is_required | `QuizzesController` | C, U, R | Student |
| 13 | Submit and resubmit a weekly assignment (text and files) | Proposal §2, §5 | R | ASSIGNMENT, ASSIGNMENT_REQUIREMENT, SUBMISSION, SUBMISSION_FILE | `SubmissionsController` | C, R | Student |
| 14 | Tutor reviews submissions for assigned courses; approves or requests improvement; next week unlocks | Proposal §2, §5 | R | COURSE_TUTOR, SUBMISSION, FEEDBACK → `CourseTutor`, `Submission`, `Feedback` | `TutorReviewsController` | C, R, U | Tutor |
| 15 | Course discussions | Proposal §2, §5; Brief | R | DISCUSSION_POST, DISCUSSION_REPLY | `DiscussionsController` | C, R, U (per G7) | Student, Tutor |
| 16 | Admin manages roadmaps, courses, weeks, lessons, resources, challenges, quizzes, questions, options, assignments and requirements | Proposal §2, §5; Brief | R | all catalogue, content and assessment entities | `Admin*` controllers | **C, R, U, D** | Administrator |
| 17 | Admin manages user data (view, update status/role, delete when there is no history) | Proposal §2, §5 | R | USER_ACCOUNT, ROLE | `AdminUsersController` | C, R, U, D (G5 rule 3) | Administrator |
| 18 | Admin assigns tutors to courses | Backlog #5 | R | COURSE_TUTOR | `AdminCourseTutorsController` | C, R, D | Administrator |
| 19 | Activity log | ERD only | R | ACTIVITY_LOG → `ActivityLog` | written through a helper; read on the admin dashboard (G8) | C, R | System / Administrator |

### CRUD matrix by role (only operations the requirements support)

| Entity | Guest | Student | Tutor | Administrator |
|--------|-------|---------|-------|---------------|
| USER_ACCOUNT | C (register) | R, U (own profile, Backlog #1) | R, U (own profile) | C, R, U, D |
| EMAIL_VERIFICATION, PASSWORD_RESET_TOKEN | C, U (system) | | | |
| ROADMAP (+ OBJECTIVE, TOPIC links), TOPIC | R (published) | R | R | C, R, U, D |
| COURSE (+ OUTCOME, COURSE_ROADMAP) | R (published) | R | R | C, R, U, D |
| COURSE_TUTOR | | | R (own) | C, R, D |
| COURSE_WEEK, LESSON, LEARNING_RESOURCE, CODING_CHALLENGE | | R (enrolled) | R (assigned) | C, R, U, D |
| QUIZ, QUIZ_QUESTION, QUIZ_OPTION | | R (enrolled) | R (assigned) | C, R, U, D |
| ASSIGNMENT, ASSIGNMENT_REQUIREMENT | | R (enrolled) | R (assigned) | C, R, U, D |
| ENROLMENT | | C, R, U (system: status and access time) | R (own courses) | R |
| LESSON_PROGRESS, CHALLENGE_PROGRESS | | C, R, U (own) | R (own courses) | R |
| QUIZ_ATTEMPT, QUIZ_ATTEMPT_ANSWER | | C, R, U (own) | R (own courses) | R |
| SUBMISSION, SUBMISSION_FILE | | C, R (own) | R, U (status) | R |
| FEEDBACK | | R (own) | C, R, U (while Draft) | R |
| DISCUSSION_POST / REPLY | | C, R, U (own post) | C (reply), R, U (pin/close) | R |
| ACTIVITY_LOG | | | | R (system writes) |

---

## N. Assignment marking-criteria evidence plan

None of this is implemented yet. The table shows *where the evidence will come from*.

### Individual criteria (Implementation, 50%)

| Criterion | Planned evidence | Location (planned) | How it will be demonstrated |
|-----------|------------------|--------------------|-----------------------------|
| Web Page Layout & Appearance | A shared `_Layout` with role-aware navigation; pages that follow the wireframes; external `site.css` plus page-specific CSS | `Views/Shared/_Layout.cshtml`, `wwwroot/css/` | Side-by-side wireframe vs. screenshot for the auth pages (W), then the other pages as wireframes arrive |
| User Authentication & Authorization | Cookie authentication on USER_ACCOUNT; password hashing; lockout; email verification; reset tokens; the security stamp signs out other sessions; `[Authorize(Roles=…)]` on the Student, Tutor and Admin areas; data-level checks through ENROLMENT and COURSE_TUTOR | `Controllers/AccountController.cs`, `Program.cs` | Log in as each seeded role; open an admin URL as a student and get Access Denied; a tutor can't open an unassigned course's submission; the lockout message appears after the 5th failure |
| Dynamic Content | Every catalogue, lesson, quiz, dashboard and discussion page is queried through EF Core; no hard-coded course data in views | `Controllers/*`, `Views/*` | Admin edits a course title and the public page changes immediately; the student dashboard progress updates after a lesson is completed |
| Insert, Update & Delete Records | Admin CRUD on roadmaps, courses, weeks, lessons, resources, quizzes and assignments; student inserts (enrolment, attempts, submissions); tutor inserts and updates (feedback, submission status) | `Controllers/Admin*` | Live create → edit → delete of a lesson; a delete blocked by the Restrict rule shows the "Unpublish instead" message (G5) |
| Form Validation, Navigation & Usability | Server-side `ModelState` validation on view models (`[Required]`, `[EmailAddress]`, `[StringLength]` matching G4, `[Compare]`) plus unobtrusive client validation; unique email checked against the database; error summaries as in auth p2; breadcrumbs Course → Week → Lesson | `Models/ViewModels/*`, views | Reproduce the "Please fix 4 errors" state from auth p2; disable JavaScript and show that the server still rejects the form |

### Group documentation (50%)

| Report section | Evidence produced by this work |
|----------------|-------------------------------|
| Introduction | Objectives and scope traced to features (table M) |
| Storyboard & Design Documents | Wireframes (authentication.pdf, plus the others to come) with W/R traceability (G9) |
| Design & Modelling | Final ERD; this mapping (sections A–L); design decisions G2–G8 with their rationale; normalisation (join tables for M:N, lookup table for roles, no repeated groups) |
| Implementation & Discussion | Fluent API excerpts (naming loop, delete rules, unique indexes); the generated migration SQL; validation code; authorization attributes; SQL-injection protection through parameterised EF queries |
| Conclusion / Formatting | Lessons learned (e.g. Restrict vs Cascade, sentinel defaults); future enhancements (e.g. the `expected_output` auto-marking the ERD marks "needs confirmation") |
