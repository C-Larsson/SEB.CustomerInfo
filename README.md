# SEB.CustomerInfo
Implementation of code task given by SEB

# Description
The solution is implemented using .NET 8 with a ASP.NET Core project for REST API and an xUnit project for unit tests.
Four end points have been implemented to retrieve, create, update and delete customer records.
The CustomerInfo Controller is using a CustomerInfo Service containing business logic and database management.
A MockDbContext class is simulating the database.

# GitHub Repository URL:
https://github.com/C-Larsson/SEB.CustomerInfo.git

# Swagger URL:
https://localhost:7251/swagger/index.html

# Prerequisites
Visual Studio 2022 (latest version)
Docker Desktop
Git

# Installation
In Visual Studio, clone the GitHub Repository URL.
Run CustomerInfo.REST project
Test end points in a browser using the Swagger URL 

# Unit tests
32 unit test have been implmented to test the solution.
10 test cases are testing the four end points in the controller, and 22 test cases are validations of the CostomerInfo details.
The tests can be run using Test Explorer in Visual Studio.

# Building and running with Docker
Open a terminal, navigate to your project directory, and run the following command to build the Docker image: 
docker build -t customerinfo .

Once the image is built, run it as a container with the following command:
docker run -d -p 8080:80 --name myapp yourappname

# Known Issues
The build script for Docker generates an error and needs to be fixed

# Contact
Christoffer Larsson, larsson.christoffer@gmail.com