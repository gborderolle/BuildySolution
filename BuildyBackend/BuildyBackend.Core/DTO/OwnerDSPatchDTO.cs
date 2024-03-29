using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

using BuildyBackend.Core.Domain.Entities;

namespace BuildyBackend.Core.DTO
{
    public class OwnerPatchDTO
    {
        #region Internal

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required(ErrorMessage = "El campo {0} es requerido")]
        [StringLength(maximumLength: 100, ErrorMessage = "El campo {0} no puede tener más de {1} caracteres")]
        public string Name { get; set; }

        public DateTime Creation { get; set; } = DateTime.Now;

        public DateTime Update { get; set; } = DateTime.Now;

        #endregion

        #region External

        #endregion

    }
}
