FROM mcr.microsoft.com/dotnet/core/sdk:3.0-buster AS build
WORKDIR /src
COPY . .
WORKDIR "/src/src/Services/IndexingService/Diploma.IndexingService.Api"
RUN dotnet restore "Diploma.IndexingService.Api.csproj"
RUN dotnet publish "Diploma.IndexingService.Api.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/core/aspnet:3.0-buster-slim AS final
WORKDIR /app
EXPOSE 80
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Diploma.IndexingService.Api.dll"]
