#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.


# Use Microsoft's official build .NET image.
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copy local code to the container image.
COPY ["./*.csproj", "./"]
RUN dotnet restore

# Copy everything else and build.
COPY [".", "./"]
RUN dotnet publish "./CustomerInfo.REST.csproj" -c Release -o out

# Use Microsoft's official runtime .NET image.
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/out .

# Configure the container to run the application.
ENTRYPOINT ["dotnet", "CustomerInfo.REST.dll"]