FROM mcr.microsoft.com/dotnet/sdk:8.0 AS builder

WORKDIR /src
COPY ["UserService/NewsShelf.UserService-main/NewsShelf.UserService.Api/NewsShelf.UserService.Api.csproj", "UserService/"]
RUN dotnet restore "UserService/NewsShelf.UserService.Api.csproj"

COPY . .
RUN dotnet build "UserService/NewsShelf.UserService-main/NewsShelf.UserService.Api/NewsShelf.UserService.Api.csproj" -c Release -o /app/build

FROM builder AS publish
RUN dotnet publish "UserService/NewsShelf.UserService-main/NewsShelf.UserService.Api/NewsShelf.UserService.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=publish /app/publish .

ENV ASPNETCORE_URLS=http://+:5001
EXPOSE 5001

HEALTHCHECK --interval=30s --timeout=3s --start-period=40s --retries=3 \
    CMD curl -f http://localhost:5001/health || exit 1

ENTRYPOINT ["dotnet", "NewsShelf.UserService.Api.dll"]
