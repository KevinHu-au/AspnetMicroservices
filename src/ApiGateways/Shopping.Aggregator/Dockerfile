FROM mcr.microsoft.com/dotnet/aspnet:6.0-focal AS base
WORKDIR /app
EXPOSE 80


FROM mcr.microsoft.com/dotnet/sdk:6.0-focal AS build
WORKDIR /src
COPY ["ApiGateways/Shopping.Aggregator/Shopping.Aggregator.csproj", "ApiGateways/Shopping.Aggregator/"]
COPY ["BuildingBlocks/Common.Logging/Common.Logging.csproj", "BuildingBlocks/Common.Logging/"]
RUN dotnet restore "ApiGateways/Shopping.Aggregator/Shopping.Aggregator.csproj"
COPY . .
WORKDIR "/src/ApiGateways/Shopping.Aggregator"
RUN dotnet build "Shopping.Aggregator.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Shopping.Aggregator.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Shopping.Aggregator.dll"]
