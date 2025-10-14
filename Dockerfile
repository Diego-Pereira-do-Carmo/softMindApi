FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /src

COPY SoftMindApi/SoftMindApi.csproj SoftMindApi/
COPY SoftMindApi.Tests/SoftMindApi.Tests.csproj SoftMindApi.Tests/
RUN dotnet restore SoftMindApi/SoftMindApi.csproj

COPY . .

RUN dotnet publish SoftMindApi/SoftMindApi.csproj -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final

RUN apt-get update \
    && apt-get install -y --no-install-recommends ca-certificates tzdata curl \
    && update-ca-certificates \
    && rm -rf /var/lib/apt/lists/*

ENV TZ=America/Sao_Paulo

WORKDIR /app

COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:8080

EXPOSE 8080

ENTRYPOINT ["dotnet", "SoftMindApi.dll"]
