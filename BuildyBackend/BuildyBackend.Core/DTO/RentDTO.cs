using BuildyBackend.Core.Domain.Entities;

namespace BuildyBackend.Core.DTO
{
    public class RentDTO
    {
        #region Internal

        public int Id { get; set; }

        public DateTime Creation { get; set; } = DateTime.Now;

        public DateTime Update { get; set; } = DateTime.Now;

        public string Comments { get; set; }

        // Uniques

        public string Warrant { get; set; }

        public decimal? MonthlyValue { get; set; }

        public DateTime? Datetime_monthInit { get; set; }

        public string Duration { get; set; }

        public bool RentIsEnded { get; set; }

        public List<string> ListFilesURL { get; set; }

        #endregion

        #region External

        /// <summary>
        /// No uso Entidad para no generar dependencia circular
        /// </summary>
        public int EstateId { get; set; }
        public Estate Estate { get; set; }

        public List<Tenant> ListTenants { get; set; } = new();

        public int PrimaryTenantId { get; set; }

        #endregion
    }
}
