FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["ReportBroker.Api/ReportBroker.Api.csproj", "ReportBroker.Api/"]
COPY ["ReportBroker.Application/ReportBroker.Application.csproj", "ReportBroker.Application/"]
COPY ["ReportBroker.Domain/ReportBroker.Domain.csproj", "ReportBroker.Domain/"]
COPY ["ReportBroker.Infrastructure/ReportBroker.Infrastructure.csproj", "ReportBroker.Infrastructure/"]

RUN dotnet restore "ReportBroker.Api/ReportBroker.Api.csproj"

COPY . .

RUN dotnet build "ReportBroker.Api/ReportBroker.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ReportBroker.Api/ReportBroker.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app

RUN apt-get update && apt-get install -y \
    libgssapi-krb5-2 \
    && rm -rf /var/lib/apt/lists/*

COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ReportBroker.Api.dll"]