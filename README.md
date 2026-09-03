# RaceDay — Part 1: System Planning and Database

**Module:** PROG6212/w — Programming 2B
**Assessment:** Portfolio of Evidence, Part 1 (System Planning & Database)

## System Description

RaceDay is a full-stack, API-driven event management platform built for the South African road running, walking, and cycling community. It replaces the paper-based registration and spreadsheet-driven processes many local events still rely on. Event Organisers can create and manage events, define age/distance categories, and capture participant results, while Participants can browse upcoming events, enrol using a chosen category, track their personal enrolment and results history, and view route information to prepare for race day.

This part of the POE contains **no application code**. It documents the planning work completed before development begins in Part 2: the database design (ERD), the API surface (endpoint plan), and the database creation script.

## Roles

| Role | Capabilities |
|---|---|
| **Organiser** | Create, edit, and delete events; manage event categories; capture participant results; view all enrolments for their events. |
| **Participant** | Register an account; browse events; enrol in an event by selecting a category; view their own enrolments; track their own results. |

## /docs Folder Contents

| File | Section | Description |
|---|---|---|
| `docs/raceday_erd.png` (+ `.svg` source) | Section A | Entity Relationship Diagram — 6 entities (Users, Events, Categories, Routes, Enrolments, Results) with primary keys, foreign keys, and cardinality. |
| `docs/api_endpoint_plan.md` | Section B | Full API endpoint plan covering Authentication, User Profile, Events, Categories, Event Enrolments, and Results. |
| `docs/raceday_schema.sql` | Section C | SQL Server script — creates all six tables with constraints and seeds realistic sample data (2 Organisers, 2 Participants, 3 Events, categories, routes, enrolments, a sample result). |

## Design Notes

- The ERD includes a **Routes** entity in addition to the required core entities, to support the platform's "live weather and route information" feature described in the background brief — this also lifts the design past the minimum six-entity requirement with a genuinely useful table rather than a padding one.
- `Users` uses a single table with a `Role` column (`Organiser` / `Participant`) rather than two separate tables, since both roles share the same core attributes (name, email, password, contact details) and only differ in behaviour, which is enforced at the API/application layer in Part 2 — not at the schema level.
- An Enrolment is unique per `(ParticipantID, EventID)` — a Participant can only enrol once per event — and a Result is unique per `EnrolmentID`, reflecting that at most one result is captured per enrolment.
- The SQL script was written to match the ERD exactly; there are no deliberate differences between the two.

## CI/CD

A GitHub Actions workflow (`.github/workflows/validate-docs.yml`) runs on every push and confirms that `/docs` contains the ERD, endpoint plan, and SQL script, and that a root-level `README.md` exists.

**CI screenshot:** _add a screenshot of the green Actions run here before submission._

## Video

**YouTube (unlisted) walkthrough of the planning documents, ERD decisions, endpoint plan choices, and a live run of the SQL script in SSMS:** _add link here before submission._

## Setup / How to Run the SQL Script

1. Open SQL Server Management Studio (SSMS) and connect to a local or clean SQL Server instance.
2. Open `docs/raceday_schema.sql`.
3. Execute the script (F5). It will create the `RaceDayDB` database, all six tables with constraints, and seed the sample data described above.
4. Verify with `SELECT * FROM RaceDayDB.dbo.Events;` (and the other tables) that the seed data loaded correctly.

## AI Disclosure

Parts of this planning documentation (ERD layout, endpoint plan drafting, and SQL script scaffolding) were produced with the assistance of an AI tool and then reviewed, adjusted, and verified by the student before submission, in line with the POE's AI disclosure instruction.
