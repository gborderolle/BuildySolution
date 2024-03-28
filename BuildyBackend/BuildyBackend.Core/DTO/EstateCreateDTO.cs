using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace BuildyBackend.Core.DTO
{
    public class EstateCreateDTO
    {
        #region Internal

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required(ErrorMessage = "El campo {0} es requerido")]
        [StringLength(maximumLength: 100, ErrorMessage = "El campo {0} no puede tener más de {1} caracteres")]
        public string Name { get; set; }

        [Required(ErrorMessage = "El campo {0} es requerido")]
        public string Address { get; set; }
        public string LatLong { get; set; }
        [Required(ErrorMessage = "El campo {0} es requerido")]
        public int CityDSId { get; set; }
        [Required(ErrorMessage = "El campo {0} es requerido")]
        public int OwnerDSId { get; set; }
        public string GoogleMapsURL { get; set; }
        public bool EstateIsRented { get; set; }
        public string? Comments { get; set; }
        public int? PresentRentId { get; set; }

        #endregion

        #region External

        #endregion
    }
}
