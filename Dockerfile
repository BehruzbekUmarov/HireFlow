FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY HireFlow.sln .
COPY src/HireFlow.Domain/HireFlow.Domain.csproj src/HireFlow.Domain/
COPY src/HireFlow.Application/HireFlow.Application.csproj src/HireFlow.Application/
COPY src/HireFlow.Infrastructure/HireFlow.Infrastructure.csproj src/HireFlow.Infrastructure/
COPY src/HireFlow.Api/HireFlow.Api.csproj src/HireFlow.Api/

RUN dotnet restore

COPY src/ src/

RUN dotnet publish src/HireFlow.Api/HireFlow.Api.csproj -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 8080
ENTRYPOINT ["dotnet", "HireFlow.Api.dll"]