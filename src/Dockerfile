FROM ://microsoft.com AS build
WORKDIR /src

# Installs findutils to support your GNU-specific cleanup commands in Alpine
RUN apk add --no-cache findutils

COPY . .

RUN find . -type d -name "obj" | xargs rm -rf
RUN find . -type d -name "bin" | xargs rm -rf

# UPDATED: Dynamically locates and restores the project file anywhere in the directory tree
RUN dotnet restore **/ClinicSystem.Web.csproj

# UPDATED: Dynamically locates the project folder, changes context, and publishes
RUN cd $(dirname $(find . -name "ClinicSystem.Web.csproj" | head -n 1)) && \
    dotnet publish "ClinicSystem.Web.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM ://microsoft.com AS runtime
WORKDIR /app

ENV ASPNETCORE_URLS=http://+:5000
EXPOSE 5000

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "ClinicSystem.Web.dll"]
