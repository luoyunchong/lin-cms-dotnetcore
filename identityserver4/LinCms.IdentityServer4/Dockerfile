#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["identityserver4/LinCms.IdentityServer4/LinCms.IdentityServer4.csproj", "identityserver4/LinCms.IdentityServer4/"]
COPY ["src/LinCms.Application/LinCms.Application.csproj", "src/LinCms.Application/"]
COPY ["src/LinCms.Infrastructure/LinCms.Infrastructure.csproj", "src/LinCms.Infrastructure/"]
RUN dotnet restore "identityserver4/LinCms.IdentityServer4/LinCms.IdentityServer4.csproj"
COPY . .
WORKDIR "/src/identityserver4/LinCms.IdentityServer4"
RUN dotnet build "LinCms.IdentityServer4.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "LinCms.IdentityServer4.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "LinCms.IdentityServer4.dll"]