using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Data.DTOs;
using API.Data.Entities;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace API.Data.Repositories
{
    public class RecordsRepository: IRecordsRepository
    {
        private readonly IMapper _mapper;
        private readonly DataContext _context;
        private readonly ISensorSimulator _simulator;
        public RecordsRepository(IMapper mapper, DataContext context, ISensorSimulator simulator)
        {
            _simulator = simulator;
            _context = context;
            _mapper = mapper;
        }

        //Adds single record to database
        public async Task CreateRecordAsync(int id, RecordDto properties)
        {
            var entity = await _simulator.Simulate(id, properties);

            await _context.AddAsync(entity);
        }

        //Returns Daily Reports for single Sensor
        public async Task<IEnumerable<RecordDto>> ReportSelectedDevice(int sensorId, int numberOfDays)
        {
            DateTime startDate;

            var query = GetRecordsFromPastDays(numberOfDays, out startDate);

            query = query.Where(x => x.SensorId == sensorId);

            var result = await GetHighestValues(query, startDate);

            return result;
        }

        //Returns Daily Reports from multiple Sensors
        public async Task<IEnumerable<RecordDto>> ReportMultipleSelectedDevices(IEnumerable<int> sensorIds, int numberOfDays)
        {            
            DateTime startDate;

            var query = GetRecordsFromPastDays(numberOfDays, out startDate);

            query = query.Where(x => sensorIds.Any(y => y == x.SensorId));

            var result = await GetHighestValues(query, startDate);

            return result;
        }

        //Returns Daily Reports from sensors within range of center point
        public async Task<IEnumerable<RecordDto>> ReportWithinDistance(
            double lat, double lng, double distance, int numberOfDays)
        {
            DateTime startDate;

            var query = GetRecordsFromPastDays(numberOfDays, out startDate);

            var dc = new DistanceCalculator(lat, lng, distance);

            query = query.Where(x => 
                    x.Latitude * Math.Pow(10, -6) > dc.LatMin && x.Latitude * Math.Pow(10, -6) < dc.LatMax &&
                    x.Longitude * Math.Pow(10, -6) > dc.LngMin && x.Longitude * Math.Pow(10, -6) < dc.LngMax);

            var result = await GetHighestValues(query, startDate);

            return result;
        }

        //Populates records for the past N days, every 15 minutes
        public async Task PopulateRecords(ICollection<SensorDto> sensors,
            int numberOfDays)
        {
            foreach (var sensor in sensors)
            {
                var endDateTime = DateTime.UtcNow;
                var currentTime = endDateTime.Date.AddDays(-numberOfDays);

                while (currentTime < endDateTime)
                {
                    var properties = new RecordDto
                    {
                        Latitude = sensor.Latitude,
                        Longitude = sensor.Longitude,
                        Time = currentTime,
                        SensorId = sensor.Id
                    };

                    await CreateRecordAsync(sensor.Id, properties);

                    await _context.SaveChangesAsync();

                    currentTime = currentTime.AddMinutes(sensor.RecordTimer);
                }
            }
        }

        //Returns all records in database. For testing purposes
        public async Task<ICollection<RecordDto>> GetAllRecords()
        {
            return await _context.Records.ProjectTo<RecordDto>
                (_mapper.ConfigurationProvider).ToListAsync();
        }

        private IQueryable<Record> GetRecordsFromPastDays(int numberOfDays, out DateTime startDate)
        {
            startDate = DateTime.UtcNow.Date.AddDays(-numberOfDays);
            var startTimeStamp = new DateTimeOffset(startDate).ToUnixTimeSeconds();;

            if (_context == null)
            {
                throw new NullReferenceException("Database not initialized");
            }

            if (_context.Records == null)
            {
                throw new NullReferenceException($"Records dataset not initialized");
            }

            var query = _context.Records.Where(x => x.TimeStamp > startTimeStamp);

            return query;
        }

        private async Task<IEnumerable<RecordDto>> GetHighestValues(IQueryable<Record> records, DateTime startDate)
        {
            var query = await records.ProjectTo<RecordDto>(_mapper.ConfigurationProvider).ToListAsync();

            var dates = HelperMethods.GetDateTimeList(startDate);
            
            var result = dates.GroupJoin(query, x => x, y => y.Time.Date, (x, y) => new
                        {
                            date = x,
                            value = y
                        })
                        .OrderBy(x => x.date)
                        .Select(x => x.value.MaxBy(y => y.Illumination))
                        .Where(x => x != null);

            return result!;
        }
    }
}