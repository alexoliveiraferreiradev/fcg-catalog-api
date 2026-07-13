FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER app
WORKDIR /app
EXPOSE 8080


FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release


WORKDIR /src



COPY ["src/Fcg.Catalog.API/Fcg.Catalog.API.csproj", "Fcg.Catalog.API/"]
COPY ["src/Fcg.Catalog.Application/Fcg.Catalog.Application.csproj", "Fcg.Catalog.Application/"]
COPY ["src/Fcg.Catalog.Domain/Fcg.Catalog.Domain.csproj", "Fcg.Catalog.Domain/"]
COPY ["src/Fcg.Catalog.Infrastructure/Fcg.Catalog.Infrastructure.csproj", "Fcg.Catalog.Infrastructure/"]

RUN dotnet restore "./Fcg.Catalog.API/Fcg.Catalog.API.csproj"


COPY src/ .
WORKDIR "/src/Fcg.Catalog.API"
RUN dotnet build "./Fcg.Catalog.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

RUN dotnet tool install --global dotnet-ef
ENV PATH="$PATH:/root/.dotnet/tools"

RUN dotnet ef migrations bundle \
    --self-contained \
    -r linux-x64 \
    --project ../Fcg.Catalog.Infrastructure/Fcg.Catalog.Infrastructure.csproj \
    --startup-project ./Fcg.Catalog.API.csproj \
    -o /app/efbundle

FROM build AS publish
RUN dotnet publish "./Fcg.Catalog.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false


FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

USER root
COPY --from=build /app/efbundle ./efbundle
RUN chmod +x ./efbundle
USER app

ENTRYPOINT ["dotnet", "Fcg.Catalog.API.dll"]