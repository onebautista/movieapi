using System.ComponentModel.DataAnnotations;

namespace ApiPeliculas.DTOs
{
    public class ActorDTO
    {

        public int Id { get; set; }
        [Required]
        [StringLength(120)]
        public string Name { get; set; }
        public DateTime dateOfBirth { get; set; }
        public string photo { get; set; }

    }
}
