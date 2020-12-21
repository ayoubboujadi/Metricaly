
 <img align="left" width="170" height="110" src="https://i.imgur.com/1B04gWW.png" />

# Welcome to Metricaly!
Public API: [![Build Status](https://ayoubbou.visualstudio.com/Metricaly/_apis/build/status/Public%20API%20-%20CI%20-%20Docker%20Compose?branchName=docker)](https://ayoubbou.visualstudio.com/Metricaly/_build/latest?definitionId=1&branchName=docker) Web App: [![Build Status](https://ayoubbou.visualstudio.com/Metricaly/_apis/build/status/Web%20App%20-%20CI%20-%20Docker%20Compose?branchName=docker)](https://ayoubbou.visualstudio.com/Metricaly/_build/latest?definitionId=4&branchName=docker)

<br/>

Metricaly is a free, open-source, and realtime monitoring solution, Metricaly provides an API where Metrics data can be sent where it is automatically aggregated, sampled, and stored.
Highly customizable dashboards can be created using multiple widgets to plot and easily monitor your applications, services, servers, or infrastructures.

> Disclaimer: Metricaly is a hoby project of mine, I started it because
> I wanted to extand and learn more about ASP.NET Core and Angular, it
> is provided as is, with no warranty, I don't recommend using it in
> production because it is simply still immature and requires more work,
> that said, I hope you can still try it out (maybe as part of your side project :) ).

# How does it work?
1. Sign up and get an API key.
2. Send the Metric data using one of the API endpoints.
3. Create a Widget in the web portal that plots the Metric.
4. Add the created widget to a new or existing Dashboard.

# Features

- Free.
- Open-source.
- Metrics storage is fully based on **redis** (which makes it fast, real fast).
- 1-second granularity.
- Customizable dashboards and widgets.
- Up to 1 year data retention period at higher granularity.
- Realtime.
- Easy to use API.

## Technologies

- Clear Architecture
- ASP.NET Core 3.1
- Entity Framework Core 3.1
- Angular 9
- MediatR
- AutoMapper
- FluentValidation
- StackExchange.Redis
- NSwag
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
