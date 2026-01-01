using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace HealthReport.Identity.Application
{
    public class RolesListHandler(RoleManager<IdentityRole<Guid>> roleManager) : IRolesListHandler
    {
        private readonly RoleManager<IdentityRole<Guid>> _roleManager = roleManager ?? throw new ArgumentNullException(nameof(roleManager));

        public async Task<ICollection<string>> ListAsync(CancellationToken cancellationToken = default)
        {
            var roles = await _roleManager.Roles
                .OrderBy(r => r.Name)
                .Select(r => r.Name ?? string.Empty)
                .ToArrayAsync(cancellationToken);

            return roles;
        }
    }
}
