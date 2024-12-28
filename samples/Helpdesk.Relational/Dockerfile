FROM mcr.microsoft.com/dotnet/aspnet:6.0@sha256:57abdb74d36f8cd459d87da732fb616a31b78b0042096760d31d51fa9c431d5e AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0@sha256:aef9e9f5740c84bffec6344269fba85d97e0065c376beec24a46c07b0063d836 AS build
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
