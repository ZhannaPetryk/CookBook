using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CookBook.DAL;
using CookBook.DAL.Data;
using CookBook.DAL.Interfaces;
using CookBook.DAL.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace CookBook.WebUI
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();
            
            string connection = Configuration.GetConnectionString("CookBookConnectionString");
            services.AddDbContextPool<CookBookDbContext>(options => options
                .UseSqlServer(connection));
            services.AddIdentity<AdminUser, IdentityRole>()
                .AddEntityFrameworkStores<CookBookDbContext>();
            
            services.AddScoped<ICookBookDbContext>(provider => provider.GetService<CookBookDbContext>());
            services.AddScoped<IRecipeService, RecipeService>();
            services.AddScoped<IRevisionService, RevisionService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            SeedDataDefaultAdmin(app);
            
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            
            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();
            
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    "default",
                    "{controller}/{action=Index}/{id?}"
                );
            });
        }
        
        private void SeedDataDefaultAdmin(IApplicationBuilder app)
        {
            using (var serviceScope = app.ApplicationServices
                .GetRequiredService<IServiceScopeFactory>()
                .CreateScope())
            {
                using (var context = serviceScope.ServiceProvider.GetService<CookBookDbContext>())
                {
                    var defaultAdmin =  context.Users.FirstOrDefaultAsync(x => x.UserName == "Admin").Result;
                    if (defaultAdmin == null)
                    {
                        var user = new AdminUser
                        {
                            UserName = "Admin",
                            Email = "admin@user.com"
                        };

                        var userManager = serviceScope.ServiceProvider.GetService<UserManager<AdminUser>>();
                        var result = userManager.CreateAsync(user, "CNDdPwd#4583").Result;
                        context.SaveChanges();
                    }
                }
            } 
        }
    }
}