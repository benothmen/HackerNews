# Build stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy project files first (better layer caching)
COPY Directory.Build.props ./
COPY Directory.Packages.props ./
COPY src/Api/Api.csproj src/Api/
COPY src/Application/Application.csproj src/Application/
COPY src/Domain/Domain.csproj src/Domain/
COPY src/Infrastructure/Infrastructure.csproj src/Infrastructure/

RUN dotnet restore src/Api/Api.csproj

# Copy source
COPY src/ ./src/

# Publish
RUN dotnet publish src/Api/Api.csproj -c Release -o /app/publish --no-restore

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

# ASP.NET Core defaults to port 8080 in containers for recent versions; expose for clarity
EXPOSE 8080

ENV ASPNETCORE_URLS=http://+:8080
ENTRYPOINT ["dotnet", "Api.dll"]
