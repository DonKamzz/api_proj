# RaceDay — Part 1: System Planning & Database

**Module:** PROG6212/w — Programming 2B
**Assessment:** Portfolio of Evidence — Part 1
**Project:** RaceDay Event Management Platform
**Database:** Microsoft SQL Server

---

## 1. Project Overview

RaceDay is a full-stack, API-driven event management platform for the South African road running, walking, and cycling community.

It replaces paper-based registration and spreadsheet-driven event management with a centralised digital platform. **Event Organisers** manage events, categories, routes, enrolments, and participant results, while **Participants** register accounts, browse upcoming events, enrol in races, and track their results history.

> **Note:** Part 1 contains no application code. It covers system planning, database design, and API planning before development begins in Part 2.

---

## 2. At a Glance

| # | Area | Summary |
|--:|---|---|
| 1 | System Overview | Full-stack, API-driven event management platform for running, walking, and cycling events. |
| 2 | Problem Statement | Paper forms and spreadsheets cause duplicate registrations, data-entry errors, and lost information. |
| 3 | Proposed Solution | A centralised digital platform for events, participants, categories, enrolments, routes, and results. |
| 4 | System Objectives | Simplify event administration, improve registration accuracy, and give organisers and participants easy access to their data. |
| 5 | User Roles | Two roles: **Organiser** and **Participant**. |
| 6 | Database | SQL Server database, **RaceDayDB**, with six entities: Users, Events, Categories, Routes, Enrolments, Results. |
| 7 | Relationships | Primary keys, foreign keys, and cardinality connect all six entities (see Section 4). |
| 8 | API Planning | Endpoints grouped into Authentication, User Profile, Events, Categories, Enrolments, and Results (see `docs/api_endpoint_plan.md`). |
| 9 | Security & Integrity | Planned for Part 2: authentication, role-based authorisation, password hashing, input validation, and database constraints. |
| 10 | Sample Data | 2 Organisers, 2 Participants, 3 Events, plus categories, routes, and enrolments. |
| 11 | CI/CD | GitHub Actions validates that all required planning documents are present on every push. |
| 12 | Future Development | Database is verifiable in SSMS today; payments, GPS routes, live weather, and QR check-in are candidates for later phases. |

---

## 3. User Roles

### Organiser
- Create, edit, and delete events
- Create and manage categories
- View event enrolments
- Capture participant results
- Manage event route information

### Participant
- Register an account and log in
- Update their profile
- Browse upcoming events and view categories and route information
- Enrol in events
- View their own enrolments and results history

---

## 4. Database Design

The RaceDay database contains six entities:

| Entity | Purpose |
|---|---|
| **Users** | Stores Organiser and Participant accounts |
| **Events** | Stores sporting event information |
| **Categories** | Stores race categories belonging to events |
| **Routes** | Stores route information for events |
| **Enrolments** | Connects participants to events and categories |
| **Results** | Stores participant race results |

### Relationships

| Relationship | Cardinality |
|---|---|
| Users → Events | 1 : Many |
| Events → Categories | 1 : Many |
| Events → Routes | 1 : Many |
| Users → Enrolments | 1 : Many |
| Events → Enrolments | 1 : Many |
| Categories → Enrolments | 1 : Many |
| Enrolments → Results | 1 : 0..1 |

---

## 5. Database Rules

**Primary keys** — every table has its own: `UserID`, `EventID`, `CategoryID`, `RouteID`, `EnrolmentID`, `ResultID`.

**Foreign keys** — connect related tables and maintain referential integrity.

**Unique enrolment** — a participant cannot enrol in the same event twice: `(ParticipantID, EventID)` is unique.

**Unique result** — each enrolment has at most one result: `EnrolmentID` is unique in the Results table.

---

## 6. API Endpoint Planning

A representative sample of the full plan — see `docs/api_endpoint_plan.md` for every endpoint, including request bodies and failure responses.

| Area | Method | Endpoint | Purpose |
|---|---|---|---|
| Authentication | POST | `/api/auth/register` | Register a user |
| Authentication | POST | `/api/auth/login` | Log in |
| Profile | GET | `/api/users/me` | View own profile |
| Profile | PUT | `/api/users/me` | Update profile |
| Events | GET | `/api/events` | View events |
| Events | POST | `/api/events` | Create event |
| Events | PUT | `/api/events/{id}` | Update event |
| Events | DELETE | `/api/events/{id}` | Delete event |
| Categories | GET | `/api/events/{id}/categories` | View categories |
| Categories | POST | `/api/events/{id}/categories` | Create category |
| Enrolments | POST | `/api/events/{id}/enrolments` | Enrol in event |
| Enrolments | GET | `/api/users/me/enrolments` | View own enrolments |
| Results | POST | `/api/events/{id}/results` | Capture a result |
| Results | GET | `/api/users/me/results` | View own results |

---

## 7. Documentation Files

```text
docs/
├── raceday_erd.png                Final Entity Relationship Diagram
├── raceday_erd.svg                Editable/source ERD
├── api_endpoint_plan.md           Full API endpoint plan
├── RaceDay_API_Endpoint_Plan.docx Full API endpoint plan (Word format)
├── raceday_schema.sql             SQL Server database creation and sample data
└── verify_database.sql            Verification queries — run after raceday_schema.sql
```

