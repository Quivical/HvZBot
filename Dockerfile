# Start your image with a node base image
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build-env

# The /app directory should act as the main application directory
WORKDIR /app

COPY *.csproj ./
RUN dotnet restore

COPY . ./
RUN dotnet publish -c Release -o out

FROM mcr.microsoft.com/dotnet/aspnet:7.0
WORKDIR /app
COPY --from=build-env /app/out .
COPY *.env ./

ENTRYPOINT ["dotnet", "HvZBot.dll"]