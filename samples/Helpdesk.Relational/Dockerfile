FROM mcr.microsoft.com/dotnet/aspnet:6.0@sha256:78db2415ff20e22d2f3fd6078a8fff5e17f327729d34e1d94d926a02a2c41520 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0@sha256:e5a27c7f2a5f679f994106aa330585a7108e2812a8367e7686c13451e7fd27a0 AS build
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
