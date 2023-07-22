using MicroondasApp.Models;
using Microsoft.EntityFrameworkCore;

namespace MicroondasApp.Persistence
{
    public class MicroOndasDb : DbContext
    {
        public MicroOndasDb(DbContextOptions<MicroOndasDb> options)
        : base(options) { }

        public DbSet<MicroOndas> MicroOndas => Set<MicroOndas>();
    }
}
