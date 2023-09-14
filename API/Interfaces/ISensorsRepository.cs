using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Data.DTOs;

namespace API.Interfaces
{
    public interface ISensorsRepository
    {
        Task<ICollection<SensorDto>> GetUserSensorsAsync(string clientId);

        Task<(SensorDto, bool)> CreateSensorAsync(SensorDto properties);

        Task UpdateSensorAsync(SensorDto properties);

        Task RemoveSensorAsync(int id);

        Task<(ICollection<SensorDto>, bool)> PopulateSensors(double lat, double lng,
            double distance, int numberOfSensors, string ClientId,
            int recordTimer, int totalRecords);

        Task<ICollection<SensorDto>> GetAllSensors();

        Task<ICollection<SensorDto>> GetLatestSensors(int numberOfSensors, string clientId);
    }
}