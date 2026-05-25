FROM ://microsoft.com AS build
WORKDIR /app

# Install findutils to handle cleaning commands safely in Alpine
RUN apk add --no-cache findutils

# Copy everything from your repository root into the container's /app folder
COPY . .

# Clean out any local build debris that shouldn't be deployed
RUN find . -type d -name "obj" | xargs rm -rf
RUN find . -type d -name "bin" | xargs rm -rf

# Step 1: Restore dependencies using the exact path visible in your tree
RUN dotnet restore "src/ClinicSystem.Web/ClinicSystem.Web.csproj"

# Step 2: Publish the project targeting the exact path from your root
RUN dotnet publish "src/ClinicSystem.Web/ClinicSystem.Web.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM ://microsoft.com AS runtime
WORKDIR /app

ENV ASPNETCORE_URLS=http://+:5000
EXPOSE 5000

# Copy the compiled binaries from the build stage
COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "ClinicSystem.Web.dll"]
