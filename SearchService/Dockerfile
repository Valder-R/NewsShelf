FROM mcr.microsoft.com/dotnet/sdk:8.0 AS builder

WORKDIR /src
COPY ["SearchService/NewsService/BusinessLogic/BusinessLogic.csproj", "SearchService/BusinessLogic/"]
COPY ["SearchService/NewsService/DataAccess/DataAccess.csproj", "SearchService/DataAccess/"]
COPY ["SearchService/NewsService/NewsService/NewsApi.csproj", "SearchService/NewsService/"]

RUN dotnet restore "SearchService/NewsService/NewsApi.csproj"

COPY . .
RUN dotnet build "SearchService/NewsService/NewsService/NewsApi.csproj" -c Release -o /app/build

FROM builder AS publish
RUN dotnet publish "SearchService/NewsService/NewsService/NewsApi.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=publish /app/publish .

ENV ASPNETCORE_URLS=http://+:5002
EXPOSE 5002

HEALTHCHECK --interval=30s --timeout=3s --start-period=40s --retries=3 \
    CMD curl -f http://localhost:5002/health || exit 1

ENTRYPOINT ["dotnet", "NewsApi.dll"]
