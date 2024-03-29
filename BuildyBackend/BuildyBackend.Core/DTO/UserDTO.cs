namespace BuildyBackend.Core.DTO
{
    /// <summary>
    /// Esta clase sirve de apoyo al IdentityUser
    /// El UserID nunca va en los DTOs porque sería una vulnerabilidad del sistema
    /// En el Context la tabla es .Users
    /// </summary>
    public class UserDTO
    {
        public string? Email { get; set; }

    }
}