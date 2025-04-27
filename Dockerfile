# Используем официальный образ ASP.NET Core
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src

# Копируем файлы проекта и восстанавливаем зависимости
COPY ["WebApplication1.csproj", "."]
RUN dotnet restore "WebApplication1.csproj"

# Копируем все файлы и собираем проект
COPY . .
RUN dotnet publish -c Release -o /app

# Финальный образ
FROM mcr.microsoft.com/dotnet/aspnet:7.0
WORKDIR /app
COPY --from=build /app .

# Устанавливаем переменные среды для Npgsql
ENV ASPNETCORE_URLS=http://+:5000
ENV ConnectionStrings__DefaultConnection="Host=localhost;Database=WebDB;Username=postgres;Password=admin"

ENTRYPOINT ["dotnet", "WebApplication1.dll"]