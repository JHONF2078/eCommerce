# Etapa base
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8081

# Etapa de build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src


################# INICIA ETAPA DE BUILD O DESARROLLO   ###################

# En esta etapa se ejecutan desde src:
# dotnet restore
# dotnet build
# dotnet publish -o /app/publish

# Copiar solo los archivos del proyecto (mucho mas ordenado)
COPY OrdersService.API/OrdersService.API.csproj ./OrdersService.API/
COPY DataAccessLayer/DataAccessLayer.csproj ./DataAccessLayer/
COPY BusinessLogicLayer/BusinessLogicLayer.csproj ./BusinessLogicLayer.Domain/

# Restaurar dependencias
RUN dotnet restore ./OrdersService.API/OrdersService.API.csproj

# Copiar el resto del codigo fuente
COPY OrdersService.API/ ./OrdersService.API/
COPY DataAccessLayer/ ./DataAccessLayer/
COPY BusinessLogicLayer/ ./BusinessLogicLayer/

# Aqui es donde copias los archivos y compilas

#/src
#├── OrdersService.API/
#│   ├── OrdersService.API.csproj
#│   ├── Controllers/
#│   ├── Program.cs
#│   └── appsettings.json
#├── OrdersService.Application/
#│   └── OrdersService.Application.csproj
#├── OrdersService.Domain/
#│   └── OrdersService.Domain.csproj
#├── OrdersService.Infrastructure/
#│   └── OrdersService.Infrastructure.csproj
#


# Build
WORKDIR /src/OrdersService.API
RUN dotnet build -c Release -o /app/build

# Publicar
FROM build AS publish
RUN dotnet publish -c Release -o /app/publish

#Esto es lo que se genera como salida despues del dotnet publish. Es lo que va a la imagen final:

#/app/publish
#├── OrdersService.API.dll
#├── OrdersService.Application.dll
#├── OrdersService.Domain.dll
#├── OrdersService.Infrastructure.dll
#├── appsettings.json
#├── web.config (si aplica)
#└── ...otros archivos necesarios para ejecución



################# FINALIZAR DE BUILD O DESARROLLO   ###################



# Imagen final
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "OrdersService.API.dll"]


# El contenido se ve asi en el contenedor final

#
#/app
#├── OrdersService.API.dll
#├── OrdersService.Application.dll
#├── OrdersService.Domain.dll
#├── OrdersService.Infrastructure.dll
#├── appsettings.json
#└── ...


#Etapa:       Carpeta:       Contenido:
#------------ -------------  ---------------------------------------------
#build        /src           Código fuente (proyectos .csproj, .cs, etc.)
#publish      /app/publish   Archivos compilados listos para ejecución
#final        /app           Copia de /app/publish → ejecutable