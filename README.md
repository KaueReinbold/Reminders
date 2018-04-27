# Reminders

Application to register and list reminders.
Each reminder has a Title, Description, Date Limit and Status.

## Technical Objective

This application was develop with Asp.Net Core MVC and Asp.Net Core Web Api. The propos is show the knowledge using this tecnologies.

## Technical Details

### Backend - Asp.Net Standard 2.0 Class Library

> *Reminders.Business, Reminders.Domain and Reminders.Context*
- Patterns: DDD, MVVM and Repository.
- Object Relational Mapping (ORM): Microsoft Entity Framework Core 2.0.
- Database: SQL Server.

### Test - Asp.Net Core 2.0 MS Test Application

> *Reminders.Test*
- Repository Test.
- Business Test.
- Browsers Test using Selenium Web Driver.
  - Chrome, Firefox and Edge.

### Frontend - [Asp.Net Core MVC](http://reminders.azurewebsites.net/)

> *Reminders.Mvc*
- App Layer is using Asp.Net Core MVC v2.0.0.
- Responsive application using Bootstrap Framework v4.1.0 and jQuery 1.9.1.

### Web Api - [Asp.Net Core Web Api](http://reminderswebapi.azurewebsites.net/)

> *Reminders.Api*
- App Layer is using Asp.Net Core web Api v2.0.0.
- Usgin Swagger UI v3.0 for API documentation.

### How to configure (Local)

- It will be necessary have an instance of SQL Server available.
- The database user must have the permission to create a database, when the application run for the first time the database will be created.
- The connection string must be put in the file "appsettings.json" at the key "StringConnectionReminders" inside the project [Reminders.Mvc](https://github.com/KaueReinbold/Reminders/blob/master/Reminders.Mvc/appsettings.json)/[Reminders.Api](https://github.com/KaueReinbold/Reminders/blob/master/Reminders.Api/appsettings.json).