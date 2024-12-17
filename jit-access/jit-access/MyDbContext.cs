namespace jit_access
{
    using Microsoft.EntityFrameworkCore;
    public class MyDbContext : DbContext
    {
        public MyDbContext(DbContextOptions<MyDbContext> options)
            : base(options)
        {
        }

        public DbSet<AccessRequest> AccessRequests { get; set; }
    }
}
