#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0.0-preview.2-bookworm-slim-amd64 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0.100-preview.2-bookworm-slim-amd64 AS build
WORKDIR /src
COPY ["../../global.json", "GordonBeemingCom/global.json"]
COPY ["GordonBeemingCom/GordonBeemingCom.csproj", "GordonBeemingCom/"]
RUN dotnet restore "GordonBeemingCom/GordonBeemingCom.csproj"
COPY . .
WORKDIR "/src/GordonBeemingCom"
RUN dotnet build "GordonBeemingCom.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "GordonBeemingCom.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENV BRANCH_NAME=git branch --show-current
ENV COMMIT_HASH=git rev-parse $(git branch --show-current)
ENTRYPOINT ["dotnet", "GordonBeemingCom.dll"]