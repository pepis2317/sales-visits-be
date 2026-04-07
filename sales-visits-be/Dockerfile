FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 5075

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["sales-visits-be/sales-visits-be.csproj", "sales-visits-be/"]
COPY ["entities/entities.csproj", "entities/"]
RUN dotnet restore "sales-visits-be/sales-visits-be.csproj"
COPY . .
WORKDIR "/src/sales-visits-be"
RUN dotnet build "./sales-visits-be.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./sales-visits-be.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "sales-visits-be.dll"]
