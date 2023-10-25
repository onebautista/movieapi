using System.ComponentModel.DataAnnotations;

namespace ApiPeliculas.Validations
{
    public class FileSizeValidation : ValidationAttribute
    {
        private readonly int maxLengthMegaBytes;

        public FileSizeValidation(int maxLengthMegaBytes)
        {
            this.maxLengthMegaBytes = maxLengthMegaBytes;
        }

        // override validation method
        protected override ValidationResult IsValid (object value, ValidationContext validationContext)
        {
            if (value == null) { return ValidationResult.Success; }
            
            //valor del form
            IFormFile formFile = value as IFormFile;
            if (formFile == null) { return ValidationResult.Success;  }

            if(formFile.Length > maxLengthMegaBytes * 1024 * 1024 ) // mb * bytes
            {
                return new ValidationResult($"El peso del archivo no debe ser mayor a {maxLengthMegaBytes} mb");
            }

            return ValidationResult.Success;
        }
    }
}
