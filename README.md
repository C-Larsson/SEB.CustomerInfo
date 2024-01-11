# ABC.CustomerInfo
Implementation of code task given by ABC

# Description
The solution is implemented using .NET 8 with a ASP.NET Core project for REST API and an xUnit project for unit tests.
Four end points have been implemented to retrieve, create, update and delete customer records.
Entity Framework Core is used with an in-memory database.

# GitHub Repository URL:
https://github.com/C-Larsson/ABC.CustomerInfo

# Prerequisites
Visual Studio 2022,
Docker Desktop,
Git

# Setting up development environment
In Visual Studio, clone the GitHub Repository URL.
To test end points in Swagger, run CustomerInfo.REST project and navigate browser to https://localhost:7251/swagger/index.html

# Unit tests
10 cases are testing the integration of the four end points, and 22 cases are validations of the CustomerInfo details.

# Installation with Docker
Open a terminal and navigate to your project directory.
Build Docker image with command: 
docker build -t customerinfo .
Run the container using command: 
docker run -d -p 8080:8080 --name myapp customerinfo

# Contact
Christoffer Larsson, larsson.christoffer@gmail.com