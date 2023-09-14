export class RecordModel {
    id: number
    weather: string | null;
    temperature: number;
    illumination: number;
    latitude: number;
    longitude: number;
    time: Date;
    sensorId: number;

    public constructor(init?:Partial<RecordModel>) {
        Object.assign(this, init);
    }
}