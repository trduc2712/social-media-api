# syntax=docker/dockerfile:1

FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:10.0.401-noble@sha256:e70cdb7f80b0348f5cb85f19a8f670fca061f033d57eed12fa003d58b0e06317 AS build
ARG TARGETARCH
ENV DOTNET_NOLOGO=true \
    DOTNET_CLI_TELEMETRY_OPTOUT=true
WORKDIR /src

COPY Directory.Build.props Directory.Packages.props ./
COPY SocialMedia.Domain/SocialMedia.Domain.csproj SocialMedia.Domain/
COPY SocialMedia.Contract/SocialMedia.Contract.csproj SocialMedia.Contract/
COPY SocialMedia.Application/SocialMedia.Application.csproj SocialMedia.Application/
COPY SocialMedia.Infrastructure/SocialMedia.Infrastructure.csproj SocialMedia.Infrastructure/
COPY SocialMedia.Api/SocialMedia.Api.csproj SocialMedia.Api/
RUN --mount=type=cache,target=/root/.nuget/packages \
    dotnet restore SocialMedia.Api/SocialMedia.Api.csproj -a "$TARGETARCH"

COPY . .
RUN --mount=type=cache,target=/root/.nuget/packages \
    dotnet publish SocialMedia.Api/SocialMedia.Api.csproj -c Release -a "$TARGETARCH" --no-restore -o /app/publish -p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0.12-noble-chiseled-extra@sha256:00e0ad6a7ef8c0c1391b87f05c7ac757a15740455688f2bfcd146a3f4b987efd
WORKDIR /app
COPY --from=build /app/publish ./
USER $APP_UID
EXPOSE 8080
ENTRYPOINT ["dotnet", "SocialMedia.Api.dll"]
