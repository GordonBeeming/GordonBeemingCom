#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:9.0-preview-alpine AS base
WORKDIR /app
EXPOSE 80
ENV ASPNETCORE_URLS=http://+:80
ARG A_COMMIT_HASH
ENV COMMIT_HASH=$A_COMMIT_HASH
ARG A_BRANCH_NAME
ENV BRANCH_NAME=$A_BRANCH_NAME

FROM mcr.microsoft.com/dotnet/sdk:9.0-preview-alpine AS build
WORKDIR /src
COPY ["global.json", "GordonBeemingCom/global.json"]
COPY ["src/GordonBeemingCom.Shared/GordonBeemingCom.Shared.csproj", "GordonBeemingCom.Shared/"]
COPY ["src/GordonBeemingCom.Database/GordonBeemingCom.Database.csproj", "GordonBeemingCom.Database/"]
COPY ["src/GordonBeemingCom/GordonBeemingCom.csproj", "GordonBeemingCom/"]
RUN dotnet restore "GordonBeemingCom/GordonBeemingCom.csproj"
COPY src/. .
WORKDIR "/src/GordonBeemingCom"
RUN dotnet build "GordonBeemingCom.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "GordonBeemingCom.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "GordonBeemingCom.dll"]