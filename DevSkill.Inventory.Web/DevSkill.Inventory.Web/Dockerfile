# Use the .NET SDK as the base image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

# Use the .NET SDK as the build environment
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy the solution file and restore dependencies
COPY ["DevSkill.Inventory.Web/DevSkill.Inventory.Web.csproj", "DevSkill.Inventory.Web/"]
COPY ["DevSkill.Inventory.Application/DevSkill.Inventory.Application.csproj", "DevSkill.Inventory.Application/"]
COPY ["DevSkill.Inventory.Domain/DevSkill.Inventory.Domain.csproj", "DevSkill.Inventory.Domain/"]
COPY ["DevSkill.Inventory.Infrastructure/DevSkill.Inventory.Infrastructure.csproj", "DevSkill.Inventory.Infrastructure/"]

# Restore dependencies for all projects
RUN dotnet restore "DevSkill.Inventory.Web/DevSkill.Inventory.Web.csproj"

# Copy the remaining files
COPY . .

# Set the working directory to the web project
WORKDIR "/src/DevSkill.Inventory.Web"

# Build the web project
RUN dotnet build "DevSkill.Inventory.Web.csproj" -c Release -o /app/build

# Publish the project
FROM build AS publish
RUN dotnet publish "DevSkill.Inventory.Web.csproj" -c Release -o /app/publish

# Use the runtime image to serve the application
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DevSkill.Inventory.Web.dll"]
