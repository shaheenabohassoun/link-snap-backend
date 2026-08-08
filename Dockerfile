# Build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY LinkSnap.Domain/LinkSnap.Domain.csproj LinkSnap.Domain/
COPY LinkSnap.Application/LinkSnap.Application.csproj LinkSnap.Application/
COPY LinkSnap.Infrastructure/LinkSnap.Infrastructure.csproj LinkSnap.Infrastructure/
COPY LinkSnap.API/LinkSnap.API.csproj LinkSnap.API/

RUN dotnet restore LinkSnap.API/LinkSnap.API.csproj

COPY . .
RUN dotnet publish LinkSnap.API/LinkSnap.API.csproj -c Release -o /app/publish /p:UseAppHost=false

# Runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
ENV ASPNETCORE_ENVIRONMENT=Production
EXPOSE 8080
ENTRYPOINT ["dotnet", "LinkSnap.API.dll"]
