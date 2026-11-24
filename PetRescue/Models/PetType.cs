
using System.ComponentModel.DataAnnotations;

namespace PetRescue.Models
{
    public class PetType
    {
        [Key]
        public int PetTypeId { get; set; }

        [Required]
        [Display(Name = "Pet Type")]
        public string PetTypeName { get; set; }
        public ICollection<Pet> Pets { get; set; } = new List<Pet>();
        public ICollection<RescueRequest> RescueRequests { get; set; } = new List<RescueRequest>();

    }
}
