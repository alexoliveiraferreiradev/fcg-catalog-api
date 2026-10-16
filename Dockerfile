FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER app
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src


RUN dotnet tool install --global dotnet-ef --version 9.0.15
ENV PATH="$PATH:/root/.dotnet/tools"

COPY ["src/Fcg.Catalog.API/Fcg.Catalog.API.csproj", "Fcg.Catalog.API/"]
COPY ["src/Fcg.Catalog.Application/Fcg.Catalog.Application.csproj", "Fcg.Catalog.Application/"]
COPY ["src/Fcg.Catalog.Domain/Fcg.Catalog.Domain.csproj", "Fcg.Catalog.Domain/"]
COPY ["src/Fcg.Catalog.Infrastructure/Fcg.Catalog.Infrastructure.csproj", "Fcg.Catalog.Infrastructure/"]

RUN dotnet restore "./Fcg.Catalog.API/Fcg.Catalog.API.csproj"

COPY src/ .
WORKDIR "/src/Fcg.Catalog.API"

RUN dotnet publish "./Fcg.Catalog.API.csproj" -c $BUILD_CONFIGURATION --no-restore -o /app/publish /p:UseAppHost=false
RUN dotnet ef migrations bundle --no-build --configuration $BUILD_CONFIGURATION --self-contained -r linux-x64 -o /app/efbundle

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
COPY --from=build /app/efbundle ./efbundle

ENTRYPOINT []
CMD ["dotnet", "Fcg.Catalog.API.dll"]
