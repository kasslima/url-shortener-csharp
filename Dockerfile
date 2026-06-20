FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY ["UrlShortener.Api/UrlShortener.Api.csproj", "UrlShortener.Api/"]
COPY ["UrlShortener.Application/UrlShortener.Application.csproj", "UrlShortener.Application/"]
COPY ["UrlShortener.Domain/UrlShortener.Domain.csproj", "UrlShortener.Domain/"]
COPY ["UrlShortener.Infrastructure/UrlShortener.Infrastructure.csproj", "UrlShortener.Infrastructure/"]
RUN dotnet restore "UrlShortener.Api/UrlShortener.Api.csproj"

COPY . .
WORKDIR /src/UrlShortener.Api
RUN dotnet publish "UrlShortener.Api.csproj" \
    --configuration Release \
    --output /app/publish \
    --no-restore \
    /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

ENV ASPNETCORE_HTTP_PORTS=8080
EXPOSE 8080

USER $APP_UID
ENTRYPOINT ["dotnet", "UrlShortener.Api.dll"]
