using Microsoft.AspNetCore.Identity;

namespace BuildyBackend.Core.Domain.IdentityEntities
{
    public class BuildyRole : IdentityRole
    {
        public DateTime Creation { get; set; } = DateTime.Now;

        public DateTime Update { get; set; } = DateTime.Now;

    }
}
