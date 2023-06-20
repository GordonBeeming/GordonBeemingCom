#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0-preview-bookworm-slim-amd64 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0-preview-bookworm-slim-amd64 AS build
WORKDIR /src
COPY ["global.json", "GordonBeemingCom.Editor/global.json"]
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