using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Data.DTOs;

namespace API.Interfaces
{
    public interface IRecordsRepository
    {
        Task CreateRecordAsync(int id, RecordDto properties);

        Task<IEnumerable<RecordDto>> ReportSelectedDevice(int sensorId, int numberOfDays);

        Task<IEnumerable<RecordDto>> ReportWithinDistance(double lat, double lng,
            double distance, int numberOfDays);

        Task<IEnumerable<RecordDto>> ReportMultipleSelectedDevices(
            IEnumerable<int> sensorIds, int numberOfDays);

        Task PopulateRecords(ICollection<SensorDto> sensors, int numberOfDays);

        Task<ICollection<RecordDto>> GetAllRecords();
    }
}