#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
RUN apt-get update -y && apt-get install -y libfontconfig1
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Directory.Build.props", "."]
COPY ["src/LinCms.Web/LinCms.Web.csproj", "src/LinCms.Web/"]
COPY ["src/LinCms.Application.Contracts/LinCms.Application.Contracts.csproj", "src/LinCms.Application.Contracts/"]
COPY ["src/LinCms.Core/LinCms.Core.csproj", "src/LinCms.Core/"]
COPY ["src/LinCms.Application/LinCms.Application.csproj", "src/LinCms.Application/"]
COPY ["src/LinCms.Infrastructure/LinCms.Infrastructure.csproj", "src/LinCms.Infrastructure/"]
COPY ["src/LinCms.Plugins/LinCms.Plugins.csproj", "src/LinCms.Plugins/"]
RUN dotnet restore "./src/LinCms.Web/./LinCms.Web.csproj"
COPY . .
WORKDIR "/src/src/LinCms.Web"
RUN dotnet build "./LinCms.Web.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./LinCms.Web.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "LinCms.Web.dll"]