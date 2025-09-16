# ────────────────────── STAGE 1 – BUILD & TEST ──────────────────────
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copiar solução e configuração do NuGet
COPY Nuget.config .
COPY Clipo.sln .

# Copiar código-fonte
COPY . .

# Restaurar dependências
RUN dotnet restore Clipo.sln --configfile Nuget.config

# Compilar e testar (caso existam testes)
RUN dotnet build Clipo.sln -c Release --no-restore

# Publicar somente a API
RUN dotnet publish Clipo/Clipo.Api/Clipo.Api.csproj \
    -c Release -o /app/publish --no-build -p:UseAppHost=false

# ────────────────────── STAGE 2 – RUNTIME ───────────────────────────
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app

# Configurações essenciais para rodar no Kubernetes
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production
ENV DOTNET_RUNNING_IN_CONTAINER=true
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false

# Copiar binários publicados
COPY --from=build /app/publish .

# Expor a porta que será usada pelo Service no cluster
EXPOSE 8080

# (Opcional) Rodar como usuário não-root (recomendado para AWS Fargate/EKS)
RUN adduser --disabled-password --gecos '' appuser && chown -R appuser /app
USER appuser

# Comando de inicialização
ENTRYPOINT ["dotnet", "Clipo.Api.dll"]
