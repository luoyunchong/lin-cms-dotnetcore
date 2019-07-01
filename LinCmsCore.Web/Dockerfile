FROM mcr.microsoft.com/dotnet/core/aspnet:2.2-stretch-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:2.2-stretch AS build
WORKDIR /src
COPY ["LinCmsCore.Web/LinCmsCore.Web.csproj", "LinCmsCore.Web/"]
RUN dotnet restore "LinCmsCore.Web/LinCmsCore.Web.csproj"
COPY . .
WORKDIR "/src/LinCmsCore.Web"
RUN dotnet build "LinCmsCore.Web.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "LinCmsCore.Web.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "LinCmsCore.Web.dll"]