using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace PROKSRent.Models
{
    public class Car
    {
        public int Id { get; set; }
        [Required]
        public int BrandId { get; set; }
        public Brand? Brand { get; set; }
        [Required]
        public string Model { get; set; }
        [Required]
        public int Year { get; set; }
        [Required]
        public string CarNumber { get; set; }
        [Required]
        public int FuelTypeId { get; set; }
        public FuelType? FuelType { get; set; }
        [Required]
        public int TransmissionTypeId { get; set; }
        public TransmissionType? TransmissionType { get; set; }
        [Required]
        public double EngineCapacity { get; set; }
        [Required]
        public int Seats { get; set; }

        public string? ImagePath { get; set; }
        [NotMapped]
        public IFormFile? CarImage { get; set; }
        public bool IsAvailable { get; set; } = true;

        public ICollection<Booking> Bookings { get; set; } = new List<Booking>();


        
        // Location
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
    }
}
