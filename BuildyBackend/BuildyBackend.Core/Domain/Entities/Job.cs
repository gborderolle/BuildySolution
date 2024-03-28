using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace BuildyBackend.Core.Domain.Entities
{
    public class Job : IId
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

        public string? Comments { get; set; }

        // Uniques

        public decimal? LabourCost { get; set; }

        /// <summary>
        /// 1-N
        /// </summary>
        public List<Photo> ListPhotos { get; set; }

        #endregion

        #region External

        [Required(ErrorMessage = "El campo {0} es requerido")] // n..0 (0=no existe este sin el padre)
        public int EstateId { get; set; }
        public Estate Estate { get; set; }

        public List<Worker> ListWorkers { get; set; } = new();

        #endregion
    }
}
