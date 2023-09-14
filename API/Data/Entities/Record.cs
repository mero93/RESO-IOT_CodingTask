using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace API.Data.Entities
{
    public class Record
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public int Weather { get; set; }
        [Required]
        public double Temperature { get; set; }
        [Required]
        public double Illumination { get; set; }
        [Required]
        public long TimeStamp { get; set; }
        public long WeatherStamp { get; set; }
        [Required]
        public double Latitude { get; set; }
        [Required]
        public double Longitude { get; set; }
        
        [Required]
        public int SensorId  { get; set; }
        public Sensor? Sensor { get; set; }
    }
}