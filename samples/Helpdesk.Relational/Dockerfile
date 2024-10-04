FROM mcr.microsoft.com/dotnet/aspnet:6.0@sha256:7155a73c14fa51599ea17597c1ef8834d12acacb68b6c867a4eee091b6e47d72 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0@sha256:a596e30591ac26f5d5299ceffe41ca5c654768ddb8349f6dc05c51f7e985472f AS build
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
