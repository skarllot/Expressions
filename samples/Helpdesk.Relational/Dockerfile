FROM mcr.microsoft.com/dotnet/aspnet:6.0@sha256:9772f0741a91ab07aa38f94c41f721691c9979e8b2c7961b3cc34c8f34a65fe7 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0@sha256:3e7491422fb3a14f0bfe98fcb58afc1d477ee6ee4aac8dff8557332140ac9cf1 AS build
WORKDIR /src
COPY . .
WORKDIR "/src/samples/Helpdesk.Relational"
RUN dotnet restore
RUN dotnet build "Helpdesk.Relational.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Helpdesk.Relational.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Raiqub.Helpdesk.Relational.dll"]
