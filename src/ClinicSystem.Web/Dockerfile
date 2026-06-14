FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 5000
ENV ASPNETCORE_URLS=http://+:5000
USER app

FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG configuration=Release
WORKDIR /src
COPY ["src/ClinicSystem.Web/ClinicSystem.Web.csproj", "src/ClinicSystem.Web/"]
RUN dotnet restore "src/ClinicSystem.Web/ClinicSystem.Web.csproj"
COPY . .

# 🔍 DEBUG — remove these after fixing
RUN echo "=== Files in /src/src/ClinicSystem.Web/ ===" && \
    ls -la /src/src/ClinicSystem.Web/ && \
    echo "=== .cs files ===" && \
    find /src/src/ClinicSystem.Web -name "*.cs" | head -10 && \
    echo "=== .razor files ===" && \
    find /src/src/ClinicSystem.Web -name "*.razor" | head -10

WORKDIR "/src/src/ClinicSystem.Web"
RUN dotnet build "ClinicSystem.Web.csproj" -c $configuration -o /app/build

FROM build AS publish
ARG configuration=Release
RUN dotnet publish "ClinicSystem.Web.csproj" -c $configuration -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ClinicSystem.Web.dll"]