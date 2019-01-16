[![Build Status](https://travis-ci.com/louisaxel-ambroise/fastnt.svg?branch=master)](https://travis-ci.com/louisaxel-ambroise/fastnt)

# FasTnT

FasTnT is a simple, lightweight GS1 EPCIS 1.2 repository written in C# using .NET Core 2.2, backed with PostGreSQL database.

## Setup

Prerequisites: 
- PostGreSQL 9.5 or higher
- .NET Core 2.2 SDK

Steps:
1. Download the source code, and create a new user/database in PostGreSQL for FasTnT;
2. Update the `FasTnT.Database` connection string in the project `FasTnT.Host` with your PostGreSQL connection string;
3. Set `FasTnT.Host` project as startup project, and start the solution
4. Make the following request to create the SQL schemas and tables: `curl -X POST http://localhost:54805/Services/1.2/Migrate` (the port number may change depending on your configuration)
5. That's it! You have a properly working EPCIS 1.2 repository.

## Endpoints

### EPCIS endpoints:

- Capture: `/Services/1.2/Capture` 
- Queries : `/Services/1.2/Query`
- Subscriptions : `/Services/1.2/Subscription`

### Others endpoints:

- Database migration: `/Services/1.2/Migrate`

The file `documents\EPCIS_Samples.postman_collection.json` contains examples of HTTP requests that you can perform on FasTnT (import and run it in [PostMan](https://www.getpostman.com/))

## Implemented Features

- Capture
  - Events
  - Master Data (CBV)
- Queries:
  - GetVendorVersion
  - GetStandardVersion
  - GetQueryNames
  - Poll 
    - SimpleEventQuery _(still a few parameters missing)_
- Subscriptions:
  - Get all subscription names
  - Subscribe to an EPCIS request 
  - Unsubscribe from EPCIS repository
  - Trigger subscriptions that register to specific trigger name

# License

This project is licensed under the Apache 2.0 license - see the LICENSE file for details

_Last update: january 2019_
