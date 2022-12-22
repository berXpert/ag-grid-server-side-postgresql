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

  public pivotMode = true;
  public columnDefs: ColDef[] = [
    { field: 'country', rowGroup: true },
    { field: 'sport', rowGroup: true },
    { field: 'year', pivot: true },
    { field: 'total', aggFunc: 'sum' },
    { field: 'gold', aggFunc: 'sum' },
    { field: 'silver', aggFunc: 'sum' },
    { field: 'bronze', aggFunc: 'sum' },
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
