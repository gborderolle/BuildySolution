using BuildyBackend.Core.Domain.Entities;

namespace BuildyBackend.Core.DTO
{
    public class ReportDTO
    {
        #region Internal

        public int Id { get; set; }

        public string Name { get; set; }

        public string Month { get; set; }

        public DateTime Creation { get; set; } = DateTime.Now;

        public DateTime Update { get; set; } = DateTime.Now;

        public string? Comments { get; set; }

        // Uniques
        public List<string> ListPhotosURL { get; set; }

        #endregion

        #region External

        public int EstateId { get; set; }
        public Estate Estate { get; set; }

        #endregion

    }
}
