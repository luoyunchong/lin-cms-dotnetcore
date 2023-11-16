#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Directory.Build.props", "."]
COPY ["identityserver4/LinCms.IdentityServer4/LinCms.IdentityServer4.csproj", "identityserver4/LinCms.IdentityServer4/"]
COPY ["src/LinCms.Application/LinCms.Application.csproj", "src/LinCms.Application/"]
COPY ["src/LinCms.Application.Contracts/LinCms.Application.Contracts.csproj", "src/LinCms.Application.Contracts/"]
COPY ["src/LinCms.Core/LinCms.Core.csproj", "src/LinCms.Core/"]
COPY ["src/LinCms.Infrastructure/LinCms.Infrastructure.csproj", "src/LinCms.Infrastructure/"]
RUN dotnet restore "./identityserver4/LinCms.IdentityServer4/./LinCms.IdentityServer4.csproj"
COPY . .
WORKDIR "/src/identityserver4/LinCms.IdentityServer4"
RUN dotnet build "./LinCms.IdentityServer4.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./LinCms.IdentityServer4.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "LinCms.IdentityServer4.dll"]