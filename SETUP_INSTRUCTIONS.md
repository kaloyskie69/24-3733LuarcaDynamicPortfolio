# Personal Website — ASP.NET Web Forms (C#) + MS SQL Server
### Setup Guide

This project is built as a **Web Site Project** (not a Web Application `.csproj`), which is
why there's no `.csproj`/`.sln` file — Visual Studio compiles it dynamically and auto-matches
your `.aspx` controls to the code-behind, so you don't need `.designer.cs` files either.

---

## 1. What you need installed

| Tool | Notes |
|---|---|
| **Visual Studio 2019 or 2022** (Community edition is fine) | Install the **"ASP.NET and web development"** workload during setup. |
| **SQL Server** (Express, Developer, or full) | SQL Server Express + **SQL Server Management Studio (SSMS)** is the easiest free combo. |
| **.NET Framework 4.8** | Usually already on Windows; Visual Studio installer will offer it if missing. |

---

## 2. Set up the database

1. Open **SQL Server Management Studio (SSMS)** and connect to your local SQL Server instance.
2. Open `Database/01_Schema_And_SeedData.sql` and click **Execute**.
   - This creates the `PersonalWebsiteDB` database, all 7 tables, and seeds your
     personal info, education, skills, and hobbies.
3. Open `Database/02_StoredProcedures.sql` and click **Execute**.
   - This creates every stored procedure the app uses. No inline SQL exists anywhere in the C# code.
4. Confirm it worked: expand **Databases → PersonalWebsiteDB → Tables** — you should see
   `Users`, `Roles`, `PersonalDetails`, `Education`, `Skills`, `Hobbies`, `AuditLogs`.

> **Note:** No user accounts are seeded — see step 5 below for why, and how to create your first Admin login.

---

## 3. Open the project in Visual Studio

1. In Visual Studio: **File → Open → Web Site...** (NOT "Open Project/Solution").
2. Browse to and select the `PersonalWebsiteProject` folder → **Open**.
3. In **Solution Explorer**, right-click the site root → **Property Pages** (or just proceed to step 4 — IIS Express works out of the box for Web Site projects).

---

## 4. Point the app at your database

1. Open **`Web.config`** in the project root.
2. Find the `<connectionStrings>` section near the top:

```xml
<add name="PersonalWebsiteDB"
     connectionString="Data Source=.\SQLEXPRESS;Initial Catalog=PersonalWebsiteDB;Integrated Security=True;TrustServerCertificate=True;"
     providerName="System.Data.SqlClient" />
```

3. Update `Data Source` to match your SQL Server instance name:
   - Default local instance → `Data Source=.`
   - SQL Express (default name) → `Data Source=.\SQLEXPRESS`
   - Named/remote server → `Data Source=YOUR_SERVER_NAME`
4. If you use **SQL Server Authentication** instead of Windows login, replace
   `Integrated Security=True` with `User Id=your_user;Password=your_password;`

---

## 5. Run it and create your first account

1. Press **F5** (or Ctrl+F5) in Visual Studio to launch the site in IIS Express.
2. It will open to `Login.aspx` (you're forced to sign in first, per the spec).
3. Click **"Sign up here"** → fill out the Register form → submit.
   - This creates a normal **User** account with a securely hashed password (PBKDF2 — the plain password is never stored).
4. To make that account an **Admin**, go back to SSMS and run:

```sql
UPDATE dbo.Users SET RoleId = 1 WHERE Username = 'the_username_you_registered';
```

5. Log out and log back in. You'll now see an **"Admin Panel"** link in the navbar, giving you access to:
   - **Users tab** — view all users, activate/deactivate, or delete accounts
   - **Statistics tab** — active/inactive counts, role breakdown, recent signups, activity log
   - **Website Content tab** — edit the Personal Info, Education, Skills, and Hobbies shown on the public dashboard

You can create as many additional test accounts as you like via Register.aspx to test the "regular User" experience (sign in → view dashboard → edit own profile in `UserProfile.aspx`) versus the Admin experience.

---

## 6. Project structure reference

```
PersonalWebsiteProject/
├── Database/
│   ├── 01_Schema_And_SeedData.sql      ← run first
│   └── 02_StoredProcedures.sql         ← run second
├── App_Code/                            ← Data Access Layer (C#)
│   ├── DbHelper.cs                     ← executes stored procedures only
│   ├── PasswordHelper.cs               ← PBKDF2 password hashing
│   ├── UserAccount.cs                  ← model
│   ├── UserRepository.cs               ← auth + admin user management
│   ├── ContentRepository.cs            ← dynamic content (personal info/education/skills/hobbies)
│   └── AuditRepository.cs              ← activity logging
├── Admin/
│   ├── AdminDashboard.aspx             ← Admin panel (Users / Stats / Content Editor tabs)
│   └── AdminDashboard.aspx.cs
├── Content/Site.css                    ← custom styling (loaded on top of Bootstrap 5 via CDN)
├── Site.Master / Site.Master.cs        ← shared layout, nav bar, logout
├── Login.aspx / .cs
├── Register.aspx / .cs
├── Default.aspx / .cs                  ← public dashboard (Personal Info/Education/Skills/Hobbies) — sign-in required
├── UserProfile.aspx / .cs              ← user edits their own Full Name / Email / Password
├── Global.asax / .cs                   ← reconstructs role info from the auth cookie on each request
└── Web.config                          ← connection string, Forms Authentication, role-based folder locking
```

---

## 7. How the key requirements are satisfied

- **Sign-in required before viewing content**: `Web.config` denies anonymous users (`<deny users="?"/>`) site-wide; only `Login.aspx`, `Register.aspx`, and static assets are explicitly allowed.
- **Sign-up if not registered**: `Register.aspx` creates the account; passwords are salted and hashed with PBKDF2 (100,000 iterations) — never stored or compared in plain text.
- **User can modify own info**: `UserProfile.aspx` (Full Name, Email, Password).
- **Admin can view users / deactivate / remove**: `Admin/AdminDashboard.aspx` → Users tab.
- **Admin can view statistics**: Statistics tab (active/inactive counts, roles, 7-day signups, recent activity log from `AuditLogs`).
- **Admin can modify website content**: Website Content tab (Personal Info, Education, Skills, Hobbies — all backed by their own tables and stored procedures).
- **All DB operations via stored procedures**: every call in `DbHelper.cs`/the repository classes invokes a named stored procedure (`CommandType.StoredProcedure`) — there is no string-built SQL anywhere.

---

## 8. Common issues

| Problem | Fix |
|---|---|
| "Cannot open database... login failed" | Check the `Data Source` value and authentication mode in `Web.config` match your SQL Server setup. |
| Blank/error page mentioning `App_Code` | Make sure you opened it as **File → Open → Web Site**, not as a Project — that's what makes `App_Code` auto-compile. |
| Login always fails | Make sure you registered via `Register.aspx` first — no accounts are pre-seeded (see Step 5). |
| Admin Panel link missing after promoting a user | Log out and log back in — the role is embedded in the auth cookie at login time, so it won't update until you re-authenticate. |
