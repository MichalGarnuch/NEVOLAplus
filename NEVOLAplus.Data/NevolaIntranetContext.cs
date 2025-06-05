using Microsoft.EntityFrameworkCore;
using NEVOLAplus.Data.Models.CMS;
using NEVOLAplus.Data.Models.HR;
using NEVOLAplus.Data.Models.Inventory;
using NEVOLAplus.Data.Models.Licensing;
using NEVOLAplus.Data.Models.Reservation;

namespace NEVOLAplus.Data
{
    public class NevolaIntranetContext : DbContext
    {
        public NevolaIntranetContext(DbContextOptions<NevolaIntranetContext> options)
            : base(options)
        {
        }
        public NevolaIntranetContext()
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                //connection string
                optionsBuilder.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=NEVOLAplusIntranetDB;Trusted_Connection=True;MultipleActiveResultSets=true");
            }
        }

        // CMS
        public DbSet<Page> Pages { get; set; } = null!;
        public DbSet<News> News { get; set; } = null!;
        public DbSet<TextSnippet> TextSnippets { get; set; } = null!;


        // HR
        public DbSet<Employee> Employees { get; set; } = null!;
        public DbSet<Position> Positions { get; set; } = null!;

        // Inventory
        public DbSet<Asset> Assets { get; set; } = null!;
        public DbSet<AssetType> AssetTypes { get; set; } = null!;

        // Reservation
        public DbSet<Reservation> Reservations { get; set; } = null!;

        // Licensing
        public DbSet<SoftwareLicense> SoftwareLicenses { get; set; } = null!;
    }
}
