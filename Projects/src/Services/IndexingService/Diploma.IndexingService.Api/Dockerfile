FROM mcr.microsoft.com/dotnet/core/sdk:3.0-buster AS build
WORKDIR /src
COPY /src/Services/IndexingService/Diploma.IndexingService.Api/Diploma.IndexingService.Api.csproj ./src/Services/IndexingService/Diploma.IndexingService.Api
RUN dotnet restore "./src/Services/IndexingService/Diploma.IndexingService.Api/Diploma.IndexingService.Api.csproj"
COPY . .
WORKDIR "/src/src/Services/IndexingService/Diploma.IndexingService.Api"
RUN dotnet publish "Diploma.IndexingService.Api.csproj" -c Release -o /app/publish --no-restore

FROM node:13.5.0-alpine as client_build
WORKDIR /ClientApp
COPY /src/Services/IndexingService/Diploma.IndexingService.Api/ClientApp/package.json .
RUN npm install
COPY /src/Services/IndexingService/Diploma.IndexingService.Api/ClientApp/ .
RUN npm run release

FROM mcr.microsoft.com/dotnet/core/aspnet:3.0-buster-slim AS final
WORKDIR /app
EXPOSE 80
COPY --from=build /app/publish .
COPY --from=client_build /ClientApp/wwwroot .
ENTRYPOINT ["dotnet", "Diploma.IndexingService.Api.dll"]