# RaceDay API Endpoint Plan

This plan is built directly from the RaceDay ERD (`raceday_erd.png`) and `raceday_schema.sql`, and covers every functional requirement described in Part 2 of the POE: Authentication, User Profile, Events, Categories, Event Enrolments, and Results.

**Role legend:** `None` = public/no login required · `Any` = any authenticated user · `Organiser` = Organiser role only · `Participant` = Participant role only. Where an endpoint says "(owner)", the API must also confirm the logged-in Organiser owns the Event/Category/Result being acted on, not just that they hold the Organiser role.

---

## 1. Authentication

| HTTP Method | Route | Description | Role Required | Request Body | Expected Response |
|---|---|---|---|---|---|
| POST | `/api/auth/register` | Creates a new account as either an Organiser or a Participant. | None | `{ firstName, lastName, email, password, phoneNumber, role }` | `201 Created` – new user (no password returned) • `400 Bad Request` – validation failed • `409 Conflict` – email already registered |
| POST | `/api/auth/login` | Verifies credentials and starts a session that stores the user's ID and role. | None | `{ email, password }` | `200 OK` – user summary + role, session cookie set • `401 Unauthorized` – invalid email or password |
| POST | `/api/auth/logout` | Ends the current session. | Any | None | `200 OK` – logged out • `401 Unauthorized` – no active session |

## 2. User Profile

| HTTP Method | Route | Description | Role Required | Request Body | Expected Response |
|---|---|---|---|---|---|
| GET | `/api/users/me` | Returns the logged-in user's own profile. | Any | None | `200 OK` – profile object • `401 Unauthorized` |
| PUT | `/api/users/me` | Updates the logged-in user's own profile (both roles can update their own details). | Any | `{ firstName, lastName, phoneNumber }` | `200 OK` – updated profile • `400 Bad Request` – validation failed • `401 Unauthorized` |

> **Note:** Profile picture upload (`profilePictureUrl`) is introduced in Part 3 alongside Azure Blob Storage integration, per the POE's Part 3 requirements. It is intentionally excluded here so this plan matches the Part 1 database exactly; the `Users` table and this endpoint will both be extended with a `ProfilePictureUrl` column when that work begins.

## 3. Events

| HTTP Method | Route | Description | Role Required | Request Body | Expected Response |
|---|---|---|---|---|---|
| GET | `/api/events` | Lists all events, with optional query filters (`type`, `location`, `fromDate`). Used for the Participant home page and Organiser dashboard. | None | None | `200 OK` – array of events |
| GET | `/api/events/{id}` | Returns full details of one event, including its categories and routes. | None | None | `200 OK` – event detail object • `404 Not Found` |
| POST | `/api/events` | Creates a new event owned by the logged-in Organiser. | Organiser | `{ name, description, eventDate, location, distanceKm, eventType, bannerImageUrl }` | `201 Created` – new event • `400 Bad Request` – validation failed • `403 Forbidden` – not an Organiser |
| PUT | `/api/events/{id}` | Updates an event (owner Organiser only). | Organiser (owner) | `{ name, description, eventDate, location, distanceKm, eventType, bannerImageUrl }` | `200 OK` – updated event • `400 Bad Request` • `403 Forbidden` – not the owning Organiser • `404 Not Found` |
| DELETE | `/api/events/{id}` | Deletes an event (owner Organiser only). | Organiser (owner) | None | `204 No Content` • `403 Forbidden` • `404 Not Found` |

## 4. Categories

| HTTP Method | Route | Description | Role Required | Request Body | Expected Response |
|---|---|---|---|---|---|
| GET | `/api/events/{eventId}/categories` | Lists the age/distance categories available for an event. | None | None | `200 OK` – array of categories • `404 Not Found` – event does not exist |
| POST | `/api/events/{eventId}/categories` | Adds a category to an event. | Organiser (owner) | `{ categoryName, minAge, maxAge, distanceKm }` | `201 Created` – new category • `400 Bad Request` • `403 Forbidden` • `404 Not Found` |
| PUT | `/api/categories/{id}` | Updates a category's details. | Organiser (owner) | `{ categoryName, minAge, maxAge, distanceKm }` | `200 OK` – updated category • `400 Bad Request` • `403 Forbidden` • `404 Not Found` |
| DELETE | `/api/categories/{id}` | Removes a category from an event. | Organiser (owner) | None | `204 No Content` • `403 Forbidden` • `404 Not Found` • `409 Conflict` – category has active enrolments |

## 5. Event Enrolments

| HTTP Method | Route | Description | Role Required | Request Body | Expected Response |
|---|---|---|---|---|---|
| POST | `/api/events/{eventId}/enrolments` | Enrols the logged-in Participant into an event under a chosen category. | Participant | `{ categoryId }` | `201 Created` – new enrolment • `400 Bad Request` • `401 Unauthorized` • `404 Not Found` – event/category does not exist • `409 Conflict` – already enrolled in this event |
| GET | `/api/users/me/enrolments` | Returns all events the logged-in Participant has enrolled in, with status. | Participant | None | `200 OK` – array of enrolments • `401 Unauthorized` |
| GET | `/api/events/{eventId}/enrolments` | Returns all Participants enrolled in an event the Organiser owns. | Organiser (owner) | None | `200 OK` – array of enrolments • `403 Forbidden` • `404 Not Found` |
| DELETE | `/api/enrolments/{id}` | Cancels the logged-in Participant's own enrolment, provided the event has not started. | Participant (owner) | None | `204 No Content` • `403 Forbidden` – not the owning Participant • `404 Not Found` • `409 Conflict` – event already started |

## 6. Results

| HTTP Method | Route | Description | Role Required | Request Body | Expected Response |
|---|---|---|---|---|---|
| POST | `/api/events/{eventId}/results` | Captures/publishes a finish time and position for an enrolled Participant after the event. | Organiser (owner) | `{ enrolmentId, finishTimeSeconds, finishPosition, totalFinishers }` | `201 Created` – new result • `400 Bad Request` • `403 Forbidden` • `404 Not Found` – event/enrolment does not exist |
| PUT | `/api/results/{id}` | Corrects a previously captured result. | Organiser (owner) | `{ finishTimeSeconds, finishPosition, totalFinishers }` | `200 OK` – updated result • `400 Bad Request` • `403 Forbidden` • `404 Not Found` |
| GET | `/api/users/me/results` | Returns the logged-in Participant's full personal race history. | Participant | None | `200 OK` – array of results with event name, date, category, finish time and position • `401 Unauthorized` |
| GET | `/api/results/{id}` | Returns one result (viewable by the owning Participant or the Organiser of that event). | Any (owner-checked) | None | `200 OK` – result object • `403 Forbidden` • `404 Not Found` |

---

POST   /api/events/{eventId}/categories    - Create new category
PUT    /api/categories/{id}                - Update category
DELETE /api/categories/{id}                - Delete category

### Coverage check against Part 2 functional requirements
- **Authentication:** register + login ✔
- **User Profile:** view + update own profile, both roles ✔
- **Events:** full CRUD for Organisers, read access for both roles ✔
- **Categories:** full CRUD for Organisers, read access for both roles ✔
- **Event Enrolments:** Participant enrolment, Organiser visibility of enrolments ✔
- **Results:** Organiser capture, Participant personal view ✔
