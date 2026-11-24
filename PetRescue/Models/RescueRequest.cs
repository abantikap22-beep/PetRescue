
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PetRescue.Models
{
    public class RescueRequest
    {
        [Key]
        public int RescueRequestId { get; set; }

        [Required]
        [Display(Name = "Name")]
        public string ReporterName { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Contact Number")]
        public string ContactNumber { get; set; } = string.Empty;

      
        [Required]
        [Display(Name = "Pet Type")]
        public int PetTypeId { get; set; }

        [ForeignKey("PetTypeId")]
        public PetType? PetType { get; set; }

        [Required]
        public string Description { get; set; } = string.Empty;

        [Required]
        public string Location { get; set; } = string.Empty;

        public string? ImageUrl { get; set; }

      
        public string Status { get; set; } = "Pending";

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
