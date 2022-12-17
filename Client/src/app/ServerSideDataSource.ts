import { HttpClient } from "@angular/common/http";
import { GridOptions, IServerSideDatasource, IServerSideGetRowsParams } from "ag-grid-community";
import { catchError, last, observable, throwError } from "rxjs";
import { OlympicWinnerModel } from "src/model/OlympicWinnerModel";

export class ServerSideDatasource implements IServerSideDatasource {

    constructor(private gridOptions: GridOptions,
        private http: HttpClient, private baseUrl: string) {
    }

    getRows(params: IServerSideGetRowsParams<any>): void {
        console.log(JSON.stringify(params.request, null, 1));

        console.log(this.baseUrl+ "OlympicWinners/winners");

        this.http.post<OlympicWinnerModel[]>(this.baseUrl + "OlympicWinners/winners", params.request)
            .pipe(catchError(err => throwError(err)))
            .subscribe(response => {
                const rows = response;

                // determine last tow size scrollbar and last block sie correctly
                let lastRow = -1;

                if(rows.length <= (this.gridOptions.cacheBlockSize??100))
                {
                    lastRow = params.request.startRow! + rows.length;
                    console.log("Reached the end:" + lastRow);
                }

                params.success({
                    rowData: rows,
                    rowCount: lastRow
                });
            });
    }
}