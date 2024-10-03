using Cyh.Net.Data;
using ImgRepo.Model;
using Microsoft.EntityFrameworkCore;

namespace ImgRepo.Web
{
    public class Startup
    {
        IConfiguration m_configuration;
        public Startup(IConfiguration configuration)
        {
            this.m_configuration = configuration;
        }


        public void ConfigureServices(IServiceCollection services)
        {
            string? connStr = this.m_configuration.GetConnectionString("DbConnectionSql");

            services.AddDbContext<ImageRepositoryContext>(opt =>
            {
                opt.UseSqlServer(connStr);
            });

            services.AddScoped<IDataSource>(sp =>
            {
                return sp.GetService<ImageRepositoryContext>()!;
            });

            services.AddScoped(ImgRepo.Service.Factories.GetImageService);
            services.AddScoped(ImgRepo.Service.Factories.GetArtistService);
            services.AddScoped(ImgRepo.Service.Factories.GetCommonAttributeService);

            services.AddControllersWithViews().AddJsonOptions((jsonOpt) =>
            {
                jsonOpt.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
            });
            services.AddCors((opt) =>
            {
                Microsoft.AspNetCore.Cors.Infrastructure.CorsPolicy policy = new Microsoft.AspNetCore.Cors.Infrastructure.CorsPolicy();
                policy.Origins.Add("*");
                policy.Headers.Add("*");
                policy.Methods.Add("*");
                opt.AddDefaultPolicy(policy);
            });
            services.AddRazorPages();
        }


        public static void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {

            using (IServiceScope serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>()!.CreateScope())
            {
                ImageRepositoryContext context = serviceScope.ServiceProvider.GetRequiredService<ImageRepositoryContext>();
                context.Database.EnsureCreated();
            }

            if (!env.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
            }
            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthorization();
#pragma warning disable
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapRazorPages();
            });
#pragma warning restore
        }
    }
}
