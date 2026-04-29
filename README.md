# 🏥 MediCare Hospital Management System
### ASP.NET Core 8 MVC Project

---

## ✅ Prerequisites

Install the following before running:

1. **.NET 8 SDK** → https://dotnet.microsoft.com/download/dotnet/8.0
2. Any code editor (Visual Studio 2022 / VS Code / Rider)

---

## 🚀 Quick Start

### Option A — Command Line

```bash
# 1. Navigate to the project folder
cd HospitalMS

# 2. Restore NuGet packages
dotnet restore

# 3. Run the app (database is created automatically with seed data)
dotnet run

# 4. Open in browser
# https://localhost:5001  OR  http://localhost:5000
```

### Option B — Visual Studio 2022

1. Open `HospitalMS.csproj` in Visual Studio 2022
2. Press **F5** or click the green ▶ Run button
3. The browser opens automatically

---

## 🗂️ Project Structure

```
HospitalMS/
├── Controllers/
│   └── Controllers.cs          # All 8 controllers (Home, Patient, Appointment, etc.)
├── Data/
│   └── HospitalDbContext.cs    # EF Core DbContext + Seed data
├── Models/
│   └── Models.cs               # Domain models (Patient, Doctor, Appointment, etc.)
├── Services/
│   └── Services.cs             # Business logic services
├── ViewModels/
│   └── ViewModels.cs           # View-specific data models
├── Views/
│   ├── Shared/
│   │   ├── _Layout.cshtml      # Main layout with sidebar navigation
│   │   └── _ValidationScriptsPartial.cshtml
│   ├── Home/                   # Dashboard
│   ├── Patient/                # CRUD pages
│   ├── Appointment/            # Booking + Schedule
│   ├── Doctor/                 # Staff management
│   ├── Pharmacy/               # Inventory
│   ├── Billing/                # Invoices
│   ├── Reports/                # Analytics
│   └── Settings/               # Configuration
├── wwwroot/
│   ├── css/site.css            # Full custom CSS (no Bootstrap required)
│   └── js/site.js              # Bar chart rendering + UI helpers
├── Program.cs                  # App setup + DI registration
├── appsettings.json            # Configuration
└── HospitalMS.csproj           # Project file
```

---

## 🌟 Features

| Module | Capabilities |
|---|---|
| Dashboard | Live stats, charts, alerts, quick actions |
| Patients | Register, search, view records, edit, delete |
| Appointments | Book, filter by status/date, weekly schedule view |
| Doctors & Staff | Add staff, status tracking, department filter |
| Schedule | Weekly calendar view with appointment slots |
| Pharmacy | Inventory tracking, stock level bars, expiry alerts |
| Billing | Create invoices, line items, payment tracking, print |
| Reports | Monthly charts, revenue breakdown, department stats |
| Settings | Hospital info, notification toggles, password update |

---

## 🗄️ Database

- Uses **SQLite** (file-based, no installation needed)
- Database file `hospital.db` is auto-created on first run
- **Seed data** is inserted automatically:
  - 8 Doctors (Cardiology, Neurology, Orthopedics, etc.)
  - 8 Patients with realistic Indian names
  - 8 Appointments (today + upcoming)
  - 8 Medicines with stock levels
  - 4 Sample Invoices
  - 100 Beds (General, ICU, Private, Semi-Private)

---

## 🔧 Extending the Project

### Add Authentication
```bash
dotnet add package Microsoft.AspNetCore.Identity.EntityFrameworkCore
# Then scaffold Identity pages
dotnet aspnet-codegenerator identity -dc HospitalMS.Data.HospitalDbContext
```

### Switch to SQL Server
In `appsettings.json`, change connection string:
```json
"DefaultConnection": "Server=.;Database=HospitalDB;Trusted_Connection=True;"
```
In `Program.cs`, change:
```csharp
options.UseSqlServer(connectionString)
```

### Apply EF Migrations
```bash
dotnet ef migrations add InitialCreate
dotnet ef database update
```

---

## 📦 NuGet Packages Used

| Package | Purpose |
|---|---|
| Microsoft.EntityFrameworkCore 8.0 | ORM |
| Microsoft.EntityFrameworkCore.Sqlite 8.0 | SQLite provider |
| Microsoft.EntityFrameworkCore.Tools 8.0 | Migrations |

---

## 🎨 Design

- Custom CSS (no Bootstrap/Tailwind dependency)
- Google Fonts: Playfair Display + DM Sans
- Responsive sidebar navigation
- Professional navy + teal color scheme
- All data visualizations built in pure CSS/JS (no Chart.js needed)

---

Built with ❤️ for MediCare General Hospital, Hyderabad
