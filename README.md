![](https://github.com/FasTnT/epcis/workflows/.NET%20Core/badge.svg)
[![Maintainability](https://api.codeclimate.com/v1/badges/40672e48b92da57852d7/maintainability)](https://codeclimate.com/github/FasTnT/epcis/maintainability)
[![codecov](https://codecov.io/gh/FasTnT/epcis/branch/develop/graph/badge.svg)](https://codecov.io/gh/FasTnT/epcis)

# FasTnT EPCIS

FasTnT EPCIS is a lightweight GS1 EPCIS 1.2 repository written in C# using .NET Core 3.1, backed with PostGreSQL database.

## Setup

Prerequisites:
- PostGreSQL 9.5 or higher
- .NET Core 3.1 SDK

Steps:
1. Download the source code, and create a new user/database in PostGreSQL for FasTnT ;
2. Update the connection string: `$ dotnet user-secrets set ConnectionStrings:FasTnT.Database "{your connectionstring}" -p src\FasTnT.Host\FasTnT.Host.csproj` ;
3. Start the repository with the command `$ dotnet run -p src\FasTnT.Host\FasTnT.Host.csproj --urls "http://localhost:5102/"` ;
4. Create the SQL schemas and tables: `curl -X POST http://localhost:5102/Setup/Database/Migrate -d ""` ;
5. That's it! You have a properly working EPCIS 1.2 repository.

## HTTP Endpoints

### EPCIS 1.2 endpoints:

The API is secured using HTTP Basic authentication. The default username:password value is `admin:P@ssw0rd`

- Capture: `POST /v1_2/Capture`
- Queries : `POST /v1_2/Query` or `POST /v1_2/Query.svc`
- Subscription trigger : `GET /v1_2/Subscription/Trigger/{triggerName}`

**Capture** endpoint only supports requests with `content-type: application/xml` or `content-type: text/xml` header and XML payload.

**Queries** endpoint supports XML requests on endpoint `/v1_2/Query` and SOAP requests on endpoint `/v1_2/Query.svc`. Note that it will not return the wsdl on a `GET` request..

The file `documents\EPCIS Examples - 1.2.postman_collection.json` contains XML requests examples to be run in [PostMan](https://www.getpostman.com/), and the file `EPCglobal-epcis-query-1-2-soapui-project.xml` contains a project with SOAP example requests to be run in [SoapUI](https://www.soapui.org/open-source.html).

### Others endpoints:

- Database migration: `POST /Setup/Database/Migrate`
- Database rollback: `POST /Setup/Database/Rollback`

These database endpoints are only available when the EPCIS server is in Development configuration.

See the [wiki](https://github.com/FasTnT/epcis/wiki) for more details.

## Implemented Features

- Capture
  - Events
  - Master Data (CBV)
- Queries:
  - GetVendorVersion
  - GetStandardVersion
  - GetQueryNames
  - GetSubsciptionIDs
  - Poll
    - SimpleEventQuery
    - SimpleMasterDataQuery
- Query Callback:
  - CallbackResults
  - CallbackQueryTooLargeException
  - CallbackImplementationException
- Subscriptions:
  - Subscribe to an EPCIS request
  - Unsubscribe from EPCIS repository
  - Trigger subscriptions that register to specific trigger name

# Authors

External contributions on FasTnT EPCIS repository are welcome from anyone. Many thanks to the people who already shown interest or contributed to this project ([@grudolf](https://github.com/grudolf), [@jnoruzi](https://github.com/jnoruzi) and many others).

FasTnT EPCIS is primarily maintained by Louis-Axel Ambroise.

# License

This project is licensed under the Apache 2.0 license - see the LICENSE file for details

Contact: fastnt@pm.me

_Last update: March 2020_
