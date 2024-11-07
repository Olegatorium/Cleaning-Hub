# 1. Базовый образ с ASP.NET Runtime для работы приложения
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

# 2. Образ с SDK для сборки приложения
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Копируем и восстанавливаем зависимости
COPY ["BookingService.API.csproj", "./"]
RUN dotnet restore "BookingService.API.csproj"

# Копируем исходный код и собираем проект
COPY . .
RUN dotnet build "BookingService.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

# 3. Публикуем приложение
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "BookingService.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# 4. Финальный этап: запускаем приложение на базе ASP.NET Runtime
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Устанавливаем точку входа для запуска приложения
ENTRYPOINT ["dotnet", "BookingService.API.dll"]
