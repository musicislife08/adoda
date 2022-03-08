FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["AgentWorker/*.csproj", "AgentWorker/"]
RUN dotnet restore "AgentWorker/AgentWorker.csproj"
COPY . .
WORKDIR "/src/AgentWorker"
RUN dotnet build "AgentWorker.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "AgentWorker.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AgentWorker.dll"]
