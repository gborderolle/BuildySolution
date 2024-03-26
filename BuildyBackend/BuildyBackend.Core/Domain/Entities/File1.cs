using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace BuildyBackend.Core.Domain.Entities
{
    public class File1
    {
        #region Internal

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public DateTime Creation { get; set; } = DateTime.Now;

        public DateTime Update { get; set; } = DateTime.Now;

        // Uniques
        [Required(ErrorMessage = "El campo {0} es requerido")]
        public string URL { get; set; }

        #endregion

        #region External

        public int? RentId { get; set; }
        public Rent? Rent { get; set; }

        #endregion
    }
}
