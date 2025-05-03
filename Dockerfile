FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build

# Install Node.js
RUN curl -fsSL https://deb.nodesource.com/setup_22.x | bash - \
    && apt-get install -y \
        nodejs \
    && rm -rf /var/lib/apt/lists/*

WORKDIR /src
COPY ["HvZBot.csproj", "HvZBot/"]
RUN dotnet restore "HvZBot/HvZBot.csproj"
WORKDIR "/src/HvZBot"
COPY . .
RUN dotnet build "HvZBot.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "HvZBot.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
ARG GID=1000
ARG UID=1000
RUN groupadd --gid $GID appgroup && \
    useradd --uid $UID --gid $GID --create-home --shell /bin/bash appuser && \
    chown -R appuser /app
USER appuser
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "HvZBot.dll"]