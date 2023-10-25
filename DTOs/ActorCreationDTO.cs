using ApiPeliculas.Validations;

namespace ApiPeliculas.DTOs
{
    public class ActorCreationDTO : ActorPatchDTO
    {
        //[Required]
        //[StringLength(120)]
        //public string Name { get; set;}
        //public DateTime DateOfBird { get; set;}

        [FileSizeValidation(maxLengthMegaBytes:  4)]
        [FileTypeValidation(groupFileType: GroupFileType.Image)]
        public IFormFile photo { get; set;}   // to store images
    }
}
