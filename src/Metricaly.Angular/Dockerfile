#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
RUN curl -sL https://deb.nodesource.com/setup_12.x | bash -
RUN apt install -y nodejs
WORKDIR /src
COPY ["src/Metricaly.Angular/Metricaly.Angular.csproj", "src/Metricaly.Angular/"]
COPY ["src/Metricaly.Infrastructure/Metricaly.Infrastructure.csproj", "src/Metricaly.Infrastructure/"]
COPY ["src/Metricaly.Core/Metricaly.Core.csproj", "src/Metricaly.Core/"]
RUN dotnet restore "src/Metricaly.Angular/Metricaly.Angular.csproj"
COPY . .
WORKDIR "/src/src/Metricaly.Angular"
RUN dotnet build "Metricaly.Angular.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Metricaly.Angular.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Metricaly.Angular.dll"]