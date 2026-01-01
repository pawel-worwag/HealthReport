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
        public async Task<ICollection<string>> ListAsync(CancellationToken cancellationToken = default)
        {
            return await roleManager.Roles
                .OrderBy(r => r.Name)
                .Select(r => r.Name ?? string.Empty)
                .ToArrayAsync(cancellationToken);
        }
    }
}
