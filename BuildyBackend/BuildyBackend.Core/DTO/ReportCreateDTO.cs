using BuildyBackend.Core.Domain.Entities;

using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using DatalexionBackend.Core.Validations;
using Microsoft.AspNetCore.Http;

namespace BuildyBackend.Core.DTO
{
    public class ReportCreateDTO
    {
        #region Internal

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required(ErrorMessage = "El campo {0} es requerido")]
        [StringLength(maximumLength: 100, ErrorMessage = "El campo {0} no puede tener más de {1} caracteres")]
        public string Name { get; set; }

        public string Month { get; set; }

        public DateTime Creation { get; set; } = DateTime.Now;

        public DateTime Update { get; set; } = DateTime.Now;

        public string Comments { get; set; }

        // Uniques

        //public List<Photo> ListPhotos { get; set; }
        [FileSizeValidation(maxSizeMB: 4)]
        [FileTypeValidation(fileTypeGroup: FileTypeGroup.Image)]
        public List<IFormFile> ListPhotos { get; set; } // Clase: https://www.udemy.com/course/construyendo-web-apis-restful-con-aspnet-core/learn/lecture/19983788#notes

        #endregion

        #region External

        [Required(ErrorMessage = "El campo {0} es requerido")] // n..0 (0=no existe este sin el padre)
        public int EstateId { get; set; }
        public Estate Estate { get; set; }

        #endregion

    }
}
