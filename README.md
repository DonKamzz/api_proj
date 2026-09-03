# RaceDay — Part 1: System Planning & Database

**Module:** PROG6212/w — Programming 2B
**Assessment:** Portfolio of Evidence — Part 1
**Project:** RaceDay Event Management Platform
**Database:** Microsoft SQL Server

---

## 1. Project Overview

RaceDay is a full-stack, API-driven event management platform designed for the South African road running, walking, and cycling community.

The system replaces paper-based registration and spreadsheet-driven event management with a centralised digital platform.

**Event Organisers** can manage events, categories, routes, enrolments and participant results. **Participants** can register accounts, browse upcoming events, enrol in races and track their results history.

> **Note:** Part 1 contains no application code. It focuses on system planning, database design, API planning and documentation before development begins in Part 2.

---

## 2. System Planning — 20 Key Points

| # | Area | Description |
|--:|------|-------------|
| 1 | **System Overview** | RaceDay is a full-stack, API-driven event management platform for running, walking and cycling events. |
| 2 | **Problem Statement** | Paper forms and spreadsheets can cause duplicate registrations, data-entry errors, lost information and increased administrative work. |
| 3 | **Proposed Solution** | RaceDay provides a centralised digital platform for managing events, participants, categories, enrolments, routes and results. |
| 4 | **System Objectives** | The system aims to simplify event administration, improve registration, reduce errors and provide easy access to participant and event information. |
| 5 | **User Roles** | The platform has two main roles: **Organiser** and **Participant**. |
| 6 | **Organiser Capabilities** | Organisers can create, edit and delete events, manage categories, view enrolments and capture participant results. |
| 7 | **Participant Capabilities** | Participants can register, log in, browse events, select categories, enrol and view their own enrolments and results. |
| 8 | **Database Design** | RaceDay uses a relational SQL Server database called **RaceDayDB** to store and manage system information. |
| 9 | **Users Entity** | Stores user information including UserID, name, email, password information, contact details and role. |
| 10 | **Events Entity** | Stores event information such as event name, description, date, location and organiser. |
| 11 | **Categories Entity** | Stores race categories associated with events, including different distances and age groups. |
| 12 | **Routes Entity** | Stores route information and supports future features such as maps, GPS information and weather information. |
| 13 | **Enrolments Entity** | Connects participants to events and categories. A participant can only enrol once in the same event. |
| 14 | **Results Entity** | Stores participant race results such as finishing time, position and result status. |
| 15 | **Relationships** | Primary keys, foreign keys and cardinality define relationships between Users, Events, Categories, Routes, Enrolments and Results. |
| 16 | **API Planning** | The API is divided into Authentication, User Profile, Events, Categories, Enrolments and Results using GET, POST, PUT and DELETE methods. |
| 17 | **Security & Integrity** | Planned security includes authentication, authorisation, password hashing and input validation. Database constraints protect data integrity. |
| 18 | **Sample Data** | The database contains 2 Organisers, 2 Participants, 3 Events, categories, routes, enrolments and sample result data. |
| 19 | **CI/CD & GitHub** | GitHub Actions automatically checks that the required planning documents and README are present whenever changes are pushed. |
| 20 | **Testing & Future Development** | The database can be tested in SSMS using SQL queries. Future versions may include payments, GPS routes, live weather, QR check-in, notifications and live results. |

---

## 3. User Roles

### Organiser

Organisers are responsible for managing sporting events.

**Organisers can:**
- Create events
- Edit events
- Delete events
- Create and manage categories
- View event enrolments
- Capture participant results
- Manage event route information

### Participant

Participants use RaceDay to find and participate in events.

**Participants can:**
- Register an account
- Log in
- Update their profile
- Browse upcoming events
- View event categories
- View route information
- Enrol in events
- View their own enrolments
- View their own results history

---

## 4. Database Design

The RaceDay database contains **six entities**:

