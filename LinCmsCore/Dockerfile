FROM mcr.microsoft.com/dotnet/core/aspnet:2.2-stretch-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:2.2-stretch AS build
WORKDIR /src
COPY ["LinCmsCore/LinCmsCore.csproj", "LinCmsCore/"]
RUN dotnet restore "LinCmsCore/LinCmsCore.csproj"
COPY . .
WORKDIR "/src/LinCmsCore"
RUN dotnet build "LinCmsCore.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "LinCmsCore.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "LinCmsCore.dll"]