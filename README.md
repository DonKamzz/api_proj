# RaceDay: Parts 2 and 3, REST API and MVC Web App

**Module:** PROG6212/w, Programming 2B
**Student:** Kamogelo Leburu (ST10459758)
**Assessment:** Portfolio of Evidence, Part 2 (RESTful API) and Part 3 (MVC web app, Azure Blob Storage, Docker)

RaceDay is an event management platform for South African running, walking and cycling events. Organisers create and manage events, categories and results. Participants browse events, enter them and track their results.

## Solution structure

```
api_proj.sln
├── api_proj/                 Part 2 and 3: ASP.NET Core Web API
│   ├── Controllers/          Auth, Users, Events, Categories, Enrolments, Results
│   ├── Models/               EF Core entities (match the Part 1 ERD)
│   ├── Data/                 ApplicationDBContext (Code-First)
│   ├── DTOs/                 Request and response models
│   ├── Auth/                 BCrypt password hasher and claims helpers
│   ├── Services/             BlobStorageService (Azure Blob Storage uploads)
│   └── Program.cs
├── api_proj.Tests/           xUnit tests, run against the real HTTP pipeline
├── mvc_proj/                 Part 3: ASP.NET Core MVC web app (no database access)
│   ├── Controllers/          Account, Home, Events, Participant, Organiser
│   ├── Services/             ApiClient, the only way the MVC app reaches data
│   ├── Filters/              SessionAuthorize (login and role checks)
│   ├── Views/                Razor pages (Bootstrap and RaceDay theme)
│   ├── wwwroot/css/site.css  RaceDay brand styling
│   └── Dockerfile
├── docker-compose.yml
├── docs/part3_api_additions.md
└── .github/workflows/dotnet-ci.yml
```

## The two roles

| Role | What they can do |
|---|---|
| Organiser | Dashboard with enrolment counts, create, edit and delete events with a banner image, add and remove categories, view enrolments, capture and publish results. |
| Participant | Browse and filter upcoming events, view event details and categories, enter an event by choosing a category, see enrolment status (green for Confirmed), see race results, update profile and profile picture. |

## How the MVC app talks to the API

1. The MVC app has no database code and no business rules. Every page calls the API through `Services/ApiClient.cs`.
2. When a user logs in, the API signs them in with its `RaceDayAuth` cookie. `ApiClient` stores that cookie in the user's MVC session and sends it with every later request, so the API still enforces roles.
3. The MVC session stores the user's ID, role and first name. `SessionAuthorize` sends visitors who aren't logged in to the login page, and sends the wrong role to an access denied page. Logging out clears the session and signs the user out of the API.
4. Images: the MVC app sends the file to the API as `multipart/form-data`. Only the API talks to Azure. The API stores the image in a private Azure Blob container and saves a signed, read-only link in the database. The MVC app displays that link.
5. Errors from the API are converted to friendly messages in `ApiClient`. Unexpected errors and missing pages show a friendly page, never raw server output.

## Setup and running

### 1. Database

Create `RaceDayDB` on your SQL Server instance. The connection string in `api_proj/appsettings.json` uses `(localdb)\PROGPOE`. Use one of these options:

- Run `docs/raceday_schema.sql` (from Part 1) in SSMS, or
- In Package Manager Console, with `api_proj` as the Default project, run `Add-Migration InitialCreate` then `Update-Database`.

### 2. Azure Blob Storage (for banner and profile images)

1. In the Azure portal, create a **Storage account** in a resource group, for example `raceday-rg`. Use a unique name in lowercase letters and numbers. This README uses `racedaystorage2003` as an example.
2. Under **Security + networking**, click **Access keys**. Under **key1**, click **Show** and copy the **Connection string**.
3. In Visual Studio, right-click `api_proj` and choose **Manage User Secrets**. Paste this, with your connection string in place of the placeholder:

```json
{
  "AzureBlobStorage": {
    "ConnectionString": "PASTE_YOUR_CONNECTION_STRING_HERE"
  }
}
```

Save the file. The connection string is stored outside the project folder, so it is not committed to GitHub.

The API creates a private container named `raceday-images` the first time an image is uploaded. Images are served through signed links that stay valid for five years. You do not need to enable public blob access.

**No Azure account?** Install Node.js, run `npm install -g azurite`, then start it with `azurite --location C:\azurite --blobPort 10000`. In User Secrets, use `"ConnectionString": "UseDevelopmentStorage=true"`. Images then only display on the machine running Azurite.

### 3. Run the API and the MVC app together

