FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src


COPY ["app-dental-api/nuget.config", "./"]

# Copiar los archivos de proyecto y restaurar dependencias de cada uno
COPY ["app-dental-api/app-dental-api.csproj", "app-dental-api/"]
COPY ["Modelo/Modelo.csproj", "Modelo/"]
COPY ["Negocio/Negocio.csproj", "Negocio/"]
COPY ["Datos/Datos.csproj", "Datos/"]
COPY ["Utilidades/Utilidades.csproj", "Utilidades/"]

# Restaurar paquetes NuGet para cada proyecto
RUN dotnet restore "app-dental-api/app-dental-api.csproj"
RUN dotnet restore "Modelo/Modelo.csproj"
RUN dotnet restore "Negocio/Negocio.csproj"
RUN dotnet restore "Datos/Datos.csproj"
RUN dotnet restore "Utilidades/Utilidades.csproj"

# Copiar el resto de los archivos del proyecto
COPY . .

RUN dotnet publish "app-dental-api/app-dental-api.csproj" -c Release -o /app/publish

# Imagen final para runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0

RUN apt-get update && apt-get install -y openssl

WORKDIR /app
COPY --from=build /app/publish .

COPY https/ /https/ 
RUN chmod 644 /https/aspnetapp.pfx
# Exponer los puertos después de la construcción
EXPOSE 80
EXPOSE 443

# Comando de entrada para ejecutar la aplicación en modo watch
ENTRYPOINT ["dotnet", "app-dental-api.dll"]
