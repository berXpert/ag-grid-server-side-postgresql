# Ag-Grid Server side using Postgres as the backend DB

This a basic implementation of Postgres as the server side provider for AG-Grid

Similar to the [Oracle](<https://www.ag-grid.com/archive/25.0.0/documentation/angular/server-side-operations-oracle/>) implementation provided by Ag-Grid.

## To run locally

You will need to define the db_connection in your config file or as a user-secret.

``` sh
dotnet user-secrets init

dotnet user-secrets set "db_connection" "Server=yourServer;Port=5432;user id=userId;password=yourPassword"
```

To run the API

``` sh
dotnet run --project Api
```

## Ag grid request

``` json
GetRowsRequest
{
    "StartRow" : 0,
    "EndRow" : 0,
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
