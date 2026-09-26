# ビルド
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY Directory.Build.props GeoaiApi.sln ./
COPY src/GeoaiApi/GeoaiApi.csproj src/GeoaiApi/packages.lock.json src/GeoaiApi/
RUN dotnet restore src/GeoaiApi/GeoaiApi.csproj --locked-mode
COPY src/ src/
RUN dotnet publish src/GeoaiApi/GeoaiApi.csproj --no-restore --configuration Release --output /app

# 実行（ポート 8080。spec.md 3.2）
FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY --from=build /app .
ENV ASPNETCORE_HTTP_PORTS=8080
EXPOSE 8080
ENTRYPOINT ["dotnet", "GeoaiApi.dll"]
