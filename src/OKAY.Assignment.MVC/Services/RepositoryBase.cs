using OKAY.Assignment.MVC.Data;
using System;
using Microsoft.AspNetCore.Http;
using OKAY.Assignment.MVC.Entities;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using OKAY.Assignment.MVC.Constraints;

namespace OKAY.Assignment.MVC.Services
{
    public abstract class RepositoryBase: IDisposable
    {
        internal readonly ApplicationDbContext _context;
        internal readonly ClaimsPrincipal user;
        public RepositoryBase(ApplicationDbContext context, IHttpContextAccessor accessor)
        {
            _context = context;
            user = accessor.HttpContext.User;
        }

        internal bool isAdminUser { get => user.IsInRole(IdentityRolesNames.Administrator); }
        internal Guid userId { 
            get
            {
                var id = user.FindFirst(x => x.Type == Constraints.ClaimTypes.NameIdentifier)?.Value;
                return string.IsNullOrEmpty(id) ? Guid.Empty : Guid.Parse(id);
            }
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
