# Ag-Grid Server side using Postgres as the backend DB

## About

This is a basic implementation of Postgresql as the server side provider for AG-Grid for Angular and Dotnet.

The implementation follows the guide of the Server-Side Row Model [SSRM](<https://www.ag-grid.com/angular-data-grid/server-side-model>) at the Ag-grid documentation site.

Similar to the samples provided by [Ag-grid](<https://www.ag-grid.com/angular-data-grid/server-side-model-infinite-scroll/>), this implementation provides infinite scroll, row grouping, filtering and pivoting for the data set Olympic winners.

## Live demo

This code is running as an Azure static app with an API based on Azure function [here](<https://agreeable-cliff-078d0f910.2.azurestaticapps.net>)

## Getting started

These instructions will get you a copy of the project up and running on your local machine for development and testing purposes.

### Prerequisites

You need to have a machine with dotnet > 8, node > 18 and npm > 8 installed.

``` bash
$ dotnet --version
8.0.100

$ node --version  
v18.12.1

$ npm --version
8.19.2
```

You will also need to setup the table and insert the sample records in your Postgresql database.

* Create the olympic_winners table using the [\Data\Setup.sql](..\Data\Setup.sql) script.
* Insert the sample records using the [\Data\Data.sql](..\Data\Data.sql) script.

### Set the required environment variables

You will need to define the db_connection in your config file or as a user-secret to connect to your Postgresql database.

#### For example: using dotnet user-secrets

``` sh
dotnet user-secrets init

dotnet user-secrets set "db_connection" "Server=yourServer;Port=5432;user id=userId;password=yourPassword" --project Api
```

### Installing, Testing, Building

The build process is handled by dotnet and npm. Both applications will download any required libraries.

#### To run the backend API

Run the following command on the root folder:

```sh
dotnet run --project WebApi
```

The Api will be listening on the <http://localhost:5049> address. The application logs should display something similar to:

``` sh

Building...
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: http://localhost:5049
info: Microsoft.Hosting.Lifetime[0]
      Application started. Press Ctrl+C to shut down.
info: Microsoft.Hosting.Lifetime[0]
      Hosting environment: Development
info: Microsoft.Hosting.Lifetime[0]
      Content root path: /Users/berxpert/code/ag-grid-server-side-postgresql/Api
```

#### To run the Angular client

On a second terminal, run the following command on the root folder:

```sh
cd Client
npm install
npm start
```

The Angular Web app will be hosted at <http://localhost:4200/>

The npm command log should display something like:

``` sh
Initial Chunk Files   | Names         |  Raw Size
vendor.js             | vendor        |   6.74 MB | 
styles.css, styles.js | styles        | 415.79 kB | 
polyfills.js          | polyfills     | 314.27 kB | 
main.js               | main          |  10.81 kB | 
runtime.js            | runtime       |   6.51 kB | 

                      | Initial Total |   7.47 MB

Build at: 2022-12-22T21:02:06.893Z - Hash: 5467cf94f11cf08c - Time: 2578ms

** Angular Live Development Server is listening on localhost:4200, open your browser on http://localhost:4200/ **


✔ Compiled successfully.
```

If you need help using the Ag-Grid visit the official [documentation](<https://www.ag-grid.com/angular-data-grid/server-side-model/>).

#### Test Ag grid request

You can call the API directly simulating the Ag-grid requests from the Client. Some samples are in the requests folder, for example the [BasicQuery.http](..\Requests\BasicQuery.http) request returns all the fields for the rows 5 and 6 on the dataset:

```json
POST http://localhost:5049/api/winners
Content-Type: application/json

{
    "StartRow" : 5,
    "EndRow" : 6,
    "RowGroupCols" :[
    ],
    "ValueCols" : [],
    "PivotCols" : [],
    "GroupKeys" : [],
    "SortModel" : [],
    "filterModel": {}
}
```

Response:

```json
{
  "data": [
    {
      "athlete": "Alicia Coutts",
      "age": 24,
      "country": "Australia",
      "country_group": "A",
      "year": 2012,
      "date": "12/08/2012",
      "sport": "Swimming",
      "gold": 1,
      "silver": 3,
      "bronze": 1,
      "total": 5
    },
    {
      "athlete": "Missy Franklin",
      "age": 17,
      "country": "United States",
      "country_group": "U",
      "year": 2012,
      "date": "12/08/2012",
      "sport": "Swimming",
      "gold": 4,
      "silver": 0,
      "bronze": 1,
      "total": 5
    }
  ],
  "lastRow": 0,
  "secondaryColumns": []
}
```
