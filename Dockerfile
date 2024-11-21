
# Use a imagem oficial do .NET SDK 8.0 para compilar o projeto
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

# Defina o diretório de trabalho no container
WORKDIR /app

# Copie o arquivo do projeto .csproj para o container
COPY *.csproj ./

# Limpe caches do NuGet e restaure dependências
RUN dotnet nuget locals all --clear
RUN dotnet restore ./backend.csproj

# Copie o restante dos arquivos do backend para o container
COPY . ./

# Compile e publique o aplicativo
RUN dotnet publish ./backend.csproj -c Release -o /publish

# Use uma imagem mais leve para executar o aplicativo
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime

# Defina o diretório de trabalho do runtime
WORKDIR /app

# Copie os arquivos compilados para o runtime
COPY --from=build /publish .

# Expor a porta padrão do ASP.NET Core
EXPOSE 80

# Comando de entrada para rodar o aplicativo
ENTRYPOINT ["dotnet", "backend.dll"]