| Entity | Purpose |
|--------|---------|
| **Users** | Stores Organiser and Participant accounts |
| **Events** | Stores sporting event information |
| **Categories** | Stores race categories belonging to events |
| **Routes** | Stores route information for events |
| **Enrolments** | Connects participants to events and categories |
| **Results** | Stores participant race results |

### Database Relationships

| Relationship | Cardinality |
|-------------|-------------|
| Users → Events | 1 : Many |
| Events → Categories | 1 : Many |
| Events → Routes | 1 : Many |
| Users → Enrolments | 1 : Many |
| Events → Enrolments | 1 : Many |
| Categories → Enrolments | 1 : Many |
| Enrolments → Results | 1 : 0..1 |

---

## 5. Important Database Rules

The database uses constraints to maintain reliable data.

### Primary Keys

Each table has a unique primary key:

```
UserID
EventID
CategoryID
RouteID
EnrolmentID
ResultID
```

### Foreign Keys

Foreign keys connect related tables and maintain referential integrity.

### Unique Enrolment

A participant cannot enrol in the same event twice. The combination of `ParticipantID + EventID` is unique.

### Unique Result

Each enrolment can have a maximum of one result. `EnrolmentID` is unique in the Results table.

---

## 6. API Endpoint Planning

| Area | Method | Endpoint | Purpose |
|------|--------|----------|---------|
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
| Results | POST | `/api/enrolments/{id}/result` | Capture result |
| Results | GET | `/api/users/me/results` | View own results |

The complete API plan is available in `docs/api_endpoint_plan.md`.

---

## 7. Documentation Files

```
docs/
├── raceday_erd.png
├── raceday_erd.svg
├── api_endpoint_plan.md
└── raceday_schema.sql
```

| File | Description |
|------|-------------|
| `raceday_erd.png` | Final Entity Relationship Diagram |
| `raceday_erd.svg` | Editable/source ERD |
| `api_endpoint_plan.md` | Full API endpoint plan |
| `raceday_schema.sql` | SQL Server database creation and sample data |

---

## 8. SQL Server Database

The database is called `RaceDayDB`. The SQL script creates:

- Database
- Users table
- Events table
- Categories table
- Routes table
- Enrolments table
- Results table
- Primary keys
- Foreign keys
- Unique constraints
- Other required constraints
- Sample data

---

## 9. Sample Data

| Data | Quantity |
|------|:--------:|
| Organisers | 2 |
| Participants | 2 |
| Events | 3 |
| Categories | Multiple |
| Routes | Multiple |
| Enrolments | Multiple |
| Results | Sample result |

The sample data allows the relationships between the database tables to be tested before application development begins.

---

## 10. Database Testing

The database can be tested using SQL Server Management Studio (SSMS).

**Basic verification queries:**

```sql
SELECT * FROM RaceDayDB.dbo.Users;
SELECT * FROM RaceDayDB.dbo.Events;
SELECT * FROM RaceDayDB.dbo.Categories;
SELECT * FROM RaceDayDB.dbo.Routes;
SELECT * FROM RaceDayDB.dbo.Enrolments;
SELECT * FROM RaceDayDB.dbo.Results;
```

**Relationship test:**

```sql
SELECT
    U.FirstName,
    U.LastName,
    E.EventName,
    C.CategoryName
FROM Enrolments EN
JOIN Users U ON EN.ParticipantID = U.UserID
JOIN Events E ON EN.EventID = E.EventID
JOIN Categories C ON EN.CategoryID = C.CategoryID;
```

---

## 11. Security Considerations

Security will be implemented during Part 2. Planned measures include:

- Password hashing
- User authentication
- Role-based authorisation
- Input validation
- Secure database queries
- Protection against unauthorised access
- User-specific data access
- Organiser permission checks

Participants should only be able to access their own enrolments and results, while organisers should only manage events and results for which they have permission.

---

## 12. Data Privacy

RaceDay stores personal information such as names, email addresses, contact numbers, account information and event participation history. The application should follow responsible data-management practices.

During production development, RaceDay should take the **Protection of Personal Information Act (POPIA)** into consideration when collecting, storing and processing participant information.

---

## 13. CI/CD

