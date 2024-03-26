using BuildyBackend.Core.Domain.Entities;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using DatalexionBackend.Core.Validations;
using Microsoft.AspNetCore.Http;


namespace BuildyBackend.Core.DTO
{
    public class RentCreateDTO
    {
        #region Internal

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public DateTime Creation { get; set; } = DateTime.Now;

        public DateTime Update { get; set; } = DateTime.Now;

        public string Comments { get; set; }

        // Uniques

        public string Warrant { get; set; }

        public decimal? MonthlyValue { get; set; }

        public DateTime? Datetime_monthInit { get; set; }

        public string Duration { get; set; }

        public bool RentIsEnded { get; set; }

        [FileSizeValidation(maxSizeMB: 4)]
        public List<IFormFile> ListFiles { get; set; }

        #endregion

        #region External

        /// <summary>
        /// No uso Entidad para no generar dependencia circular
        /// </summary>
        [Required(ErrorMessage = "El campo {0} es requerido")] // n..0 (0=no existe este sin el padre)
        public int EstateId { get; set; }
        public Estate Estate { get; set; }

        // public List<TenantCreateDTO> ListTenants { get; set; } = new();
        public List<int> TenantIds { get; set; } = new List<int>();  // Cambio aquí

        public int PrimaryTenantId { get; set; }

        #endregion
    }
}
