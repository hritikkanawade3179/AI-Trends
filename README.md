# AI Trends News — .NET Core MVC Web Application

A full-featured AI news & updates management platform built with ASP.NET Core 8 MVC.

## Tech Stack
- **Backend:** .NET 8 / ASP.NET Core MVC
- **ORM:** Entity Framework Core 8 with SQL Server
- **Auth:** ASP.NET Core Identity (roles: Admin, Editor, Viewer)
- **Frontend:** HTML5, Bootstrap 5.3, Bootstrap Icons
- **Charts:** Chart.js (admin dashboard)

---

## Quick Start

### 1. Prerequisites
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- SQL Server (LocalDB is included with Visual Studio) or any SQL Server instance

### 2. Configure Database
Edit `appsettings.json` and update your connection string:
```json
"DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=AITrendsNewsDb;Trusted_Connection=True;"
```
For SQL Server Express: `Server=.\\SQLEXPRESS;Database=AITrendsNewsDb;Trusted_Connection=True;`

### 3. Run the Application
```bash
cd AITrendsNews
dotnet restore
dotnet ef migrations add InitialCreate
dotnet ef database update
dotnet run
```
The app auto-runs migrations and seeds the database on first launch.

### 4. Default Login Credentials
| Role   | Email                        | Password   |
|--------|------------------------------|------------|
| Admin  | admin@aitrendsnews.com       | Admin@123! |
| Editor | editor@aitrendsnews.com      | Editor@123!|

---

## Project Structure
```
AITrendsNews/
├── Controllers/
│   ├── HomeController.cs       — Public homepage
│   ├── NewsController.cs       — News listing + detail
│   ├── AdminController.cs      — CMS dashboard (auth required)
│   └── AccountController.cs   — Login/logout
├── Models/
│   ├── Post.cs                 — Main content model
│   ├── Category.cs             — Post categories
│   ├── Tag.cs / PostTag.cs     — Many-to-many tags
│   ├── ApplicationUser.cs      — Extended Identity user
│   └── NewsletterSubscriber.cs
├── Data/
│   ├── ApplicationDbContext.cs — EF Core DbContext + seeded categories
│   └── DbSeeder.cs            — Seeds roles and admin user
├── Services/
│   └── PostService.cs         — Business logic, queries
├── ViewModels/
│   └── ViewModels.cs          — HomeViewModel, NewsListViewModel, etc.
├── Views/
│   ├── Home/Index.cshtml       — Public homepage
│   ├── News/
│   │   ├── Index.cshtml        — News listing with filters
│   │   └── Detail.cshtml       — Full post view
│   ├── Admin/
│   │   ├── Dashboard.cshtml    — Stats + analytics charts
│   │   ├── Posts.cshtml        — Post management table
│   │   ├── CreatePost.cshtml / EditPost.cshtml
│   │   ├── _PostForm.cshtml    — Shared post form partial
│   │   ├── Categories.cshtml
│   │   ├── Tags.cshtml
│   │   ├── Subscribers.cshtml
│   │   └── Users.cshtml
│   ├── Account/Login.cshtml
│   └── Shared/
│       ├── _Layout.cshtml      — Public layout
│       └── _AdminLayout.cshtml — Admin sidebar layout
├── wwwroot/
│   ├── css/site.css + admin.css
│   └── js/site.js
├── Program.cs                  — App startup, DI, middleware
└── appsettings.json
```

## Content Types Managed
- **News Articles** — regular news posts
- **Research Papers** — academic / arxiv papers with external links
- **AI Tools** — tool listings with external URLs
- **Video Updates** — video content with links
- **Events** — conferences, meetups, webinars

## Admin Features
- Dashboard with post counts, total views, subscriber count, Chart.js doughnut chart
- Create / Edit / Delete posts with rich HTML body, thumbnail, external URL, tags, categories
- Featured + Trending flags per post
- Category management (name, color, icon, sort order)
- Tag management
- Newsletter subscriber list
- Role-based access: Admin (full), Editor (posts + categories/tags), Viewer (read-only)