The project uses GitHub Actions for automated documentation validation.

**Workflow file:**

```
.github/
└── workflows/
    └── validate-docs.yml
```

The workflow checks that the following required files are present:

```
README.md
docs/raceday_erd.png
docs/raceday_erd.svg
docs/api_endpoint_plan.md
docs/raceday_schema.sql
```

This provides an automated quality check whenever changes are pushed to GitHub.

### CI Evidence

> **Insert screenshot of the successful/green GitHub Actions run here.**

---

## 14. How to Run the SQL Script

1. Open **SQL Server Management Studio (SSMS)**
2. Connect to a local or clean SQL Server instance
3. Open `docs/raceday_schema.sql`
4. Execute the script by pressing **F5**
5. Confirm that the database `RaceDayDB` has been created
6. Verify the tables and sample data, e.g.:

```sql
SELECT * FROM RaceDayDB.dbo.Events;
```

---

## 15. ERD

The RaceDay ERD is available in both PNG and SVG format:

```
docs/raceday_erd.png
docs/raceday_erd.svg
```

The ERD shows: six entities, primary keys, foreign keys, entity attributes, relationships and cardinality. The SQL database structure was designed to match the ERD.

---

## 16. Design Decisions

### Single Users Table

A single `Users` table is used for both Organisers and Participants. Both roles share common fields (name, email, password, contact information). Their different permissions and behaviours are controlled through the API/application layer.

### Routes Table

Routes are a dedicated entity because route information is a core part of the platform. This also enables future functionality such as GPS route data, interactive maps, elevation information, weather information and water-point information.

### Enrolments Table

The Enrolments table resolves the many-to-many relationship between participants and events, and stores information specific to a participant's registration for an event.

---

## 17. Assumptions

1. Every event has an organiser.
2. Every category belongs to an event.
3. Participants must have an account before enrolling.
4. A participant can only enrol once per event.
5. Each enrolment is associated with one category.
6. Results can only be recorded against valid enrolments.
7. Organisers manage their own events.
8. Authentication and authorisation will be implemented in Part 2.
9. SQL Server will be used as the database management system.
10. The application will communicate with the database through the API.

---

## 18. Future Enhancements

- Online registration payments
- Weather API integration
- GPS route mapping
- Live race tracking
- Live results
- QR-code race check-in
- Email and SMS notifications
- Mobile application
- Participant performance statistics
- Organiser dashboards
- Advanced reporting
- Digital race certificates

These features can be introduced without fundamentally changing the core database design.

---

## 19. Evidence Required for Submission

| Evidence | Requirement |
|----------|-------------|
| **ERD** | Screenshot/image of completed ERD |
| **Database** | Screenshot of RaceDayDB in SSMS |
| **Tables** | Evidence showing all six tables |
| **Sample Data** | Evidence showing inserted records |
| **SQL Execution** | Screenshot showing successful script execution |
| **API Plan** | Endpoint planning document |
| **CI/CD** | Screenshot of green GitHub Actions workflow |
| **Video** | Unlisted YouTube walkthrough |
| **Repository** | GitHub project repository |

---

## 20. Video Walkthrough

The walkthrough video covers:

1. Introduction to RaceDay
2. Explanation of the system problem
3. Explanation of the two user roles
4. Walkthrough of the ERD
5. Explanation of database relationships
6. Explanation of the six entities
7. Explanation of the API endpoint plan
8. Demonstration of the SQL script
9. Execution of the SQL script in SSMS
10. Verification of the sample data
11. Demonstration of the GitHub repository
12. Demonstration of the successful GitHub Actions workflow



## Conclusion

RaceDay Part 1 establishes the foundation for the development of the RaceDay event management platform. The planning phase defines the system requirements, user roles, database structure, API endpoints, security considerations, testing approach and development workflow.

The six database entities — **Users, Events, Categories, Routes, Enrolments and Results** — provide a structured foundation for managing sporting events and participant information.

The ERD, API endpoint plan and SQL Server database script are designed to work together and provide a clear blueprint for the application development that will take place in **Part 2**.
