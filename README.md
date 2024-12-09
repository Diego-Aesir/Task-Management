This is a project developed with ASP.NET 8, using Docker to simplify the setup and execution of the application.

Before you begin, ensure you have the following installed:
- Visual Studio
- Docker Hub
- .NET 8 SDK

============================================================================================

Steps to run this project:

1 - Clone this repository

2 - Restore Dependencies 

3 - Start Docker Container

4 - Apply Database Migrations

5 - Access the application via UserAPI (http://localhost:8080/swagger)

============================================================================================

(2) - In order to Restore Dependecies, you need to enter on EACH API via Visual Studio and open the terminal (ctrl + `) and apply this command: dotnet restore. But if you had oppenned via Visual Studio, there is a chance that the IDE already had done this. 

(3) - To start the application, you just need to open a CMD on the root, and write: docker-compose up --build. It's better if you use Docker Hub so you can have visual on the containers.

(4) - Once the Docker container are all green (ready to use), just enter on UserAPI and TaskAPI via Visual Studio and as the step 2, you need to write on EACH terminal "dotnet ef database update"

By now you're good to go. This projects consists on creating a Users and setting Tasks via RabbitMQ.
