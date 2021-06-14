using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Identity;
using PressStart.Data;

using Microsoft.Extensions.Logging;
using System.Net;

namespace PressStart
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
            services.AddRazorPages();
            services.AddDbContext<PressStartContext>();
            services.AddIdentity<IdentityUser, IdentityRole>(options => options.SignIn.RequireConfirmedAccount = false)
            .AddEntityFrameworkStores<PressStartContext>();
            services.AddAuthentication().AddFacebook(facebookOptions =>
            {
                facebookOptions.AppId = Configuration["Authentication:Facebook:AppId"];
                facebookOptions.AppSecret = Configuration["Authentication:Facebook:AppSecret"];
            });

            services.AddHttpsRedirection(options =>
            {
                options.RedirectStatusCode = (int)HttpStatusCode.TemporaryRedirect;
                options.HttpsPort = 5001;
            });


        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseAuthentication();

            app.UseRouting();

            app.UseAuthorization();

            SeedUsersAndRoles(userManager, roleManager);

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
            });
        }

        private void SeedUsersAndRoles(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            // note: we only seed roles in this particular example but
            // you may want to seed users with assigned roles too (e.g. an Administrator user)
            string[] roleNamesList = new string[] { "User", "Admin" };

            foreach (string roleName in roleNamesList)
            {
                if (!roleManager.RoleExistsAsync(roleName).Result)
                {
                    IdentityRole role = new IdentityRole();
                    role.Name = roleName;
                    IdentityResult result = roleManager.CreateAsync(role).Result;
                    // WARNING: we ignore any errors that Create may return, they should be AT LEAST logged !
                    foreach (IdentityError error in result.Errors)
                    {
                        // TODO: Log it!
                    }
                }

            }

            // WARNING: For testing ONLY. Do NOT do it on a production system!
            // Create an Administrator. 
            string adminEmail = "admin@admin.com";
            string adminPass = "Admin123!"; // a terrible password
            if (userManager.FindByNameAsync(adminEmail).Result == null)
            {
                IdentityUser user = new IdentityUser();
                user.UserName = adminEmail;
                user.Email = adminEmail;
                user.EmailConfirmed = true;
                IdentityResult result = userManager.CreateAsync(user, adminPass).Result;
                if (result.Succeeded)
                {
                    IdentityResult result2 = userManager.AddToRoleAsync(user, "Admin").Result;
                    if (!result2.Succeeded)
                    {
                        // FIXME: log the error
                    }
                }
                else
                {
                    // FIXME: log the error
                }
            }
        }
    }
}
