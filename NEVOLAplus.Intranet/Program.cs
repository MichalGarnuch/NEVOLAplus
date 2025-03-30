using Microsoft.EntityFrameworkCore;
using NEVOLAplus.Data;

namespace NEVOLAplus.Intranet

{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // 1) Dodaj kontekst EF
            builder.Services.AddDbContext<NevolaIntranetContext>(options =>
                options.UseSqlServer(
                    builder.Configuration.GetConnectionString("NevolaIntranetContext")));

            // 2) Dodaj kontrolery z widokami
            builder.Services.AddControllersWithViews();

            var app = builder.Build();

            // 3) Obs³uga b³êdów
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            // 4) Middleware
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthorization();

            // 5) Routing
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
