# Ag-Grid Server side using Postgres as the backend DB

This a basic implementation of Postgres as the server side provider for AG-Grid

Similar to the [Oracle](<https://www.ag-grid.com/archive/25.0.0/documentation/angular/server-side-operations-oracle/>) implementation provided by Ag-Grid.

## To run locally

You need dotnet 7 installed.

You will need to define the db_connection in your config file or as a user-secret.

``` sh
dotnet user-secrets init

dotnet user-secrets set "db_connection" "Server=yourServer;Port=5432;user id=userId;password=yourPassword" --project Api
```

To run the API

```sh
dotnet run --project Api
```

## Ag grid request

```json
{
    "StartRow" : 5,
    "EndRow" : 7,
    "RowGroupCols" :[
       {
        "Id" : "FieldId",
        "DisplayName" : "The display",
        "Field" : "Field",
        "AggFunc" : "Sum"
        } 
    ]
}
```

## API response

```json
[
  {
    "athlete": "Alicia Coutts",
    "age": 24,
    "country": "Australia",
    "countryGroup": null,
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
    "countryGroup": null,
    "year": 2012,
    "date": "12/08/2012",
    "sport": "Swimming",
    "gold": 4,
    "silver": 0,
    "bronze": 1,
    "total": 5
  }
]
```
