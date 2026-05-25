FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY . .

RUN find . -type d -name "obj" | xargs rm -rf
RUN find . -type d -name "bin" | xargs rm -rf

RUN dotnet restore "ClinicSystem.Web/ClinicSystem.Web.csproj"

WORKDIR /src/ClinicSystem.Web

# This will show us exactly where the glob includes are coming from
RUN dotnet msbuild -preprocess:preprocessed.xml ClinicSystem.Web.csproj && grep -n "\*\*\/\*\." preprocessed.xml | head -30

RUN dotnet publish "ClinicSystem.Web.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
ENV ASPNETCORE_URLS=http://+:5000
EXPOSE 5000
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "ClinicSystem.Web.dll"]