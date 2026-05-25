FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj files
COPY src/ClinicSystem.Web/ClinicSystem.Web.csproj src/ClinicSystem.Web/
COPY src/ClinicSystem.Application/ClinicSystem.Application.csproj src/ClinicSystem.Application/
COPY src/ClinicSystem.Domain/ClinicSystem.Domain.csproj src/ClinicSystem.Domain/
COPY src/ClinicSystem.Infrastructure/ClinicSystem.Infrastructure.csproj src/ClinicSystem.Infrastructure/

# Restore
RUN dotnet restore src/ClinicSystem.Web/ClinicSystem.Web.csproj

# Copy everything
COPY . .

# Publish
WORKDIR /src/src/ClinicSystem.Web
RUN dotnet publish -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

ENV ASPNETCORE_URLS=http://+:5000
EXPOSE 5000

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "ClinicSystem.Web.dll"]