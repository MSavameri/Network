# Network Infrastructure

This repository contains a web application used to manage network assets such as servers or services. It is built with **ASP.NET Core 9** and uses Entity Framework Core, AutoMapper and FluentValidation. User login is validated against an LDAP directory.

## Features

- CRUD operations for network assets
- LDAP based authentication using `Novell.Directory.Ldap`
- MVC views styled with Bootstrap and icons
- Unit tests with MSTest, Moq and FluentAssertions
- Dockerfile and CI workflow for container builds

## Requirements

- [.NET 9 SDK](https://dotnet.microsoft.com/) or newer
- SQL Server instance for the `NetConnection` string in `appsettings.json`

## Building and running

Clone the repo and restore dependencies:

```bash
 dotnet restore
 dotnet build
 dotnet run --project src/NetworkInfrastructure.Web
```

The application will start on `http://localhost:5062` when using the included launch settings.

## Running tests

Execute the unit tests using:

```bash
 dotnet test
```

## Docker

A `Dockerfile` is included. You can build and run locally:

```bash
 docker build -t meysam57/networkinfrastructureweb .
 docker run -d -p 80:80 meysam57/networkinfrastructureweb
```

The image is also published to Docker Hub and can be pulled directly:

```bash
 docker pull meysam57/networkinfrastructureweb:latest
```

Docker Hub repository: <https://hub.docker.com/r/meysam57/networkinfrastructureweb>

## Continuous Integration

A GitHub Actions workflow builds and pushes the Docker image on every push. See `.github/workflows/docker-publish.yml` for details.
