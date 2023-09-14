using System.ComponentModel.DataAnnotations;

namespace API.Data.Entities
{
    public class Sensor
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public string? ClientId { get; set; }
        [Required]
        public double Latitude { get; set; }
        [Required]
        public double Longitude { get; set; }
        [Required]
        public int RecordTimer { get; set; }
        [Required]
        public int TotalRecords { get; set; }
        public ICollection<Record> Records { get; set; } = new List<Record>();
    }
}