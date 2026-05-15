FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY VitalPhotography.Api/VitalPhotography.Api.csproj VitalPhotography.Api/
RUN dotnet restore VitalPhotography.Api/VitalPhotography.Api.csproj
COPY . .
RUN dotnet publish VitalPhotography.Api/VitalPhotography.Api.csproj \
    -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

# SQLite lives on a Fly persistent volume mounted at /data
RUN mkdir -p /data
ENV ConnectionStrings__DefaultConnection="Data Source=/data/vital-photography.db"
ENV ASPNETCORE_URLS="http://+:8080"

EXPOSE 8080
ENTRYPOINT ["dotnet", "VitalPhotography.Api.dll"]
