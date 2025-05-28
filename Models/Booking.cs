using System.ComponentModel.DataAnnotations;

namespace PROKSRent.Models
{
    public class Booking
    {
        public int Id { get; set; }
        [Required]
        public DateTime StartDate { get; set; }
        [Required]
        public DateTime EndDate { get; set; }
        public bool IsConfirmed { get; set; }
        [Required]
        public int CarId { get; set; }
        public Car? Car { get; set; }
        [Required]
        public string UserId { get; set; }
        public ApplicationUser? User { get; set; }
    }
}
