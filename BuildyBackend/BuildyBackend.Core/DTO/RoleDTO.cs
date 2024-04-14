namespace BuildyBackend.Core.DTO
{
    public class RoleDTO
    {
        #region Internal

        public string Id { get; set; }
        public required string Name { get; set; }
        public DateTime Creation { get; set; } = DateTime.Now;
        public DateTime Update { get; set; } = DateTime.Now;

        #endregion

    }
}