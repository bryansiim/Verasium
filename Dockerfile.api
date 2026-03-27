FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY src/Verasium.Core/Verasium.Core.csproj Verasium.Core/
COPY src/Verasium.Api/Verasium.Api.csproj Verasium.Api/
RUN dotnet restore Verasium.Api/Verasium.Api.csproj

COPY src/Verasium.Core/ Verasium.Core/
COPY src/Verasium.Api/ Verasium.Api/
RUN dotnet publish Verasium.Api/Verasium.Api.csproj -c Release -o /app --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app
COPY --from=build /app .
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
ENTRYPOINT ["dotnet", "Verasium.Api.dll"]
