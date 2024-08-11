#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /workspace
COPY Directory.Build.props .
COPY ["src/GoDaddyDDNS/GoDaddyDDNS.csproj", "src/GoDaddyDDNS/"]
RUN dotnet restore "src/GoDaddyDDNS/GoDaddyDDNS.csproj"
COPY . .
WORKDIR "/workspace/src/GoDaddyDDNS"
RUN dotnet build "GoDaddyDDNS.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "GoDaddyDDNS.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

ENV AUTHOR="Tudor Stanciu"
ENV TZ=Europe/Bucharest

ARG APP_VERSION=0.0.0.0
ENV APP_VERSION=${APP_VERSION}

ARG LOGGING_LEVEL="Information"
ENV Logging__LogLevel__Default=${LOGGING_LEVEL}

ARG DOMAIN=""
ENV Domain=${DOMAIN}

ARG KEY=""
ENV Key=${KEY}

ARG SECRET=""
ENV Secret=${SECRET}

ARG EXECUTION_FREQUENCY
ENV ExecutionFrequency=${EXECUTION_FREQUENCY}

ENTRYPOINT ["dotnet", "GoDaddyDDNS.dll"]