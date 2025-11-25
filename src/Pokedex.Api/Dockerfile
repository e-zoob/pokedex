FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY . .

RUN dotnet restore "src/Pokedex.Api/Pokedex.Api.csproj"

RUN dotnet publish "src/Pokedex.Api" -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

ENV ASPNETCORE_URLS=http://+:5283
EXPOSE 5283

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "Pokedex.Api.dll"]