FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["Tems.IdentityServer/Tems.IdentityServer.csproj", "Tems.IdentityServer/"]
COPY ["Tems.Common/Tems.Common.csproj", "Tems.Common/"]
RUN dotnet restore "Tems.IdentityServer/Tems.IdentityServer.csproj"
COPY . .
WORKDIR "/src/Tems.IdentityServer"
RUN dotnet build "Tems.IdentityServer.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Tems.IdentityServer.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Tems.IdentityServer.dll"]
