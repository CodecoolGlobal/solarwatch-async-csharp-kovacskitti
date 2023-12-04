# Dockerfile
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build-env
WORKDIR /app

# Copy csproj and restore as distinct layers


# Copy everything else and build
COPY . ./
RUN dotnet restore
RUN dotnet publish -c Release -o out

# Install dotnet-ef globally
RUN dotnet tool install --global dotnet-ef --version 7.0.0

#Migration
CMD ["dotnet", "ef", "database", "update", "--project", "YourDbContextProjectName"]

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:7.0
WORKDIR /app
COPY --from=build-env /app/out .
ENTRYPOINT ["dotnet", "SolarWatch.dll"]

ENV ASPNETCORE_URLS=http://+:80