---

## 8. SQL Server Database

The database is called `RaceDayDB`. The script creates:

- The database itself
- Users, Events, Categories, Routes, Enrolments, and Results tables
- Primary keys, foreign keys, and unique/check constraints
- Sample seed data

---

## 9. Sample Data

| Data | Quantity |
|---|--:|
| Organisers | 2 |
| Participants | 2 |
| Events | 3 |
| Categories | Multiple (per event) |
| Routes | Multiple (per event) |
| Enrolments | Multiple |
| Results | Sample result |

This data lets every relationship in the schema be verified before application development begins.

---

## 10. Database Testing

Run `docs/verify_database.sql` in SSMS immediately after `raceday_schema.sql`. It checks:

1. All six tables exist (`INFORMATION_SCHEMA.TABLES`)
2. Row counts per table, to confirm seed data loaded
3. Full contents of every table
4. Every primary key, foreign key, and constraint that was created
5. A full relationship join — Participant → Event → Category → Enrolment Status → Result
6. Each Event's Organiser resolves correctly
7. Each Event's Routes
8–9. Two commented-out negative tests — uncomment either one to prove a constraint actually rejects bad data (a duplicate enrolment, or an invalid Role) — good evidence to show live in the video walkthrough.

---

## 11. Security Considerations (Planned for Part 2)

- Password hashing
- User authentication and role-based authorisation
- Input validation and parameterised queries
- Organiser permission checks, so an Organiser can only manage their own events and results
- Participant-scoped data access, so a Participant can only see their own enrolments and results

---

## 12. Data Privacy

RaceDay stores personal information: names, email addresses, contact numbers, account details, and event participation history. Production development should account for the **Protection of Personal Information Act (POPIA)** when collecting, storing, and processing participant data.

---

## 13. CI/CD

```text
.github/workflows/validate-docs.yml
```

Runs on every push and confirms the required files exist:

```text
README.md
docs/raceday_erd.png
docs/raceday_erd.svg
docs/api_endpoint_plan.md
docs/raceday_schema.sql
docs/verify_database.sql
```

**CI screenshot:** *insert a screenshot of the green GitHub Actions run here before submission.*

---

## 14. How to Run the SQL Script

1. Open **SQL Server Management Studio (SSMS)**.
2. Connect to a local or clean SQL Server instance.
3. Open `docs/raceday_schema.sql`.
4. Execute the script (**F5**).
5. Confirm the `RaceDayDB` database was created.
6. Open `docs/verify_database.sql` and run it to confirm the tables, constraints, and sample data (see Section 10).

---

## 15. ERD

```text
docs/raceday_erd.png
docs/raceday_erd.svg
```

Shows all six entities, their attributes, primary keys, foreign keys, relationships, and cardinality. The SQL schema was designed to match the ERD exactly.

---

## 16. Design Decisions

**Single Users table** — Organisers and Participants share one table, since both roles need the same core fields (name, email, password, contact details). Their different permissions are enforced at the API/application layer, not the schema.

**Routes as a dedicated entity** — included because route information is central to the platform, and it leaves room for future GPS, elevation, and weather data without changing the core schema.

**Enrolments as a junction table** — resolves the many-to-many relationship between Participants and Events, while also storing enrolment-specific data like status and date.

---

## 17. Assumptions

1. Every event has an organiser.
2. Every category belongs to an event.
3. Participants must have an account before enrolling.
4. A participant can only enrol once per event.
5. Each enrolment is associated with one category.
6. Results can only be recorded against valid enrolments.
7. Organisers manage their own events only.
8. Authentication and authorisation are implemented in Part 2.
9. SQL Server is the database management system.
10. The application communicates with the database through the API, not directly.

---

## 18. Future Enhancements

Online payments, weather API integration, GPS route mapping, live race tracking and results, QR-code check-in, email/SMS notifications, a mobile app, organiser dashboards, advanced reporting, and digital race certificates — none of which require changing the core database design.

---

## 19. Evidence Required for Submission

| Evidence | Requirement |
|---|---|
| ERD | Screenshot/image of the completed ERD |
| Database | Screenshot of RaceDayDB in SSMS |
| Tables | Evidence showing all six tables |
| Sample Data | Evidence showing inserted records |
| SQL Execution | Screenshot showing successful script execution |
| API Plan | Endpoint planning document |
| CI/CD | Screenshot of the green GitHub Actions workflow |
| Video | Unlisted YouTube walkthrough |
| Repository | GitHub project repository |

---

## 20. Video Walkthrough

Cover, in order: an introduction to RaceDay and the problem it solves; the two user roles; a walkthrough of the ERD, relationships, and entities; the API endpoint plan; a live run of the SQL script in SSMS with sample data verification; and a demonstration of the GitHub repository and the green Actions workflow.

**YouTube link:** *insert the unlisted walkthrough link here before submission.*

---

## Conclusion

Part 1 establishes the foundation for RaceDay: system requirements, user roles, database structure, API endpoints, security considerations, and testing approach. The six entities — **Users, Events, Categories, Routes, Enrolments, and Results** — provide a structured foundation for managing sporting events and participant information. The ERD, API endpoint plan, and SQL script are designed to work together as a clear blueprint for the application development that follows in **Part 2**.
