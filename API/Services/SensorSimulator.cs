using API.Data;
using API.Data.DTOs;
using API.Data.Entities;
using API.Helpers;
using API.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace API.Services
{
    public class SensorSimulator: ISensorSimulator
    {
        private readonly Random _rand;
        private readonly DataContext _context;
        public SensorSimulator(DataContext context)
        {
            _context = context;
            _rand = new Random();
        }

        //Simulates illumination and temperature that would be reported from actual sensor.
        //Weather is major factor on illumination. To closely simulate weather effect on sensors close to
        //each other and for weather not to fluctuate in short timeframe, records are dependant on previous
        //records within vicinity and 3 hours timeframe of weather report from previous record.
        //Reference record is chosen by weighting in closeness to recording sensor and recentness of
        //weather report.
        public async Task<Record> Simulate(int id, RecordDto properties)
        {
            //Search radius of 20 km
            const double searchDist = 20;
            //Search timespan of 3 hours
            const double searchTime = 10800;

            var dc = new DistanceCalculator(properties.Latitude, properties.Longitude, searchDist);

            var rand = new Random();

            //New record timestamp
            var timeStamp = new DateTimeOffset(properties.Time).ToUnixTimeSeconds();

            long weatherStamp;

            int weather;

            if (_context.Records != null)
            {
                var query = _context.Records
                    .Where(x => (timeStamp > x.WeatherStamp) && ((timeStamp - x.WeatherStamp) < searchTime))
                    .Where(x => 
                    x.Latitude * Math.Pow(10, -6) > dc.LatMin && x.Latitude * Math.Pow(10, -6) < dc.LatMax &&
                    x.Longitude * Math.Pow(10, -6) > dc.LngMin && x.Longitude * Math.Pow(10, -6) < dc.LngMax);

                var length = await query.CountAsync();

                if (length > 0)
                {
                    var referencePoint = await query.Select(x => 
                        new
                        {
                            value = x,
                            vicinity = 1 - (Math.Sqrt(
                                (Math.Pow((dc.Lat - (x.Latitude * Math.Pow(10, -6))), 2) 
                                + Math.Pow((dc.Lng - (x.Longitude * Math.Pow(10, -6))), 2))
                                )/searchDist),
                            recentness = (1 - (Math.Abs(x.WeatherStamp - timeStamp)/searchTime)),
                        })
                        .OrderByDescending(x => (x.vicinity + x.recentness))
                        .Select(x => x.value).FirstAsync();

                    if (referencePoint.WeatherStamp - timeStamp < 7200)
                    {
                        weatherStamp = referencePoint.WeatherStamp;
                        weather = referencePoint.Weather;
                    }

                    else
                    {
                        var roll = rand.NextDouble();
                        weatherStamp = timeStamp;

                        switch (referencePoint.Weather)
                        {
                            case 0:
                                if (roll < 0.2) weather = 1;
                                else if (roll > 0.9) weather = 2;
                                else weather = 0;
                                break;
                            case 1:
                                if (roll < 0.15) weather = 0;
                                else if (roll > 0.85) weather = 2;
                                else weather = 1;
                                break;
                            default:
                                if (roll < 0.2) weather = 1;
                                else if (roll > 0.9) weather = 0;
                                else weather = 2;
                                break;
                            }
                        }
                    }
                else
                {
                    weatherStamp = timeStamp;
                    weather = rand.Next(3);
                }
            }

            else
            {
                weatherStamp = timeStamp;
                weather = rand.Next(3);
            }

            double illumination;
            double temperature;
            CalculateIlluminationTemperature(rand, weather, properties.Time,
                out illumination, out temperature);

            var result = new Record
            {
                Latitude = properties.Latitude * Math.Pow(10, 6),
                Longitude = properties.Longitude * Math.Pow(10, 6),
                SensorId = id,
                TimeStamp = timeStamp,
                WeatherStamp = weatherStamp,
                Weather = weather,
                Illumination = illumination,
                Temperature = temperature,
            };

            return result;
        }

        private void CalculateIlluminationTemperature(Random rand, int weather, 
                DateTime dateTime, out double illumination, out double temperature)
        {
            //Nominal Sunrise/Sundown base illumination value
            var nominalBaseLux = 400.0;

            //Natural base with fluctuations of general factors
            var naturalBaseLux = (nominalBaseLux * 0.9) + (nominalBaseLux * 0.2 * rand.NextDouble());

            //Nominal 3 a.m. base temperature value
            var nominalBaseTemp = 18.0;

            //Nominal 3 a.m. base temperature value
            var naturalBaseTemp = (nominalBaseTemp * 0.9) + (nominalBaseTemp * 0.2 * rand.NextDouble());

            //Decimal value of hours of time of the day
            var hours = dateTime.TimeOfDay.TotalHours;

            //Weather impact coefficient on illumination
            double weatherCoeff;

            //Weather impact coefficient on temperature
            double tempCoeff;
            
            switch (weather)
            {
                case 0:
                    weatherCoeff = 0.025;
                    tempCoeff = 0.7;
                    break;
                case 1:
                    weatherCoeff = 0.1;
                    tempCoeff = 0.9;
                    break;
                default:
                    weatherCoeff = 1;
                    tempCoeff = 1;
                    break;
            }

            //Seasonal coefficient affecting illumination
            double seasonCoeff;

            //Seasonal average temperature difference
            double seasonTemp;

            switch (dateTime.Month)
            {
                case 12: case 1: case 2:
                    seasonCoeff = 0.7;
                    seasonTemp = -3;
                    break;
                case 3: case 4: case 5:
                    seasonCoeff = 1;
                    seasonTemp = 15;
                    break;
                case 6: case 7: case 8:
                    seasonCoeff = 1.3;
                    seasonTemp = 20;
                    break;
                default:
                    seasonCoeff = 0.9;
                    seasonTemp = 7;
                    break;
            }

            illumination = naturalBaseLux * weatherCoeff * seasonCoeff
                *  Math.Exp(5.3 * Math.Sin(((hours * 15) - 90) * Math.PI / 180));

            illumination = Math.Round(illumination * 2, MidpointRounding.AwayFromZero) / 2;

            temperature = seasonTemp + (Math.Abs(Math.Sin((hours * 7.5 - 22.5) * Math.PI / 180))
                * naturalBaseTemp * tempCoeff);

            temperature = Math.Round(temperature * 10, MidpointRounding.AwayFromZero) / 10;
        }
    }
}