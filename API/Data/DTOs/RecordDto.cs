using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Data.DTOs
{
    public class RecordDto
    {
        public int Id { get; set; }
        public string? Weather { get; set; }
        public double Temperature { get; set; }
        public double Illumination { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public DateTime Time { get; set; }
        public int SensorId  { get; set; }
    }
}