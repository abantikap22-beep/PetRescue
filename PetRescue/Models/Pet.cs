using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PetRescue.Models
{
    public class Pet
    {
        [Key]
        public int PetId { get; set; }

        public int? PetTypeId { get; set; }

        [ForeignKey("PetTypeId")]
        public PetType? PetType { get; set; }

        public string? Breed { get; set; }

        public int? Age { get; set; }

        public string? Gender { get; set; }

        public string? Description { get; set; }

        public string? ImageUrl { get; set; }

        [Required]
        public string? Location { get; set; }

        [Required]
        [Display(Name = "Available")]
        public string Status { get; set; } = "Available";

        public ICollection<AdoptionRequest> AdoptionRequests { get; set; } = new List<AdoptionRequest>();
    }
}