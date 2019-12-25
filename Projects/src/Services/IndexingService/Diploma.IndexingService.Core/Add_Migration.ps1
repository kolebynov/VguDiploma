$migrationName = Read-Host "Migration name"
dotnet ef migrations add $migrationName --startup-project ..\Diploma.IndexingService.Api\Diploma.IndexingService.Api.csproj
Pause