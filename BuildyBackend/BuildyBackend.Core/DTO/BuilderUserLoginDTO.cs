using System.ComponentModel.DataAnnotations;

namespace BuildyBackend.Core.DTO;
public class BuilderUserLoginDTO
{
    [Required]
    public string Username { get; set; }

    [Required]
    public string Password { get; set; }

}
