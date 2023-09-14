import { Component, Inject } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { RecordModel } from '../models/record.model';
import { Clipboard } from '@angular/cdk/clipboard';

@Component({
  selector: 'app-report-page',
  templateUrl: './report-page.component.html',
  styleUrls: ['./report-page.component.scss']
})
export class ReportPageComponent {
  constructor(public dialogRef: MatDialogRef<ReportPageComponent>,
    @Inject(MAT_DIALOG_DATA) public data: RecordModel[],
    private clipboard: Clipboard) {}

  displayedColumns: string[] = ['date', 'sensorId','id', 'latitude', 'longitude',
    'weather', 'illumination', 'temperature', 'time']

  close() {
    this.dialogRef.close()
  }

  copy() {
    const object = {abc:'abc',xy:{x:'1',y:'2'}}

    this.clipboard.copy(JSON.stringify(this.data, null, 2));
    // this.clipboard.copy(JSON.stringify(object, null, 2));
  }
}
