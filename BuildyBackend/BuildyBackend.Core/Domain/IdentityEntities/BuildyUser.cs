using Microsoft.AspNetCore.Identity;

namespace BuildyBackend.Core.Domain.IdentityEntities
{
    public class BuildyUser : IdentityUser
    {
        public required string Name { get; set; }

        public DateTime Creation { get; set; } = DateTime.Now;

        public DateTime Update { get; set; } = DateTime.Now;

    }
}