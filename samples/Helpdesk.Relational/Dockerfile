FROM mcr.microsoft.com/dotnet/aspnet:8.0.22@sha256:e9684fdc74056bda84e8f5918c7211cac837a979a26b8a56c4e424f1ab94c80b AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0.416@sha256:a87db299d259cec53210df406cd5e51f900a0c6d938fe56d5f392629c0505d75 AS build
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
