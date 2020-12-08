# Welcome to Metricaly!

Metricaly is a free, open-source, and realtime monitoring and observability solutions, you send your Metrics through the API and Metrically takes care of aggregating, sampling, and storing the data.
You can create highly customizable dashboards using multiple widgets to plot and easily monitor your applications and infrastructure.

> Disclaimer: Metricaly is a hoby project of mine, I started it because
> I wanted to extand and learn more about ASP.NET Core and Angular, it
> is provided as is, with no warranty, I don't recommend using it in
> production because it is simply still immature and requires more work,
> that said, I hope you can try it out still (maybe as part of your side project :) ).

# How does it work?
1. Sign up and get an API key.
2. 

# Features

- Free.
- Opensource.
- Metrics storage is fully based on **redis** (which makes it fast, real fast).
- 1-second granularity.
- Customizable dashboards and widgets.
- Up to 1 year data retention period at higher granularity.
- Realtime.
- Easy to use API.

## Technologies

- ASP.NET Core 3.1
- Entity Framework Core 3.1
- Angular 9
- MediatR
- AutoMapper
- FluentValidation
- StackExchange.Redis
- NUnit, FluentAssertions and Moq

## Data Storage

Two data storage solutions are used, PostgreSQL and redis.
**PostgreSQL**: for storing identity details, widgets and dashboards, as well as metrics names and namespaces.
**Redis**: for storing time series metrics data.

## Use Cases
You can use metricaly to monitor any type of application as long as it has access to the internet.



## Deployment of Metricaly

Metricaly uses Azure's pipelines to automatically builds and deploys to a stand alone server where the Web Application and the Public API are both deployed to two seperate docker containers using docker-compose.

## TODOs
Things that I'd like to add/fix in Metricaly in the future:

- [ ] Old metrics data should be downsampled and moved to a redis Stream.
- [ ] Dashboards should use SignalR to load live metrics data instead of HTTP polling.
- [ ] Add Gauge widget type.
- [ ] Add Table widget type.
- [ ] Add Bar chart widget type.
- [ ] Share Dashboards publicaly or protected with a password.


## Run Metricaly on your own


    dotnet ef database update -c applicationdbcontext -p ../Metricaly.Infrastructure/Metricaly.Infrastructure.csproj -s Metricaly.Angular.csproj

dotnet ef migrations add AddedDashboardTables2 --context applicationdbcontext -p ../Metricaly.Infrastructure/Metricaly.Infrastructure.csproj -s Metricaly.Angular.csproj -o Data/Migrations
