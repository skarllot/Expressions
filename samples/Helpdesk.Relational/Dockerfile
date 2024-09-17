FROM mcr.microsoft.com/dotnet/aspnet:6.0@sha256:a5fce63da9e79bb68f2911b4d4b69c4c28d393cd3066fc33fe10824b3bcc24ee AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0@sha256:84eb54349c50bbb5792d989cc5c65b9006ccc298e8169320718dd65c78856ffc AS build
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
