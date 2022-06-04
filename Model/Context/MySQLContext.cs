using Microsoft.EntityFrameworkCore;

namespace Invoice.API.Model.Context
{
    public class MySQLContext : DbContext
    {
        public MySQLContext() {}
        public MySQLContext(DbContextOptions<MySQLContext> options) : base(options) {}

        public DbSet<InvoiceEntity> Invoice { get; set; }
    }
}
