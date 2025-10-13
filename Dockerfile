FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /src

COPY SoftMindApi/SoftMindApi.csproj SoftMindApi/
COPY SoftMindApi.Tests/SoftMindApi.Tests.csproj SoftMindApi.Tests/
RUN dotnet restore SoftMindApi/SoftMindApi.csproj

COPY . .

RUN dotnet publish SoftMindApi/SoftMindApi.csproj -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final

WORKDIR /app

COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:8080

EXPOSE 8080

ENTRYPOINT ["dotnet", "SoftMindApi.dll"]
