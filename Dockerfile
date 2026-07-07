# Base runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

# Build image
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["ProjectTaskManagementAPI.csproj", "."]
RUN dotnet restore "ProjectTaskManagementAPI.csproj"

COPY . .

RUN dotnet publish "ProjectTaskManagementAPI.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "ProjectTaskManagementAPI.dll"]