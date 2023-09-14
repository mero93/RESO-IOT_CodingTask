using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Data.DTOs
{
    public class SensorDto
    {
        public int Id { get; set; }
        public string? ClientId { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public int RecordTimer { get; set; }
        public int TotalRecords { get; set; }
    }
}