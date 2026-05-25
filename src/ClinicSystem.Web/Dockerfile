FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /app

COPY . .

WORKDIR /app/src/ClinicSystem.Web

RUN dotnet restore
RUN dotnet publish -c Release -o /publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

ENV ASPNETCORE_URLS=http://+:5000
EXPOSE 5000

COPY --from=build /publish .

ENTRYPOINT ["dotnet", "ClinicSystem.Web.dll"]