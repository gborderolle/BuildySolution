namespace BuildyBackend.Core.DTO
{
    public class TenantDTO
    {
        #region Internal

        public int Id { get; set; }

        public string Name { get; set; }

        public DateTime Creation { get; set; } = DateTime.Now;

        public DateTime Update { get; set; } = DateTime.Now;

        public string Phone1 { get; set; }

        public string Phone2 { get; set; }

        public string Email { get; set; }

        public string IdentityDocument { get; set; }

        public string Comments { get; set; }

        #endregion

        #region External

        public int? RentId { get; set; } // n..1 (1=sí existe este sin el padre)

        #endregion
    }
}
