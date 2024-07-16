FROM mcr.microsoft.com/dotnet/aspnet:6.0@sha256:c9547e412ce5a6ccb27e75a25672a007a7a0d6a61bbd7b73cc7d53d9e7d93b1c AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0@sha256:2c83e0a1f2097ed9ade26584f07861b47acb8028db5c41c6cf4cc4d074940dc5 AS build
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
