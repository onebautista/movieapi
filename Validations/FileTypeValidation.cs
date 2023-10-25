
using System.ComponentModel.DataAnnotations;

namespace ApiPeliculas.Validations
{
    public class FileTypeValidation: ValidationAttribute
    {
        private readonly string[] validTypes;

        public FileTypeValidation(string[] validTypes)
        {
            this.validTypes = validTypes;
        }


        public FileTypeValidation(GroupFileType groupFileType)  
        {
            if( groupFileType == GroupFileType.Image)
            {
                validTypes = new string[] { "image/jpeg", "image/png", "image/gif" };
            }
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null) { return ValidationResult.Success; }

            IFormFile formFile = value as IFormFile;
            if (formFile == null) { return ValidationResult.Success; }

            if(!validTypes.Contains(formFile.ContentType))
            {
                return new ValidationResult($"El tipo de archivo debe ser: {string.Join(", ", validTypes)}");
            }
            

            return ValidationResult.Success;
        }
    }
}
