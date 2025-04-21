# Etapa base
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
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
COPY ProductsService.API/ProductsService.API.csproj ./ProductsService.API/
COPY DataAccessLayer/DataAccessLayer.csproj ./DataAccessLayer/
COPY BusinessLogicLayer/BusinessLogicLayer.csproj ./BusinessLogicLayer.Domain/

# Restaurar dependencias
RUN dotnet restore ./ProductsService.API/ProductsService.API.csproj

# Copiar el resto del codigo fuente
COPY ProductsService.API/ ./ProductsService.API/
COPY DataAccessLayer/ ./DataAccessLayer/
COPY BusinessLogicLayer/ ./BusinessLogicLayer/

# Aqui es donde copias los archivos y compilas

#/src
#├── ProductsService.API/
#│   ├── ProductsService.API.csproj
#│   ├── Controllers/
#│   ├── Program.cs
#│   └── appsettings.json
#├── ProductsService.Application/
#│   └── ProductsService.Application.csproj
#├── ProductsService.Domain/
#│   └── ProductsService.Domain.csproj
#├── ProductsService.Infrastructure/
#│   └── ProductsService.Infrastructure.csproj
#


# Build
WORKDIR /src/ProductsService.API
RUN dotnet build -c Release -o /app/build

# Publicar
FROM build AS publish
RUN dotnet publish -c Release -o /app/publish

#Esto es lo que se genera como salida despues del dotnet publish. Es lo que va a la imagen final:

#/app/publish
#├── ProductsService.API.dll
#├── ProductsService.Application.dll
#├── ProductsService.Domain.dll
#├── ProductsService.Infrastructure.dll
#├── appsettings.json
#├── web.config (si aplica)
#└── ...otros archivos necesarios para ejecución



################# FINALIZAR DE BUILD O DESARROLLO   ###################



# Imagen final
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ProductsService.API.dll"]


# El contenido se ve asi en el contenedor final

#
#/app
#├── ProductsService.API.dll
#├── ProductsService.Application.dll
#├── ProductsService.Domain.dll
#├── ProductsService.Infrastructure.dll
#├── appsettings.json
#└── ...


#Etapa:       Carpeta:       Contenido:
#------------ -------------  ---------------------------------------------
#build        /src           Código fuente (proyectos .csproj, .cs, etc.)
#publish      /app/publish   Archivos compilados listos para ejecución
#final        /app           Copia de /app/publish → ejecutable