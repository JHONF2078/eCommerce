# Etapa base
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 5001

# Etapa de build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src


################# INICIA ETAPA DE BUILD O DESARROLLO   ###################

# En esta etapa se ejecutan desde src:
# dotnet restore
# dotnet build
# dotnet publish -o /app/publish

# Copiar solo los archivos del proyecto (mucho mas ordenado)
COPY UsersService.API/UsersService.API.csproj ./UsersService.API/
COPY UsersService.Application/UsersService.Application.csproj ./UsersService.Application/
COPY UsersService.Domain/UsersService.Domain.csproj ./UsersService.Domain/
COPY UsersService.Infrastructure/UsersService.Infrastructure.csproj ./UsersService.Infrastructure/

# Restaurar dependencias
RUN dotnet restore ./UsersService.API/UsersService.API.csproj

# Copiar el resto del codigo fuente
COPY UsersService.API/ ./UsersService.API/
COPY UsersService.Application/ ./UsersService.Application/
COPY UsersService.Domain/ ./UsersService.Domain/
COPY UsersService.Infrastructure/ ./UsersService.Infrastructure/


# Aqui es donde copias los archivos y compilas

#/src
#├── UsersService.API/
#│   ├── UsersService.API.csproj
#│   ├── Controllers/
#│   ├── Program.cs
#│   └── appsettings.json
#├── UsersService.Application/
#│   └── UsersService.Application.csproj
#├── UsersService.Domain/
#│   └── UsersService.Domain.csproj
#├── UsersService.Infrastructure/
#│   └── UsersService.Infrastructure.csproj
#


# Build
WORKDIR /src/UsersService.API
RUN dotnet build -c Release -o /app/build

# Publicar
FROM build AS publish
RUN dotnet publish -c Release -o /app/publish

#Esto es lo que se genera como salida despues del dotnet publish. Es lo que va a la imagen final:

#/app/publish
#├── UsersService.API.dll
#├── UsersService.Application.dll
#├── UsersService.Domain.dll
#├── UsersService.Infrastructure.dll
#├── appsettings.json
#├── web.config (si aplica)
#└── ...otros archivos necesarios para ejecución



################# FINALIZAR DE BUILD O DESARROLLO   ###################



# Imagen final
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "UsersService.API.dll"]


# El contenido se ve asi en el contenedor final

#
#/app
#├── UsersService.API.dll
#├── UsersService.Application.dll
#├── UsersService.Domain.dll
#├── UsersService.Infrastructure.dll
#├── appsettings.json
#└── ...


#Etapa:       Carpeta:       Contenido:
#------------ -------------  ---------------------------------------------
#build        /src           Código fuente (proyectos .csproj, .cs, etc.)
#publish      /app/publish   Archivos compilados listos para ejecución
#final        /app           Copia de /app/publish → ejecutable