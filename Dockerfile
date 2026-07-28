# ---------------------------------------------
# STAGE 1 — BUILD
# Uses the full SDK image to compile the project
# ---------------------------------------------
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy solution and project files first — Docker caches this layer.
# If only your code changes (not .csproj), this layer is reused
# and dotnet restore is NOT re-run. Saves 30-60 seconds per build.
COPY HireFlow.sln .
COPY src/HireFlow.Domain/HireFlow.Domain.csproj             src/HireFlow.Domain/
COPY src/HireFlow.Application/HireFlow.Application.csproj   src/HireFlow.Application/
COPY src/HireFlow.Infrastructure/HireFlow.Infrastructure.csproj src/HireFlow.Infrastructure/
COPY src/HireFlow.Api/HireFlow.Api.csproj                   src/HireFlow.Api/

# Restore all packages — cached unless .csproj files change
RUN dotnet restore

# Now copy the rest of the source code
COPY src/ src/

# Build and publish in Release mode to /app/publish
RUN dotnet publish src/HireFlow.Api/HireFlow.Api.csproj \
    -c Release \
    -o /app/publish \
    --no-restore

# ---------------------------------------------
# STAGE 2 — RUNTIME
# Uses the smaller ASP.NET runtime image (no SDK)
# Final image is ~200MB instead of ~700MB
# ---------------------------------------------
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Copy only the published output from the build stage
# The SDK and source code are NOT included in the final image
COPY --from=build /app/publish .

# The port your .NET app listens on inside the container
EXPOSE 8080

# Start the application
ENTRYPOINT ["dotnet", "HireFlow.Api.dll"]