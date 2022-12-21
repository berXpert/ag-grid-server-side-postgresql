import { HttpClient } from '@angular/common/http';
import { Component, Inject, OnInit } from '@angular/core';
import {
  ColDef,
  GridOptions,
  RowModelType,
  SideBarDef,
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
  serverSideDatasource: ServerSideDatasource;

  public columnDefs: ColDef[] = [
    { field: 'athlete', minWidth: 220, filter: 'agTextColumnFilter' },
    {
      field: 'country', minWidth: 200, filter: 'agTextColumnFilter',
      enableRowGroup: true,
      enablePivot: true,
      rowGroup: true,
    },
    { field: 'year', filter: 'agNumberColumnFilter' },
    {
      field: 'sport',
      enableRowGroup: true,
      enablePivot: true,
      pivot: true,
      rowGroup: true,
      filter: 'agTextColumnFilter',
    },
    { field: 'gold', aggFunc: 'sum', filter: 'agNumberColumnFilter' },
    { field: 'silver', aggFunc: 'sum', filter: 'agNumberColumnFilter' },
    { field: 'bronze', aggFunc: 'sum', filter: 'agNumberColumnFilter' },
  ];
  public defaultColDef: ColDef = {
    flex: 1,
    minWidth: 120,
    resizable: true,
    sortable: true,
  };
  public rowGroupPanelShow: 'always' | 'onlyWhenGrouping' | 'never' = 'always';
  public sideBar: SideBarDef | string | string[] | boolean | null = 'columns';

  public autoGroupColumnDef: ColDef = {
    flex: 1,
    minWidth: 280,
  };
  public rowModelType: RowModelType = 'serverSide';
  public cacheBlockSize = 5;

  constructor(private http: HttpClient, @Inject('BASE_URL') baseUrl: string) {
    this.gridOptions = {
      rowModelType: this.rowModelType,
      columnDefs: this.columnDefs,
      defaultColDef: this.defaultColDef,
      autoGroupColumnDef: this.autoGroupColumnDef,
      cacheBlockSize: this.cacheBlockSize,
      rowGroupPanelShow: 'always',
      pivotPanelShow: 'always',
    } as GridOptions;

    this.serverSideDatasource = new ServerSideDatasource(this.gridOptions, this.http, baseUrl);
  }
}
