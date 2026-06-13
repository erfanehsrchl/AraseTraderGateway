FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Build context should include both AraseTraderGateway and the referenced Contracts project.
COPY ["AraseTraderGateway/AraseTraderGateway.sln", "AraseTraderGateway/"]
COPY ["AraseTraderGateway/Api/Api.csproj", "AraseTraderGateway/Api/"]
COPY ["AraseTraderGateway/Application/Application.csproj", "AraseTraderGateway/Application/"]
COPY ["AraseTraderGateway/Domain/Domain.csproj", "AraseTraderGateway/Domain/"]
COPY ["AraseTraderGateway/Infrastructure/Infrastructure.csproj", "AraseTraderGateway/Infrastructure/"]
COPY ["AraseTraderOrderService/Contracts/Contracts.csproj", "AraseTraderOrderService/Contracts/"]
COPY ["AraseTraderGateway/NuGet.Config", "AraseTraderGateway/"]

WORKDIR /src/AraseTraderGateway
RUN dotnet restore "AraseTraderGateway.sln"

WORKDIR /src
COPY ["AraseTraderGateway/", "AraseTraderGateway/"]
COPY ["AraseTraderOrderService/Contracts/", "AraseTraderOrderService/Contracts/"]

WORKDIR /src/AraseTraderGateway
RUN dotnet build "Api/Api.csproj" -c $BUILD_CONFIGURATION --no-restore
RUN dotnet publish "Api/Api.csproj" -c $BUILD_CONFIGURATION --no-build -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Api.dll"]
