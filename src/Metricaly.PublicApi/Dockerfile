#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["src/Metricaly.PublicApi/Metricaly.PublicApi.csproj", "src/Metricaly.PublicApi/"]
COPY ["src/Metricaly.Infrastructure/Metricaly.Infrastructure.csproj", "src/Metricaly.Infrastructure/"]
COPY ["src/Metricaly.Core/Metricaly.Core.csproj", "src/Metricaly.Core/"]
RUN dotnet restore "src/Metricaly.PublicApi/Metricaly.PublicApi.csproj"
COPY . .
WORKDIR "/src/src/Metricaly.PublicApi"
RUN dotnet build "Metricaly.PublicApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Metricaly.PublicApi.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Metricaly.PublicApi.dll"]