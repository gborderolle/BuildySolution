using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace DatalexionBackend.Core.Validations
{
    public class FileTypeValidation : ValidationAttribute
    {
        private readonly string[] _typeList;

        public FileTypeValidation(string[] typeList)
        {
            _typeList = typeList;
        }

        public FileTypeValidation(FileTypeGroup fileTypeGroup)
        {
            if (fileTypeGroup == FileTypeGroup.Image)
            {
                _typeList = new string[] { "image/jpeg", "image/png", "image/gif" };
            }
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

            if (!_typeList.Contains(formFile.ContentType))
            {
                return new ValidationResult($"El tipo del archivo debe ser uno de los siguientes {string.Join(",", _typeList)}.");
            }
            return ValidationResult.Success;
        }

    }
}