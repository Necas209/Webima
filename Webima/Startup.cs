using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Webima.Data;
using Webima.Services;

namespace Webima
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    //Configuration.GetConnectionString("DefaultConnection")));
                    Configuration.GetConnectionString("MyDbConnection")));
            services.AddDatabaseDeveloperPageExceptionFilter();

            services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>();
            services.AddControllersWithViews();

            services.AddTransient<IEmailSender, EmailSender>();
            services.Configure<AuthMessageSenderOptions>(Configuration);

            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(10);
                options.Cookie.Name = "Webima.Session";
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseMigrationsEndPoint();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseSession();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapRazorPages();
                endpoints.MapControllerRoute(
                    name: "conta",
                    pattern: "Conta",
                    defaults: new { controller = "Conta", action = "Index" });
                endpoints.MapRazorPages();
                endpoints.MapControllerRoute(
                    name: "cartaz",
                    pattern: "Cartaz",
                    defaults: new { controller = "Filmes", action = "Cartaz" });
                endpoints.MapRazorPages();
                endpoints.MapControllerRoute(
                    name: "estreias",
                    pattern: "Estreias",
                    defaults: new { controller = "Filmes", action = "Estreias" });
                endpoints.MapRazorPages();
                endpoints.MapControllerRoute(
                    name: "acerca",
                    pattern: "FAQs",
                    defaults: new { controller = "Home", action = "FAQs" });
                endpoints.MapRazorPages();
                endpoints.MapControllerRoute(
                    name: "acerca",
                    pattern: "Acerca",
                    defaults: new { controller = "Home", action = "Acerca" });
                endpoints.MapRazorPages();
            });
        }
    }
}
