using Microsoft.EntityFrameworkCore;
using NEVOLAplus.Intranet.Models.CMS;
using NEVOLAplus.Intranet.Models.HR;
using NEVOLAplus.Intranet.Models.Inventory;
using NEVOLAplus.Intranet.Models.Licensing;
using NEVOLAplus.Intranet.Models.Reservation;

namespace NEVOLAplus.Intranet.Data
{
    public class NevolaIntranetContext : DbContext
    {
        public NevolaIntranetContext(DbContextOptions<NevolaIntranetContext> options)
            : base(options)
        {
        }

        // CMS
        public DbSet<Page> Pages { get; set; } = null!;
        public DbSet<News> News { get; set; } = null!;

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
