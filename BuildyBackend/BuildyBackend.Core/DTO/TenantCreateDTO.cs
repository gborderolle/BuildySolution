
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace BuildyBackend.Core.DTO
{
    public class TenantCreateDTO
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

        [Required(ErrorMessage = "El campo {0} es requerido")]
        public string Phone1 { get; set; }

        public string? Phone2 { get; set; }

        [EmailAddress]
        public string? Email { get; set; }

        public string? IdentityDocument { get; set; }

        public string? Comments { get; set; }

        #endregion

        #region External

        public int? RentId { get; set; } // n..1 (1=sí existe este sin el padre)

        #endregion

    }
}
