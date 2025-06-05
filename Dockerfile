# Use the official ASP.NET Core runtime as a parent image
FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

# Use the official .NET SDK as a build image
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["src/NetworkInfrastructure.Web/NetworkInfrastructure.Web.csproj", "NetworkInfrastructure.Web/"]
# Copy other .csproj files if there are any in the solution that NetworkInfrastructure.Web depends on
# For example: COPY ["src/NetworkInfrastructure.AnotherProject/NetworkInfrastructure.AnotherProject.csproj", "NetworkInfrastructure.AnotherProject/"]
RUN dotnet restore "NetworkInfrastructure.Web/NetworkInfrastructure.Web.csproj"
COPY . .
WORKDIR "/src/NetworkInfrastructure.Web"
RUN dotnet build "NetworkInfrastructure.Web.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "NetworkInfrastructure.Web.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Build runtime image
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "NetworkInfrastructure.Web.dll"]
