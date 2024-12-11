FROM mcr.microsoft.com/dotnet/aspnet:6.0@sha256:0faa0ce89de2527c7c4b5dd878875898fbc8225733d8013f890f6e5c54dc2018 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0@sha256:696d87a7980a9b84d8a1a4d56d1d2287ed13a7cf62e0d2a971ecc5648669fc39 AS build
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
