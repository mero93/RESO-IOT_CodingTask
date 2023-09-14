import { Subscription, interval } from "rxjs";
import { RecordModel } from "./record.model";

export class SensorModel {
    id: number;
    clientId: string | null;
    latitude: number;
    longitude: number;
    recordTimer: number;
    totalRecords: number;
    records: RecordModel[] = [];
    recording = false;
    recordSubscription: Subscription;
}