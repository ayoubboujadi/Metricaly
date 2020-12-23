

 <img align="left" width="170" height="110" src="https://i.imgur.com/1B04gWW.png" />

# Welcome to Metricaly!
Public API: [![Build Status](https://ayoubbou.visualstudio.com/Metricaly/_apis/build/status/Public%20API%20-%20CI%20-%20Docker%20Compose?branchName=docker)](https://ayoubbou.visualstudio.com/Metricaly/_build/latest?definitionId=1&branchName=docker) Web App: [![Build Status](https://ayoubbou.visualstudio.com/Metricaly/_apis/build/status/Web%20App%20-%20CI%20-%20Docker%20Compose?branchName=docker)](https://ayoubbou.visualstudio.com/Metricaly/_build/latest?definitionId=4&branchName=docker)

<br/>

Metricaly is a free, open-source, and realtime monitoring solution, Metricaly provides an API where Metrics data can be sent, automatically aggregated, sampled, and stored.
Highly customizable dashboards can be created using multiple widgets to plot and visualize the Metrics, which allows for an overview of the monitored system.

> Disclaimer: Metricaly is a hoby project of mine, I started it because
> I wanted to extand and learn more about ASP.NET Core and Angular, it
> is provided as is, with no warranty, I don't recommend using it in
> production because it is simply still immature and requires more work,
> that said, I hope you can still try it out (maybe as part of your side project :) ).

# How does it work?
1. Sign up and get an API key.
2. Send the Metric(s) data using one of the API endpoints.
3. Create Widgets and Dashboards to plot and visualize the sent Metrics.

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

## API Documentation
All requests should be sent to the following host: `api.metricaly.com`
The `ApiKey` header should be provided for all requests.

The following API endpoints allow for submission of Metric(s).

### Endpoint 1

**Endpoint:** `/collect/single/{namespace}/{metricName}/{value}`
**Method:** `GET`
**Body:** `none`
**Description:** This is the simplest way to send a single Metric value, also, using this API endpoint will store the sent Metric with the current unix timestamp.

**curl Example:**
```console
curl -X GET \
  https://api.metricaly.com/collect/performance/cpu/62 \
  -H 'ApiKey: 53DtDkHM*******************EqSn9E='
```
---
### Endpoint 2

**Endpoint:** `/collect/multiple`
**Method:** `POST`
**Body:** 

    [{
        "metricName": "string",
        "metricNamespace": "string",
        "value": 0,
        "timestamp": 0
    }]

**Description:** Using this endpoint allows for the epecification of the timestamp, as well as sending multiple Metrics at the same time.

**curl Example:**
```console
curl -X POST \
  https://api.metricaly.com/collect/multiple \
  -H 'ApiKey: 53DtDkHM*******************EqSn9E=' \
  -H 'Content-Type: application/json' \
  -d '[
    {
        "metricName": "cpu",
        "metricNamespace": "performance",
        "value": 62,
        "timestamp": 1608715412
    }
  ]'
```
---
### Endpoint 3

**Endpoint:** `/collect/aggregated`
**Method:** `POST`
**Body:**
```
[
  {
      "metricName": "string",
      "metricNamespace": "string",
      "max": 0,
      "min": 0,
      "sum": 0,
      "samplesCount": 0,
      "timestamp": 0
  }
]
```
**Description:** If Metrics are aggregated on the client side, this endpoint can be used to send their values.

**curl Example:**
```console
curl -X POST \
  https://localhost:44334/collect/aggregated \
  -H 'ApiKey: 53DtDkHM*******************EqSn9E=' \
  -H 'Content-Type: application/json' \
  -H 'cache-control: no-cache' \
  -d '[
    {
        "metricName": "cpu",
        "metricNamespace": "performance",
        "max": 95,
        "min": 32,
        "sum": 763,
        "samplesCount": 12,
        "timestamp": 1608715412
    }
]'
```

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

