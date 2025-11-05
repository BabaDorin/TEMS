# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy solution file
COPY ["Tems.sln", "./"]

# Copy all project files
COPY ["Tems.Host/Tems.Host.csproj", "Tems.Host/"]
COPY ["Tems.Common/Tems.Common.csproj", "Tems.Common/"]
COPY ["Modules/Example/Example.API/Example.API.csproj", "Modules/Example/Example.API/"]
COPY ["Modules/Example/Example.Contract/Example.Contract.csproj", "Modules/Example/Example.Contract/"]
COPY ["Modules/Example/Example.Application/Example.Application.csproj", "Modules/Example/Example.Application/"]
COPY ["Modules/Example/Example.Infrastructure/Example.Infrastructure.csproj", "Modules/Example/Example.Infrastructure/"]

# Restore dependencies
RUN dotnet restore "Tems.Host/Tems.Host.csproj"

# Copy everything else
COPY . .

# Build and publish
WORKDIR "/src/Tems.Host"
RUN dotnet build "Tems.Host.csproj" -c $BUILD_CONFIGURATION -o /app/build
RUN dotnet publish "Tems.Host.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app

# Create non-root user for security
RUN groupadd -r appuser && useradd -r -g appuser appuser

# Copy published app
COPY --from=build /app/publish .

# Set ownership
RUN chown -R appuser:appuser /app

# Switch to non-root user
USER appuser

# Azure App Service expects port 8080 by default (or you can configure via WEBSITES_PORT)
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

# Health check
HEALTHCHECK --interval=30s --timeout=3s --start-period=5s --retries=3 \
  CMD curl -f http://localhost:8080/health || exit 1

ENTRYPOINT ["dotnet", "Tems.Host.dll"]