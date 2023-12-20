# SEB.CustomerInfo
Implementation of code task given by SEB

# Description
The solution is implemented using .NET 8 with a ASP.NET Core project for REST API and an xUnit project for unit tests.
Four end points have been implemented to retrieve, create, update and delete customer records.
The CustomerInfo Controller is using a CustomerInfo Service containing business logic and database management.
A MockDbContext class is simulating the database.

# GitHub Repository URL:
https://github.com/C-Larsson/SEB.CustomerInfo.git

# Prerequisites
Visual Studio 2022 (latest version),
Docker Desktop,
Git

# Setting up development environment
In Visual Studio, clone the GitHub Repository URL.
To test end points in Swagger, run CustomerInfo.REST project and navigate browser to https://localhost:7251/swagger/index.html

# Unit tests
32 unit test have been implmented to test the solution.
10 test cases are testing the four end points in the controller, and 22 test cases are validations of the CostomerInfo details.
The tests can be run using Test Explorer in Visual Studio.

# Installation with Docker
Open a terminal and navigate to your project directory.
Build Docker image with command: 
docker build -t customerinfo .
Run the container using command: 
docker run -d -p 8080:8080 --name myapp customerinfo



# Contact
Christoffer Larsson, larsson.christoffer@gmail.com