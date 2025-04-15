# Etapa base
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 5001


# Etapa de build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src/UsersService

#COPY ["UsersService.API/UsersService.API.csproj", "UsersService.API/"]
#COPY ["UsersService.Application/UsersService.Application.csproj", "UsersService.Application/"]
#COPY ["UsersService.Domain/UsersService.Domain.csproj", "UsersService.Domain/"]
#COPY ["UsersService.Infrastructure/UsersService.Infrastructure.csproj", "UsersService.Infrastructure/"]


COPY . .

WORKDIR /src/UsersService/UsersService.API
RUN dotnet restore

# Publicar
FROM build AS publish
RUN dotnet publish -c Release -o /app/publish

# Imagen final
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "UsersService.API.dll"]
