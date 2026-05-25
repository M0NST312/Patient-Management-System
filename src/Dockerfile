FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /repo

COPY . .

RUN find . -type d -name "obj" | xargs rm -rf
RUN find . -type d -name "bin" | xargs rm -rf

RUN dotnet restore "src/ClinicSystem.Web/ClinicSystem.Web.csproj"

WORKDIR /repo/src/ClinicSystem.Web

RUN find /repo/src/ClinicSystem.Web -name "*.cs" -not -path "*/obj/*" | head -20

RUN dotnet msbuild -preprocess:preprocessed.xml ClinicSystem.Web.csproj && grep -n "razor\|cshtml\|Compile Include" preprocessed.xml | head -30

RUN dotnet publish "ClinicSystem.Web.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
ENV ASPNETCORE_URLS=http://+:5000
EXPOSE 5000
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "ClinicSystem.Web.dll"]