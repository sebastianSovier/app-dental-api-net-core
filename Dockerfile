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

# Exponer los puertos después de la construcción
EXPOSE 80
EXPOSE 443

# Comando de entrada para ejecutar la aplicación en modo watch
CMD ["dotnet", "watch", "run", "--project", "app-dental-api/app-dental-api.csproj", "--urls", "https://*:443;http://*:80"]
