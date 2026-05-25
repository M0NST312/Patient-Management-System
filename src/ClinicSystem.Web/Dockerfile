FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy project files (use repo root 'src' as context so all project references are available)
COPY ["src/ClinicSystem.Web/ClinicSystem.Web.csproj", "ClinicSystem.Web/"]
COPY ["src/ClinicSystem.Application/ClinicSystem.Application.csproj", "ClinicSystem.Application/"]
COPY ["src/ClinicSystem.Domain/ClinicSystem.Domain.csproj", "ClinicSystem.Domain/"]
COPY ["src/ClinicSystem.Infrastructure/ClinicSystem.Infrastructure.csproj", "ClinicSystem.Infrastructure/"]

RUN dotnet restore "ClinicSystem.Web/ClinicSystem.Web.csproj"

# Copy remaining source files
COPY src/ .

WORKDIR /src/ClinicSystem.Web
RUN dotnet publish "ClinicSystem.Web.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

ENV ASPNETCORE_URLS=http://+:5000
EXPOSE 5000

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "ClinicSystem.Web.dll"]