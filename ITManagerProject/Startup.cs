using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using ITManagerProject.Contexts;
using ITManagerProject.HelperTypes;
using ITManagerProject.Managers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.EntityFrameworkCore;
using ITManagerProject.Models;
using ITManagerProject.Validators;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Westwind.AspNetCore.LiveReload;

namespace ITManagerProject
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

            services.AddIdentity<User, Role>(options =>
            {
                options.User.RequireUniqueEmail = true;
            }).AddEntityFrameworkStores<UserAppContext>().AddErrorDescriber<LocalizedIdentityErrorDescriber>();
            
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie();
            services.AddAuthorization(options =>
            {
                options.AddPolicy(PolicyTypes.Users.Manage, policy =>
                {
                    policy.RequireClaim(CustomClaimTypes.Permission, Permissions.Users.View);
                    policy.RequireClaim(CustomClaimTypes.Permission, Permissions.Users.Add);
                    policy.RequireClaim(CustomClaimTypes.Permission, Permissions.Users.Remove);
                    policy.RequireClaim(CustomClaimTypes.Permission, Permissions.Users.Edit);
                });
                
                options.AddPolicy(PolicyTypes.Users.View, policy =>
                {
                    policy.RequireClaim(CustomClaimTypes.Permission, Permissions.Users.View);
                });
                
                options.AddPolicy(PolicyTypes.Users.Edit, policy =>
                {
                    policy.RequireClaim(CustomClaimTypes.Permission, Permissions.Users.Edit);
                    policy.RequireClaim(CustomClaimTypes.Permission, Permissions.Users.View);
                });
                
                options.AddPolicy(PolicyTypes.Organization.Remove, policy =>
                {
                    policy.RequireClaim(CustomClaimTypes.Permission, Permissions.Organization.Remove);
                });
            });
            services.ConfigureApplicationCookie(options =>
            {
                options.AccessDeniedPath = new PathString("/Dashboard/AccessDenied");
                options.Events.OnValidatePrincipal = PrincipalValidator.ValidateAsync;
            });

            services.AddDbContext<UserAppContext>(builder =>
            {
                builder.UseSqlite(Configuration.GetConnectionString("DefaultConnection"));
            });
            
            services.TryAddScoped<OrganizationManager<Organization>>();
            //services.AddAuthorization();
            services.AddLiveReload(config =>
            {
                // optional - use config instead
                //config.LiveReloadEnabled = true;
                //config.FolderToMonitor = Path.GetFullname(Path.Combine(Env.ContentRootPath,"..")) ;
            });
            services.AddDatabaseDeveloperPageExceptionFilter();
            services.AddRazorPages();
            services.AddControllersWithViews().AddRazorRuntimeCompilation();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseLiveReload();
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

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapRazorPages();
            });
        }
    }
}