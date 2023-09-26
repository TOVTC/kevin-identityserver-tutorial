using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using IdentityServer4.Models;
using IdentityServer4.Test;
using Ids;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ids
{
    public class Startup
    {

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            // add identity server and specify where it can find its resources
            // can initialize with empty lists if no configuration available
            services.AddIdentityServer()
                .AddInMemoryClients(Config.Clients)
                .AddInMemoryIdentityResources(Config.IdentityResources)
                .AddInMemoryApiResources(Config.ApiResources)
                .AddInMemoryApiScopes(Config.ApiScopes)
                .AddTestUsers(Config.Users)
                // provides signing material for various resources but is for dev scenarios only (for when you don't have a certificate to use)
                // in production, AddSigningCredential or AddValidationKey should be used instead
                .AddDeveloperSigningCredential();

            // add the following to allow IdentityServer to use IdentityServer UI
            // adds all the controllers to the services collection
            services.AddControllersWithViews();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();
            // add identity server into the pipeline - identity server itself is a piece of middleware
            app.UseIdentityServer();
            // add use authorization to allow authorization of users
            app.UseAuthorization();

            // update the endpoint to server the IdentityServer UI
            app.UseEndpoints(endpoints => endpoints.MapDefaultControllerRoute());

        }
    }
}