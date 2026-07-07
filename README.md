# School Management System

A web application built to manage day-to-day school operations efficiently. 

## Features Done
* **Student Management:** Register new students, update profiles, and manage records.
* **Teacher Records:** Track faculty information, assigned subjects, and details.
* **Class & Section Control:** Assign teachers to classes and group students into sections.
* **Relational Database:** Data storage using Entity Framework Core to map relationships.

## Built With
* **Language:** C#
* **Framework:** 
 Core
* **ORM / Database Access:** Entity Framework Core & LINQ
* **Database:** Microsoft SQL Server

## Setup Instructions
1. Clone the repository: `git clone https://github.com`
2. Open the project in Visual Studio or VS Code.
3. Update the connection string in `appsettings.json` to match your local SQL Server.
4. Run `dotnet ef database update` in the terminal to apply database migrations.
5. Run the application using `dotnet run`.
