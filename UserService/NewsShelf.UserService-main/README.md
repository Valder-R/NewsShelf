# NewsShelf User Service

ASP.NET Core 8 microservice that manages NewsShelf user accounts, profiles, reading history, favourite topics, and OAuth sign-in flows.

## Features
- Email/password registration and login backed by ASP.NET Identity + SQLite.
- Profile management (display name, bio, favourite topics).
- Activity tracking for read news items and favourites history endpoints.
- OAuth integration hooks (Google, Facebook) with a simple token validator stub.
- JWT based authentication with Swagger/OpenAPI documentation.

## Getting Started
1. **Prerequisites**
   - .NET 8 SDK
   - SQLite (optional, created automatically)
2. **Restore & Build**
   ```bash
   dotnet restore
   dotnet build
   ```
   > If your environment blocks access to nuget.org, add the proper feed credentials or run `dotnet restore --ignore-failed-sources`.
3. **Database**
   - Connection strings live in `appsettings*.json`. By default data files are stored under `NewsShelf.UserService.Api/Data`.
   - Apply migrations automatically on startup (`dotnet ef migrations add InitialCreate` can be added later if you need explicit migration files).
4. **Run**
   ```bash
   dotnet run --project NewsShelf.UserService.Api
   ```
5. **Swagger UI**
   - Navigate to `https://localhost:5001/swagger` (or configured port) to explore endpoints.

## Configuration
- Update the `Jwt` section with a secure signing key and issuer/audience pair.
- Populate `Authentication:Google` and `Authentication:Facebook` secrets to enable real OAuth flows.
- Identity password rules are relaxed for demo purposes—tighten them before production.

## Key Endpoints
- `POST /api/auth/register` – create user + optional favourite topics.
- `POST /api/auth/login` – issue JWT for email/password.
- `POST /api/auth/external` – sign in via Google/Facebook token (validator stub).
- `GET /api/profile/me` – fetch current profile.
- `PUT /api/profile` – update display name, bio, favourite topics.
- `POST /api/activity/read` – record news read event.
- `GET /api/activity/history` – list recent activity.
- `GET/POST /api/activity/favorite-topics` – manage favourite topics collection.

## Next Steps
- Replace `SimpleExternalTokenValidator` with calls to Google/Facebook token introspection APIs.
- Add integration tests for critical flows.
- Create EF Core migrations and CI/CD pipeline scripts when moving beyond the prototype.
