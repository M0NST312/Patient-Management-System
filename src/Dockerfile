FROM ://microsoft.com AS build
WORKDIR /src

# Installs findutils to support your cleanup commands
RUN apk add --no-cache findutils

COPY . .

# Cleans up existing build artifacts safely
RUN find . -type d -name "obj" | xargs rm -rf
RUN find . -type d -name "bin" | xargs rm -rf

# Restores using the exact repository subfolder structure
RUN dotnet restore "src/ClinicSystem.Web/ClinicSystem.Web.csproj"

# Navigates to the project directory and publishes
WORKDIR /src/src/ClinicSystem.Web
RUN dotnet publish "ClinicSystem.Web.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM ://microsoft.com AS runtime
WORKDIR /app

ENV ASPNETCORE_URLS=http://+:5000
EXPOSE 5000

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "ClinicSystem.Web.dll"]
