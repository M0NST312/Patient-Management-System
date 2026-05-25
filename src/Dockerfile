FROM ://microsoft.com AS build
WORKDIR /build

# Install findutils to handle cleaning commands
RUN apk add --no-cache findutils

# Copy all repository files into the container
COPY . .

# Clear out any conflicting local build artifacts
RUN find . -type d -name "obj" | xargs rm -rf
RUN find . -type d -name "bin" | xargs rm -rf

# Restore the project using its exact folder path from the repo root
RUN dotnet restore "src/ClinicSystem.Web/ClinicSystem.Web.csproj"

# Step directly into the project folder to publish it
WORKDIR /build/src/ClinicSystem.Web
RUN dotnet publish "ClinicSystem.Web.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM ://microsoft.com AS runtime
WORKDIR /app

ENV ASPNETCORE_URLS=http://+:5000
EXPOSE 5000

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "ClinicSystem.Web.dll"]
