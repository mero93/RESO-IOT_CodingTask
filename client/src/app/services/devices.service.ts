import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { SensorModel } from '../models/sensor.model';
import { HttpClient, HttpParams } from '@angular/common/http';
import { map, take, timer } from 'rxjs';
import { RecordModel } from '../models/record.model';

@Injectable({
  providedIn: 'root'
})
export class DevicesService {
  baseUrl = environment.baseUrl + '/api/devices/'
  sensors: SensorModel[] = [];


  constructor(private http: HttpClient) { }

  getSensors() {
    return this.http.get<SensorModel[]>(this.baseUrl).pipe(
      map(response => {
        this.sensors = response;
        return response;
      }),
      take(1))
  }

  getDeviceStatisticsById(id: number, numberOfDays: number) {
    let params = new HttpParams();
    params = params.append('numberOfDays', numberOfDays);

    return this.http.get<RecordModel[]>(this.baseUrl + 'statistics/' + id + '/', { params: params} )
    .pipe(take(1));
  }

  getDeviceStatisticsWithinDistance(lat: number, lng: number, distance: number,
    numberOfDays: number) {
    let params = new HttpParams();
    params = params.append('lat', lat);
    params = params.append('lng', lng);
    params = params.append('distance', distance)
    params = params.append('numberOfDays', numberOfDays)


    return this.http.get<RecordModel[]>(this.baseUrl + 'statistics/within-distance/', { params: params} ).pipe(
      take(1));
  }

  getDeviceStatisticsUsersSelection(ids: number[], numberOfDays: number) {
    let params = new HttpParams();
    let idsString = ids.join('-');
    params = params.append('sensorIdsString', idsString);
    params = params.append('numberOfDays', numberOfDays);

    return this.http.get<RecordModel[]>(this.baseUrl + 'statistics/user-selection/', { observe: "response", params: params} ).pipe(
      take(1));
  }

  addSensor(sensor: SensorModel) {
    return this.http.post<SensorModel>(this.baseUrl + 'add-sensor', sensor).pipe(
      take(1),
      map(response => {
        this.sensors.unshift(response)
        return response
      }))
  }

  editSensor(id: number, sensor: SensorModel) {
    return this.http.post<SensorModel>(this.baseUrl + id + '/edit-sensor', sensor).pipe(
      take(1))
  }

  deleteSensor(id: number) {

    return this.http.delete(this.baseUrl + id + '/remove-sensor').pipe(
      take(1),
      map(response => 
        {
          console.log('next filed')
          this.sensors = this.sensors.filter(x => x.id != id);
          return response;
        }))
  }

  publishRecords(id: number, records: RecordModel[]) {
    return this.http.post<SensorModel>(this.baseUrl + id + '/telemetry', records).pipe(
      take(1));
  }

  //For testing only
  populateSensors(lat: number, lng: number, distance: number,
    numberOfSensors: number, recordTimer: number, totalRecords: number) {
    let params = new HttpParams();
    params = params.append('lat', lat);
    params = params.append('lng', lng)
    params = params.append('distance', distance)
    params = params.append('numberOfSensors', numberOfSensors)
    params = params.append('recordTimer', recordTimer)
    params = params.append('totalRecords', totalRecords)

    return this.http.post<SensorModel[]>(this.baseUrl + 'populate-sensors/', null, { params: params}).pipe(
      map( response => {
        this.sensors.unshift(...response) 
        return response}),
      take(1))
  }

  //For testing only
  populateRecords(numberOfSensors: number, numberOfDays: number) {
    let params = new HttpParams();
    params = params.append('numberOfSensors', numberOfSensors)
    params = params.append('numberOfDays', numberOfDays)

    return this.http.post<SensorModel[]>(this.baseUrl + 'populate-records/', null, { params: params}).pipe(
      take(1))
  }

  //For testing only
  getSensorsAll() {
    return this.http.get<SensorModel[]>(this.baseUrl + 'get-all-sensors').pipe(
      take(1));
  }
  
  //For testing only
  getRecordsAll() {
    return this.http.get<RecordModel[]>(this.baseUrl + 'get-all-records').pipe(
      take(1));
  }

  sensorRecord(sensor: SensorModel) {
    return timer(0, 60000 * sensor.recordTimer).pipe(
      map(() => {
        let newRecord = new RecordModel
        ({
          sensorId: sensor.id,
          time: new Date(),
          latitude: sensor.latitude,
          longitude: sensor.longitude,
        });

        sensor.records.push(newRecord);

        if (sensor.records.length >= sensor.totalRecords) {
          this.publishRecords(sensor.id, sensor.records)
        }
      })
    )
  }
}
