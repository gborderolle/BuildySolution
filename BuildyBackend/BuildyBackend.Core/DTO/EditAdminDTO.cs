using System.ComponentModel.DataAnnotations;

namespace BuildyBackend.Core.DTO
{
    public class EditAdminDTO
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}