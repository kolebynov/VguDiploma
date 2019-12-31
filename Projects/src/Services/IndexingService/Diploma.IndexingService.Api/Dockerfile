FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY src/Infrastructure/Diploma.Api.Shared/Diploma.Api.Shared.csproj src/Infrastructure/Diploma.Api.Shared/
COPY src/Infrastructure/Diploma.Shared/Diploma.Shared.csproj src/Infrastructure/Diploma.Shared/
COPY src/Services/IndexingService/Diploma.IndexingService.Core/Diploma.IndexingService.Core.csproj src/Services/IndexingService/Diploma.IndexingService.Core/
COPY src/Services/IndexingService/Diploma.IndexingService.EsDocumentStorage/Diploma.IndexingService.EsDocumentStorage.csproj src/Services/IndexingService/Diploma.IndexingService.EsDocumentStorage/
COPY src/Services/IndexingService/Diploma.IndexingService.Api/Diploma.IndexingService.Api.csproj src/Services/IndexingService/Diploma.IndexingService.Api/
RUN dotnet restore "src/Services/IndexingService/Diploma.IndexingService.Api/Diploma.IndexingService.Api.csproj"
COPY . .
WORKDIR "/src/src/Services/IndexingService/Diploma.IndexingService.Api"
RUN dotnet publish "Diploma.IndexingService.Api.csproj" -c Release -o /app/publish --no-restore

FROM node:13.5.0-alpine as client_build
WORKDIR /ClientApp
COPY /src/Services/IndexingService/Diploma.IndexingService.Api/ClientApp/package.json /src/Services/IndexingService/Diploma.IndexingService.Api/ClientApp/package-lock.json ./
RUN npm install
COPY /src/Services/IndexingService/Diploma.IndexingService.Api/ClientApp/ .
RUN npm run release

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS final
WORKDIR /app
EXPOSE 80
COPY --from=build /app/publish .
COPY --from=client_build /ClientApp/wwwroot ./wwwroot/
COPY wait-for-it.sh ./wait-for-it.sh
RUN ["chmod", "+x", "./wait-for-it.sh"]
ENTRYPOINT ["dotnet", "Diploma.IndexingService.Api.dll"]