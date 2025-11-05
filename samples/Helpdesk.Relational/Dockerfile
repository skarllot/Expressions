FROM mcr.microsoft.com/dotnet/aspnet:8.0.21@sha256:95f47686d234619398df242962148324129b4400aa185986fd571da4e20e24bc AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0.415@sha256:4632e98dc703311ca09d0c37170fe70ad4ba334bd88a841c78cbcc9d29994b2c AS build
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
