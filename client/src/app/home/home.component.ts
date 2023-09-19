import { Component, IterableDiffers, OnInit, ViewChild } from '@angular/core';
import { SensorModel } from '../models/sensor.model';
import { DevicesService } from '../services/devices.service';
import { RecordModel } from '../models/record.model';
import { interval } from 'rxjs';
import { MatDialog } from '@angular/material/dialog';
import { ReportPageComponent } from '../report-page/report-page.component';
import { GoogleMap } from '@angular/google-maps';
import { MatTableDataSource } from '@angular/material/table';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.scss']
})
export class HomeComponent implements OnInit{
  sensors: SensorModel[] = []

  displayedColumns: string[] = ['icon', 'report','no.', 'latitude', 'longitude',
    'recordTime', 'totalRecords', 'delete']
  displayedColumnsDescription: string[] = ['icon-desc', 'report-desc', 'no.-desc',
    'latitude-desc', 'longitude-desc', 'recordTime-desc', 'totalRecords-desc',
    'delete-desc']
  dataSource = new MatTableDataSource<SensorModel>();
  defaultLat = 41.699591;
  defaultLng = 44.788348;
  defaultDistance = 50;
  defaultNumberOfDays = 7;
  defaultRecordTimer = 60;
  defaultTotalRecords = 4;
  @ViewChild(GoogleMap) map: GoogleMap;


  mapOptions: google.maps.MapOptions = {
    center: { lat: this.defaultLat, lng: this.defaultLng},
    zoom: 9,
    streetViewControl: false,
  }

  options = {
    draggable: true,
    animation: google.maps.Animation.DROP
  }

  constructor(private devices: DevicesService, public dialog: MatDialog) { }

  ngOnInit(): void {
    this.getSensors()
  }

  adjustDefaults() {
    this.defaultLat = this.map.getCenter().lat();
    this.defaultLng = this.map.getCenter().lng();
  }

  getSensors() {
    this.devices.getSensors().subscribe(response => {
      this.sensors = response

      this.refresh()
      console.log(this.sensors)
    });
  }

  populateSensors() {
    this.devices.populateSensors(this.defaultLat, this.defaultLng, this.populateDistance,
      4, this.populateRecordTimer, this.populateTotalRecords).subscribe(response => {
        this.refresh()
        console.log(this.sensors)
      })
  }

  populateDistance = 50;
  populateRecordTimer = 15;
  populateTotalRecords = 4;

  populateRecords() {
    this.devices.populateRecords(this.populateNumberOfSensors, this.populateNumberOfDays).subscribe(() => {
      console.log('records populated')
    })
  }

  populateNumberOfSensors = 4
  populateNumberOfDays = 7

  getStatisticsById(id: number) {
    this.devices.getDeviceStatisticsById(id, this.defaultNumberOfDays)
    .subscribe(response => {
      console.log(response)
      this.openDialog(response)
    });
  }

  getStatisticsByIds() {
    this.devices.getDeviceStatisticsUsersSelection([1, 2,3], 5)
  }

  getStatisticsWithinRange() {
    this.devices.getDeviceStatisticsWithinDistance(this.defaultLat, this.defaultLng,
      this.statisticsDistance, this.statisticsNumberOfDays).subscribe(response => {
       console.log(response);
       this.openDialog(response)
      })
  }

  statisticsDistance = 50;
  statisticsNumberOfDays = 7;

  getSensorsAll() {
    this.devices.getSensorsAll().subscribe(response => 
      console.log('sensors', response));
  }

  getRecordsAll() {
    this.devices.getRecordsAll().subscribe(response => 
      console.log('reports', response));
  }

  //for testing only
  publishRecord(sensor: SensorModel) {
    let startDate = new Date().getTime() + (new Date().getTimezoneOffset() * 60000);

    for (let i = 0; i < 3; i++)
    {
      let record = new RecordModel
      ({
        latitude: sensor.latitude,
        longitude: sensor.longitude,
        time: new Date(startDate + (i * 60000 * 15))
      })
      sensor.records.push(record)
    }

    this.devices.publishRecords(sensor.id, sensor.records)
  }

  deleteSensor(id: number) {
    this.devices.deleteSensor(id).subscribe(() => {
      this.sensors = this.sensors.filter(x => x.id != id);
      console.log(this.sensors)
    })
  }

  startRecording(sensor: SensorModel) {
    sensor.recording = true;
    sensor.recordSubscription = interval(sensor.recordTimer * 60000).subscribe(val => 
      {
        let record = new RecordModel
        ({
          sensorId: sensor.id,
          latitude: sensor.latitude,
          longitude: sensor.longitude,
          time: new Date()
        })

        if (!sensor.records)
        {
          sensor.records = []
        }

        sensor.records.push(record);

        console.log('Sensor N-' + sensor.id + ' report N-' + (val + 1), sensor.records, record)

        if (sensor.records.length >= sensor.totalRecords)
        {
          let records: RecordModel[] = [...sensor.records];
          sensor.records = []
          this.devices.publishRecords(sensor.id, records).subscribe(response =>
            console.log('Successfully sent to database', records))
        }
      })
  }

  stopRecording(sensor: SensorModel) {
    sensor.recording = false;
    sensor.recordSubscription.unsubscribe();
  }

  openDialog(records: RecordModel[]): void {
    const dialogRef = this.dialog.open(ReportPageComponent, {
      width: '80%',
      height: '70%',
      data: records,
    });
  }

  updatePending(id: number, sensor: SensorModel) {
    this.devices.editSensor(id, sensor).subscribe(() => {
      console.log('updating')
    })
  }

  onDragEnd(id: number, event: google.maps.MapMouseEvent) {
    let sensor = this.sensors.find(x => x.id == id);
    sensor.latitude = event.latLng.lat();
    sensor.longitude = event.latLng.lng();
    this.updatePending(id, sensor)
  }

  addSensor() {
    let sensor = new SensorModel ()
    sensor.latitude = this.defaultLat
    sensor.longitude = this.defaultLng
    sensor.recordTimer = this.defaultRecordTimer
    sensor.totalRecords = this.defaultTotalRecords

    this.devices.addSensor(sensor).subscribe(response => {

      this.refresh()
      console.log(this.sensors)
    })
  }

  refresh() {
    this.dataSource.data = this.sensors
  }
}
