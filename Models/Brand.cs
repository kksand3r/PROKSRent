using System.ComponentModel.DataAnnotations;

namespace PROKSRent.Models
{
    public class Brand
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }

        public ICollection<Car>? Cars { get; set; }
    }
}
