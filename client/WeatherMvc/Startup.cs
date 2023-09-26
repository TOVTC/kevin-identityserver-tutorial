using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using WeatherMvc.Services;

namespace WeatherMvc
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
            services.AddControllersWithViews();

            // register configuration for user authentication
            services.AddAuthentication(options =>
            {
                // the name of the cookie that will be used to maintain the user's authentication
                options.DefaultScheme = "cookie";
                // use oidc as the challenge mechanism for the user
                // basically indicates that we will call IdentityServer and have it handle authentication for us
                options.DefaultChallengeScheme = "oidc";
            })
                .AddCookie("cookie")
                // configure oidc using NuGet package
                .AddOpenIdConnect("oidc", options =>
                {
                    // configure the openid server we are going to use (taken from the interactiveServiceSettings section in appSettings.json)
                    options.Authority = Configuration["InteractiveServiceSettings:AuthorityUrl"];
                    options.ClientId = Configuration["InteractiveServiceSettings:ClientId"];
                    options.ClientSecret = Configuration["InteractiveServiceSettings:ClientSecret"];

                    // specifies that we are using the authorization code flow
                    options.ResponseType = "code";
                    options.UsePkce = true;
                    options.ResponseMode = "query";

                    // specify the scopes being asked for
                    options.Scope.Add(Configuration["InteractiveServiceSettings:Scopes:0"]);
                    // causes the identity and access tokens to be saved, making them available to other parts of the code
                    options.SaveTokens = true;
                });

            // register configuration for token service
            services.Configure<IdentityServerSettings>(Configuration.GetSection("IdentityServerSettings"));
            services.AddSingleton<ITokenService, TokenService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            // add authentication to the pipeline
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                  name: "default",
                  pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}