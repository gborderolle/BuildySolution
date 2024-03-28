using BuildyBackend.Core.Domain.Entities;

using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace BuildyBackend.Core.DTO
{
    public class WorkerDTO
    {
        #region Internal

        public int Id { get; set; }

        public string Name { get; set; }

        public DateTime Creation { get; set; } = DateTime.Now;

        public DateTime Update { get; set; } = DateTime.Now;

        public string Phone { get; set; }

        public string Email { get; set; }

        public string IdentityDocument { get; set; }

        public string? Comments { get; set; }

        #endregion

        #region External

        public int? JobId { get; set; } // n..1 (1=sí existe este sin el padre)

        #endregion
    }
}