1. Right-click the solution, choose **Configure Startup Projects**, select **Multiple startup projects**, and set both `api_proj` and `mvc_proj` to **Start**.
2. Press F5.
   - API and Swagger: https://localhost:5001/swagger
   - MVC web app: http://localhost:5100

The MVC app reads the API address from `mvc_proj/appsettings.Development.json` (`ApiSettings:BaseUrl`).

### 4. Run the MVC app in Docker

The API keeps running from Visual Studio at https://localhost:5001. From the repository root, run:

```
docker compose up --build
```

Or, with plain Docker:

```
docker build -f mvc_proj/Dockerfile -t raceday-mvc .
docker run -p 8080:8080 -e ApiSettings__BaseUrl=https://host.docker.internal:5001/ -e ApiSettings__AllowUntrustedCertificate=true raceday-mvc
```

Open http://localhost:8080. The container uses the same code as the local run. The only difference is the API address, passed as `ApiSettings__BaseUrl`. `host.docker.internal` lets the container reach the host machine. `AllowUntrustedCertificate` accepts the API's self-signed development certificate and must never be used in production.

### 5. Unit tests

```
dotnet test api_proj.sln
```

| Test class | What it covers |
|---|---|
| AuthTests | Register, login (success and failure), cookie-protected access, logout |
| EventManagementTests | Organiser event management, ownership checks, validation, 404 |
| RoleEnforcementTests | Wrong role and not-logged-in users are rejected |
| EnrolmentTests | Enrolment is recorded, duplicates are blocked, 404 and 401 handling |
| Part3EndpointTests | Organiser dashboard list, image upload role and file checks, delete blocked when enrolments exist |

## Demo flow

1. **Organiser:** sign up as an Organiser, log in, and click **New event**. Create an event and upload a banner image.
2. **Organiser:** click **My events**, then **Categories** on that event. Add a category such as Open, with the distance set. Categories must exist before anyone can enter the event.
3. **Participant:** log out (or use a private browser window), sign up as a Participant, log in, open the event from the home page, choose the category and click **Enter event**. Check **My enrolments**, where the status shows green.
4. **Organiser:** log in, open **Dashboard** to see the enrolment count, then **Results** to enter a finish time, position and total finishers, and click **Save**.
5. **Participant:** open **My results** to see the time and position.

## Troubleshooting

| Problem | Likely cause and fix |
|---|---|
| Event page shows no enrol button | The event has no categories. Add one as the Organiser. |
| Image shows "Public access is not permitted" | Old link from an earlier version. Upload the banner again. |
| Upload returns 503 "not been configured" | User Secrets missing or misspelled. Check `AzureBlobStorage:ConnectionString`, then restart. |
| Upload returns 503 "unavailable" | Connection string is wrong or copied incompletely. Recopy from Access keys. |
| Azure portal shows no access or no subscription | Use an Azure for Students subscription or Azurite (see above). |
| MVC pages send you to login | Session expired after 20 minutes idle. Log in again. |
| "We could not reach the RaceDay API" | API not running, or `ApiSettings:BaseUrl` does not match its address. |

## Part 3 rubric: where each item is addressed

| Rubric criterion | Where it is addressed |
|---|---|
| Correct submission | This README (setup, Docker run command, video link), `mvc_proj/Dockerfile`, full source |
| GitHub and CI/CD | `.github/workflows/dotnet-ci.yml` builds the API, MVC app and tests, runs the tests and builds the Docker image |
| Authentication and role-based navigation | `AccountController` (register, login, logout), session stores user ID and role, `SessionAuthorizeAttribute`, role-specific menus in `Views/Shared/_Layout.cshtml` |
| Organiser interface | `OrganiserController` and `Views/Organiser`: dashboard, events (create, edit, delete, banner), categories, enrolments, results |
| Participant interface | `HomeController` (events and filters), `EventsController` (details and enrol), `ParticipantController` (my enrolments and results), `AccountController` (profile and picture) |
| Azure Blob Storage | `api_proj/Services/BlobStorageService.cs`, `POST /api/events/{id}/banner`, `POST /api/users/me/profile-picture`. The MVC app only calls the API. |
| Docker | `mvc_proj/Dockerfile` (multi-stage, non-root user), `docker-compose.yml`, run commands above |
| UI design | Bootstrap plus `wwwroot/css/site.css`: RaceDay colours and fonts, colour-coded statuses, consistent layout, validation messages and friendly error pages |
| Video | Link below |

## CI/CD

`.github/workflows/dotnet-ci.yml` runs on every push. It restores packages, builds `api_proj`, `mvc_proj` and `api_proj.Tests` in Release, runs the tests, and separately builds the MVC Docker image.


