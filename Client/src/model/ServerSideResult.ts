export interface ServerSideResult<T> { 
    data: T[];
    lastRow: number;
    secondaryColumns: string[];
}