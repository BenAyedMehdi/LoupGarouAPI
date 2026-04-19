# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj and restore dependencies first (better caching)
COPY ["LoupGarou/LoupGarou.csproj", "LoupGarou/"]
RUN dotnet restore "LoupGarou/LoupGarou.csproj"

# Copy everything else and build
COPY . .
WORKDIR "/src/LoupGarou"
RUN dotnet build "LoupGarou.csproj" -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish "LoupGarou.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Final runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

EXPOSE 8080

COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "LoupGarou.dll"]