

dotnet ef migrations add AddedDashboardTables2 --context applicationdbcontext -p ../Metricaly.Infrastructure/Metricaly.Infrastructure.csproj -s Metricaly.Angular.csproj -o Data/Migrations


dotnet ef database update -c applicationdbcontext -p ../Metricaly.Infrastructure/Metricaly.Infrastructure.csproj -s Metricaly.Angular.csproj




## TODO:



