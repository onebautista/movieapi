using ApiPeliculas.Validations;
using System.ComponentModel.DataAnnotations;

namespace ApiPeliculas.DTOs
{
    public class PeliculaCreacionDTO
    {

        public int Id { get; set; }
        [Required]
        [StringLength(300)]
        public string Titulo { get; set; }
        public bool EnCines { get; set; }
        public DateTime FechaEstreno { get; set; }

        [FileSizeValidation(maxLengthMegaBytes: 4)]
        [FileTypeValidation(groupFileType: GroupFileType.Image)]
        public IFormFile Poster { get; set; }

        public List<int> GenerosIDs { get; set; }
    }
}
