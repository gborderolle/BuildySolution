namespace BuildyBackend.Core.Domain.Entities
{
    /// <summary>
    /// Heredar todas las clases que tengan Id
    /// </summary>
    public interface IId
    {
        public int Id { get; set; }
    }
}