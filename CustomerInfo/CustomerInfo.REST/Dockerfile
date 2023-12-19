#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# Use the latest version of .NET for the base image.
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# Use Microsoft's official build .NET image.
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy local code to the container image.
COPY ["./CustomerInfo.REST.csproj", "CustomerInfo.REST/"]
RUN dotnet restore "./CustomerInfo.REST/CustomerInfo.REST.csproj"
COPY . .
WORKDIR "/src/CustomerInfo.REST"
RUN dotnet build "./CustomerInfo.REST.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./CustomerInfo.REST.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CustomerInfo.dll"]