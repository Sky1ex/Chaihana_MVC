FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

RUN dotnet tool install --global dotnet-ef

COPY ["WebApplication1.csproj", "."]
RUN dotnet restore "WebApplication1.csproj"
COPY . .
RUN dotnet publish "WebApplication1.csproj" -c Release -o /app

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app .

# Решение для DataProtection
RUN mkdir -p /app/keys
ENV ASPNETCORE_URLS=http://+:5000
ENV ASPNETCORE_DataProtection__Keys__Path=/app/keys

ENTRYPOINT ["dotnet", "WebApplication1.dll"]