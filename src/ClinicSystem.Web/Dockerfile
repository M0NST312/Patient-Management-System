FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy project files (use repo root 'src' as context so all project references are available)
# Copy full source tree first so MSBuild globs expand correctly during restore/publish
COPY src/ .

RUN dotnet restore "ClinicSystem.Web/ClinicSystem.Web.csproj"

WORKDIR /src/ClinicSystem.Web
RUN dotnet publish "ClinicSystem.Web.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

ENV ASPNETCORE_URLS=http://+:5000
EXPOSE 5000

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "ClinicSystem.Web.dll"]