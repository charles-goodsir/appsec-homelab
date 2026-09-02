# syntax=docker/dockerfile:1

# --- build stage ---
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY AppSecLab.Api.csproj ./
RUN dotnet restore AppSecLab.Api.csproj

COPY . ./
RUN dotnet publish AppSecLab.Api.csproj -c Release -o /app --no-restore

# --- runtime stage ---
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app ./

# aspnet:8.0 images already default to ASPNETCORE_HTTP_PORTS=8080
EXPOSE 8080
ENTRYPOINT ["dotnet", "AppSecLab.Api.dll"]
