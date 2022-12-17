import { HttpClient } from '@angular/common/http';
import { Component, Inject, OnInit } from '@angular/core';
import {
  GridOptions,
  RowModelType,
} from 'ag-grid-community';
import 'ag-grid-enterprise';
import { OlympicWinnerModel } from 'src/model/OlympicWinnerModel';
import { ServerSideDatasource } from './ServerSideDataSource';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styles: []
})
export class AppComponent {
  title = 'Ag-Grid Server side - dotnet - Postgresql';

  public gridOptions: GridOptions;
  public rowData!: OlympicWinnerModel[];
  public rowModelType: RowModelType = 'serverSide';
  serverSideDatasource: ServerSideDatasource;

  constructor(private http: HttpClient, @Inject('BASE_URL') baseUrl: string) {
    this.gridOptions = {
      rowModelType: this.rowModelType,
      columnDefs: [
        { field: 'athlete', minWidth: 220 },
        { field: 'country', minWidth: 200 },
        { field: 'year' },
        { field: 'sport', minWidth: 200 },
        { field: 'gold' },
        { field: 'silver' },
        { field: 'bronze' },
      ],
      defaultColDef: {
        sortable: true
      }
    } as GridOptions;

    this.serverSideDatasource = new ServerSideDatasource(this.gridOptions, this.http, baseUrl);
  }
}
