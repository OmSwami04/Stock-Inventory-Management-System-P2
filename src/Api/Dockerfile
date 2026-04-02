# Build Stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /app

# Copy csproj files and restore
COPY ["src/Api/InventoryManagement.Api.csproj", "src/Api/"]
COPY ["src/Application/InventoryManagement.Application.csproj", "src/Application/"]
COPY ["src/Domain/InventoryManagement.Domain.csproj", "src/Domain/"]
COPY ["src/Infrastructure/InventoryManagement.Infrastructure.csproj", "src/Infrastructure/"]
COPY ["src/Interfaces/InventoryManagement.Interfaces.csproj", "src/Interfaces/"]
COPY ["src/Shared/InventoryManagement.Shared.csproj", "src/Shared/"]

RUN dotnet restore "src/Api/InventoryManagement.Api.csproj"

# Copy everything else and build
COPY . .
WORKDIR "/app/src/Api"
RUN dotnet publish "InventoryManagement.Api.csproj" -c Release -o /app/publish

# Final Stage
FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY --from=build /app/publish .

# Environment variables
ENV ASPNETCORE_URLS=http://+:5153
EXPOSE 5153

ENTRYPOINT ["dotnet", "InventoryManagement.Api.dll"]
