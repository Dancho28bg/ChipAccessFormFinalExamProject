## Configuration

This project uses `appsettings.Development.json` for **local secrets**.

After cloning the repository, create the following file:

```
ChipAccess.Api/appsettings.Development.json
```

Example content:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "YOUR_SQL_CONNECTION_STRING"
  },
  "Jwt": {
    "Key": "YOUR_JWT_SECRET"
  }
}
```
## Run Backend

From the repository root:

```bash
cd ChipAccess.Api
dotnet restore
dotnet ef database update
dotnet run
```

The API will start at:

```
https://localhost:5001
```

Swagger UI:

```
https://localhost:5001/swagger
```

---

## Database Seeding

On application startup, the database is automatically created and seeded with test data.

Seeded data includes:
- 1 Admin
- 1 Manager
- 2 Users
- 10 access requests in various states

### Test Users

| Role     | BAM ID   |
|----------|----------|
| User     | uuuu0001 |
| User     | uuuu0002 |
| User     | uuuu0003 |
| Manager  | mmmm0001 |
| Admin    | aaaa0001 |

---

## Run Frontend

Open a **new terminal** and run:

```bash
cd chipaccess-frontend
npm install
npm run dev
```

The frontend will be available at:

```
http://localhost:5173
```
