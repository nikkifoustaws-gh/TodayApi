# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy project file and restore dependencies (layer caching optimization)
COPY TodayApi.csproj .
RUN dotnet restore

# Copy remaining source code and build
COPY . .
RUN dotnet publish -c Release -o /app/publish --no-restore

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Create non-root user for security
RUN adduser --disabled-password --gecos "" appuser

# Copy published application
COPY --from=build /app/publish .

# Set environment variables
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production
ENV DOTNET_RUNNING_IN_CONTAINER=true
ENV TZ=America/New_York

# Expose port (Azure Container Apps/App Service typically uses 8080)
EXPOSE 8080

# Switch to non-root user
USER appuser

# Start the application
ENTRYPOINT ["dotnet", "TodayApi.dll"]
