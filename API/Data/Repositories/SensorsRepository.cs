using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Reflection.Metadata.Ecma335;
using System.Threading.Tasks;
using API.Data.DTOs;
using API.Data.Entities;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Data.Repositories
{
    public class SensorsRepository: ISensorsRepository
    {
        private readonly IMapper _mapper;
        private readonly DataContext _context;

        public SensorsRepository(IMapper mapper, DataContext context)
        {
            _context = context;
            _mapper = mapper;
        }

        //Returns sensors for current user(client)
        public async Task<ICollection<SensorDto>> GetUserSensorsAsync(string clientId)
        {
            if (_context == null)
            {
                throw new NullReferenceException("Database not initialized");
            }

            if (_context.Sensors == null)
            {
                return new List<SensorDto>();
            }

            var query = await _context.Sensors.Where(x => x.ClientId == clientId)
                .ProjectTo<SensorDto>(_mapper.ConfigurationProvider)
                .OrderByDescending(x => x.Id).ToListAsync();

            return query;
        }

        //Creates and adds to database new sensor
        public async Task<(SensorDto, bool)> CreateSensorAsync(SensorDto properties)
        {
            var entity = _mapper.Map<Sensor>(properties);

            await _context.AddAsync(entity);

            var result = await _context.SaveChangesAsync() > 0;

            return (_mapper.Map<SensorDto>(entity), result);
        }

        //Updates sensors properties
        public async Task UpdateSensorAsync(SensorDto properties)
        {
            if (_context == null)
            {
                throw new NullReferenceException("Database not initialized");
            }

            if (_context.Sensors == null)
            {
                throw new NullReferenceException($"Sensors dataset not initialized");
            }

            var result = await _context.Sensors.SingleAsync(x => x.Id == properties.Id);

            if (result == null)
            {
                throw new NullReferenceException($"Sensor with id {properties.Id} couldn't be found");
            }

            result = _mapper.Map(properties, result);

            _context.Update(result);
        }

        public async Task RemoveSensorAsync(int id)
        {
            if (_context == null)
            {
                throw new NullReferenceException("Database not initialized");
            }

            if (_context.Sensors == null)
            {
                throw new NullReferenceException($"Sensors dataset not initialized");
            }

            var result = await _context.Sensors.SingleAsync(x => x.Id == id);

            if (result == null)
            {
                throw new NullReferenceException($"Sensor with id {id} couldn't be found");
            }

            _context.Remove(result);
        }

        public async Task<(ICollection<SensorDto>, bool)> PopulateSensors(double lat, double lng,
            double distance, int numberOfSensors, string ClientId,
            int recordTimer, int totalRecords)
        {
            var dc = new DistanceCalculator(lat, lng, distance);

            var rand = new Random();

            var sensors = new List<Sensor>();

            for (int i = 0; i < numberOfSensors; i++)
            {
                var lngRand = rand.NextDouble() * 2 * dc.LngDiff + dc.LngMin;

                var sensor = new Sensor
                {
                    ClientId = ClientId,
                    Latitude = (rand.NextDouble() * 2 * dc.LatDiff + dc.LatMin) * Math.Pow(10,6),
                    Longitude = lngRand > 180? (-360 + lngRand) * Math.Pow(10,6) :
                                lngRand < -180? (360 + lngRand) * Math.Pow(10,6) :
                                lngRand * Math.Pow(10,6),
                    RecordTimer = recordTimer,
                    TotalRecords = totalRecords
                };

                sensors.Add(sensor);
            }

            await _context.AddRangeAsync(sensors);

            var result = await _context.SaveChangesAsync() > 0;

            return (_mapper.Map<List<SensorDto>>(sensors).OrderByDescending(x => x.Id).ToList(), result);
        }

        public async Task<ICollection<SensorDto>> GetAllSensors()
        {
            return await _context.Sensors.ProjectTo<SensorDto>(_mapper.ConfigurationProvider).ToListAsync();
        }

        public async Task<ICollection<SensorDto>> GetLatestSensors(int numberOfSensors, string clientId)
        {
            if (_context == null)
            {
                throw new NullReferenceException("Database not initialized");
            }

            if (_context.Sensors == null)
            {
                throw new NullReferenceException($"Sensors dataset not initialized");
            }

            var query = await _context.Sensors.Where(x => x.ClientId == clientId)
                .OrderByDescending(x => x.Id).Take(numberOfSensors).ToListAsync();

            return  _mapper.Map<List<SensorDto>>(query);
        }
    }
}