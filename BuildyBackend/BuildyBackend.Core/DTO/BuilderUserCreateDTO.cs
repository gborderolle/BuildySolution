namespace BuildyBackend.Core.DTO;
public class BuilderUserCreateDTO
{
    public string Username { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public string UserRoleId { get; set; }
    public string UserRoleName { get; set; }
}