#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:9.0-preview-alpine AS base
WORKDIR /app
EXPOSE 80
ENV ASPNETCORE_URLS=http://+:80
ARG A_COMMIT_HASH
ENV COMMIT_HASH=$A_COMMIT_HASH
ARG A_BRANCH_NAME
ENV BRANCH_NAME=$A_BRANCH_NAME
RUN apk add icu-libs
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["global.json", "GordonBeemingCom.Editor/global.json"]
COPY ["Directory.*.props", "./"]
COPY ["src/GordonBeemingCom.Shared/GordonBeemingCom.Shared.csproj", "GordonBeemingCom.Shared/"]
COPY ["src/GordonBeemingCom.Database/GordonBeemingCom.Database.csproj", "GordonBeemingCom.Database/"]
COPY ["src/GordonBeemingCom.Editor/GordonBeemingCom.Editor.csproj", "GordonBeemingCom.Editor/"]
RUN dotnet restore "GordonBeemingCom.Editor/GordonBeemingCom.Editor.csproj"
COPY src/. .
WORKDIR "/src/GordonBeemingCom.Editor"
RUN dotnet build "GordonBeemingCom.Editor.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "GordonBeemingCom.Editor.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "GordonBeemingCom.Editor.dll"]