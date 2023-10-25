using System.ComponentModel.DataAnnotations;

namespace ApiPeliculas.DTOs
{
    public class ActorPatchDTO
    {
        [Required]
        [StringLength(120)]
        public string Name { get; set; }
        public DateTime DateOfBird { get; set; }
    }
}
