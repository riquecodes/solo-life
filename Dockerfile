# Build multi-stage: publica a API e roda numa imagem aspnet enxuta.
# As imagens base do .NET 10 sao multi-arch (buildam em arm64 na VM Oracle Ampere).
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY . .
RUN dotnet restore src/SoloLife.Api/SoloLife.Api.csproj
RUN dotnet publish src/SoloLife.Api/SoloLife.Api.csproj -c Release -o /app --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY --from=build /app .
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
ENTRYPOINT ["dotnet", "SoloLife.Api.dll"]
