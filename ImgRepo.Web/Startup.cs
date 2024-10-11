using Cyh.Net;
using Cyh.Net.Data;
using ImgRepo.Model;
using ImgRepo.Web.StreamFileHelper;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.EntityFrameworkCore;

namespace ImgRepo.Web
{
    public class Startup
    {
        readonly IConfiguration m_configuration;
        public Startup(IConfiguration configuration)
        {
            this.m_configuration = configuration;
        }


        public void ConfigureServices(IServiceCollection services)
        {
            string? connStr = this.m_configuration.GetConnectionString("DbConnectionSql");
            services.Configure<IISOptions>(options =>
            {
                options.AutomaticAuthentication = false;
                options.ForwardClientCertificate = false;
            });
            services.AddDbContext<ImageRepositoryContext>(opt =>
            {
                opt.UseSqlServer(connStr);
            });

            services.AddScoped<IDataSource>(sp =>
            {
                return sp.GetService<ImageRepositoryContext>()!;
            });

            string? filePath = this.m_configuration.GetSection("FileServer")["Root"];

            if (filePath.IsNullOrEmpty())
            {
                throw new Exception("FileServer Root is not set in appsettings.json");
            }

            services.AddScoped(sp => ImgRepo.Service.Factories.GetFileAccessService(sp, filePath));
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
            services.AddRazorPages(op =>
            {
                op.Conventions.AddPageApplicationModelConvention("/StreamedSingleFileUploadPhysical", model =>
                {
                    model.Filters.Add(new GenerateAntiforgeryTokenCookieAttribute());
                    model.Filters.Add(new DisableFormValueModelBindingAttribute());
                });
            });
            services.Configure<FormOptions>(options =>
            {
                options.BufferBodyLengthLimit = long.MaxValue;
                options.KeyLengthLimit = int.MaxValue;
                options.MultipartBodyLengthLimit = long.MaxValue;
                options.MultipartBoundaryLengthLimit = int.MaxValue;
                options.ValueCountLimit = int.MaxValue;
                options.ValueLengthLimit = int.MaxValue;
            });
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
