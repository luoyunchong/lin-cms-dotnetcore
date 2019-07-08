FROM mcr.microsoft.com/dotnet/core/aspnet:2.2-stretch-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:2.2-stretch AS build
WORKDIR /src
COPY ["LinCms.Web/LinCms.Web.csproj", "LinCms.Web/"]
RUN dotnet restore "LinCms.Web/LinCms.Web.csproj"
COPY . .
WORKDIR "/src/LinCms.Web"
RUN dotnet build "LinCms.Web.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "LinCms.Web.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "LinCms.Web.dll"]