FROM ://microsoft.com AS build
WORKDIR /src

# Installs findutils to support your file-clearing commands
RUN apk add --no-cache findutils

COPY . .

# Cleans build residue safely
RUN find . -type d -name "obj" | xargs rm -rf
RUN find . -type d -name "bin" | xargs rm -rf

# FIX: Native shell logic finds the project directory and executes restore/publish directly inside it
RUN export TARGET_DIR=$(dirname $(find . -name "ClinicSystem.Web.csproj" | head -n 1)) && \
    cd "$TARGET_DIR" && \
    dotnet restore && \
    dotnet publish -c Release -o /app/publish /p:UseAppHost=false

FROM ://microsoft.com AS runtime
WORKDIR /app

ENV ASPNETCORE_URLS=http://+:5000
EXPOSE 5000

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "ClinicSystem.Web.dll"]
