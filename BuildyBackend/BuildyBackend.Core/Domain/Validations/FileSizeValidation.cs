using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace DatalexionBackend.Core.Validations
{
    public class FileSizeValidation : ValidationAttribute
    {
        private readonly int _maxSizeMB;

        public FileSizeValidation(int maxSizeMB)
        {
            _maxSizeMB = maxSizeMB;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null)
            {
                return ValidationResult.Success;
            }

            IFormFile formFile = value as IFormFile;
            if (formFile == null)
            {
                return ValidationResult.Success;
            }

            if (formFile.Length > _maxSizeMB * 1024 * 1024)
            {
                return new ValidationResult($"El peso del archivo no debe ser mayor a {_maxSizeMB}mb.");
            }
            return ValidationResult.Success;
        }

    }
}
