using Hangfire;
using Hangfire.Common;
using Microsoft.EntityFrameworkCore;

namespace jit_access
{
    public class PermissionRevocationJob : IJob
    {
        private readonly MyDbContext _context;

        public PermissionRevocationJob(MyDbContext context)
        {
            _context = context;
        }

        public void Execute()
        {
            // Get expired access requests
            var expiredRequests = _context.AccessRequests
                .Where(r => r.ExpirationTime < DateTime.UtcNow)
                .ToList();

            foreach (var request in expiredRequests)
            {
                // Revoke permissions (e.g., using T-SQL)
                // ...

                _context.AccessRequests.Remove(request);
            }

            _context.SaveChanges();
        }
    }
}

