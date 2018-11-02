# Reminders

Application to register and list reminders.
Each reminder has a Title, Description, Date Limit and Status.

## Technical Objective

This application was developed with Asp.Net Core. 
The propos is show the knowledge using this techology and applying best code practices.

## Technical Details

### Backend - Asp.Net Standard v.2.1 Class Library

> *Reminders.Business, Reminders.Domain, Reminders.Core and Reminders.Context*
- Patterns: Domain-Driven Design (DDD), Model-View-ViewModel (MVVM), Repository and Unit of Work.
- Object Relational Mapping (ORM): Microsoft Entity Framework Core v.2.1.
- Using In Memory Database.

### Test - Asp.Net Core v.2.0 MS Test Application

> *Reminders.Api.Test, Reminders.Business.Test and Reminders.Mvc.Test*
- ChromeRemindersCRUD, EdgeRemindersCRUD, FirefoxRemindersCRUD, RemindersApiCRUD, RemindersBusinessCRUD and RemindersRepositoryCRUD.
- Repository and Unit of Work Test.
- Api Test.
- Browsers Test using Selenium Web Driver v.3.1.
  - Chrome, Firefox and Edge.

### Frontend - [Asp.Net Core MVC](http://reminders.azurewebsites.net/)

> *Reminders.Mvc*
- App Layer is using Asp.Net Core MVC v.2.1.
- Responsive application using Bootstrap Framework v4.1 and jQuery 1.9.1.

### Web Api - [Asp.Net Core Web Api](http://reminderswebapi.azurewebsites.net/)

> *Reminders.Api*
- App Layer is using Asp.Net Core Web Api v2.1.
- Using Swagger UI v.3.0 for API documentation.

### How to configure

- Web Api Test will use the "UrlApi" key inside the project [Reminders.Api.Test](https://github.com/KaueReinbold/Reminders/blob/master/Reminders.Test/Reminders.Api.Test/appsettings.json)
- MVC Test will use the "UrlApplicationMvc" key inside the project [Reminders.Mvc.Test](https://github.com/KaueReinbold/Reminders/blob/master/Reminders.Test/Reminders.Mvc.Test/appsettings.json)