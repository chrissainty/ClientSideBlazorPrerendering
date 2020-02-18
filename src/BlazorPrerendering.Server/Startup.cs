using System;
using System.Linq;
using System.Net.Http;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace BlazorPrerendering.Server
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().AddNewtonsoftJson();
            services.AddResponseCompression(opts =>
            {
                opts.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(
                    new[] { "application/octet-stream" });
            });

            services.AddScoped<HttpClient>(s =>
            {
                var navigationManager = s.GetRequiredService<NavigationManager>();
                return new HttpClient
                {
                    BaseAddress = new Uri(navigationManager.BaseUri)
                };
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseResponseCompression();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBlazorDebugging();
            }

            app.UseClientSideBlazorFiles<Client.Program>();

            app.UseStaticFiles();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
                endpoints.MapFallbackToPage("/_Host");
            });
        }
    }
}
