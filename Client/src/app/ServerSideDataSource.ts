import { HttpClient } from "@angular/common/http";
import { ColDef, ColGroupDef, ColumnApi, ColumnVO, GridOptions, IServerSideDatasource, IServerSideGetRowsParams, IServerSideGetRowsRequest } from "ag-grid-community";
import { catchError, last, observable, throwError } from "rxjs";
import { OlympicWinnerModel } from "src/model/OlympicWinnerModel";
import { ServerSideResult } from "src/model/ServerSideResult";

export class ServerSideDatasource implements IServerSideDatasource {

    constructor(private gridOptions: GridOptions,
        private http: HttpClient, private baseUrl: string) {
    }

    getRows(params: IServerSideGetRowsParams<any>): void {
        console.log(JSON.stringify(params.request, null, 1));

        console.log(this.baseUrl + "api/winners");

        this.http.post<ServerSideResult<OlympicWinnerModel>>(this.baseUrl + "api/winners", params.request)
            .pipe(catchError(err => throwError(err)))
            .subscribe(response => {
                const rows = response.data || [];

                // determine last tow size scrollbar and last block sie correctly
                let lastRow = -1;

                if (rows.length <= (this.gridOptions.cacheBlockSize ?? 100)) {
                    lastRow = params.request.startRow! + rows.length;
                    console.log("Reached the end:" + lastRow);
                }

                params.success({
                    rowData: rows,
                    rowCount: lastRow
                });

                this.updateSecondaryColumns(params.request, response, params.columnApi);
            });
    }

    private updateSecondaryColumns(request: IServerSideGetRowsRequest, 
        response: ServerSideResult<OlympicWinnerModel>,
        columnApi: ColumnApi) {
        const valueCols = request.valueCols;
        if (request.pivotMode && request.pivotCols.length > 0 && valueCols.length > 0) {
            const secondaryCols = this.createSecondaryColumns(response.secondaryColumns, valueCols);
            columnApi.setPivotResultColumns(secondaryCols);
        }
        else {
            columnApi.setPivotResultColumns([]);
        }
    }

    private createSecondaryColumns(secondaryColumns: string[], valueCols: ColumnVO[]): (ColDef | ColGroupDef)[] {
        const secondaryCols: any[] = [];

        function addColDef(colId: string, parts: any[], res: any[]): (ColDef | ColGroupDef)[] {
            if (parts.length == 0) {
                return [];
            }

            const firstPart = parts.shift();
            const existing = res.find(r => r.groupID === firstPart);

            if (existing) {
                existing.children = addColDef(colId, parts, existing.children);
            }
            else {
                let colDef: (ColDef | ColGroupDef);
                const isGroup = parts.length > 0;
                if (isGroup) {
                    colDef = {
                        groupId: firstPart,
                        headerName: firstPart
                    } as ColGroupDef;
                }
                else {
                    const valueCol = valueCols.find(c => c.field === firstPart);
                    colDef = {
                        colId: colId,
                        headerName: valueCol?.displayName,
                        field: colId,
                        width: 100,
                        suppressMenu: true
                    } as ColDef;
                }
                const children = addColDef(colId, parts, []);
                if (children.length > 0) {
                    (colDef as ColGroupDef).children = children;
                }

                res.push(colDef);
            }

            return res;
        }

        secondaryColumns.forEach(colId => {
            const parts = colId.split("|");
            addColDef(colId, parts, secondaryCols);
        });

        return secondaryCols;
    }
}