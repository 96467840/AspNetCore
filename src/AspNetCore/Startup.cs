using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Reflection;
using Microsoft.Extensions.FileProviders;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.WebEncoders;
using System.Text.Unicode;
using System.Text.Encodings.Web;
using NLog.Extensions.Logging;
using NLog.Web;
using AspNetCoreSqlite;
using AspNetCoreComponentLibrary.Abstractions;

namespace AspNetCore
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();

            if (env.IsDevelopment())
            {
                env.ConfigureNLog("nlog.config");
            }
            else
            {
                env.ConfigureNLog("nlog.linux.config");
            }
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddMvc();

            // Setup options with DI
            services.AddOptions();
            services.Configure<SQLiteConfigure>(Configuration.GetSection("SQLiteConfigure"));
            // обязательно AddScoped (ибо в каждом запросе мы юзаем 2 БД)
            services.AddScoped<IStorage, AspNetCoreSqlite.Storage>();

            // чтобы во вьюхах русские символы не кодировались
            services.Configure<WebEncoderOptions>(options =>
            {
                options.TextEncoderSettings = new TextEncoderSettings(UnicodeRanges.All);
            });

            // конфигурируем подгрузку представлений из библиотеки
            var assembly = typeof(AspNetCoreComponentLibrary.TestComponent).GetTypeInfo().Assembly;
            var embededFileProvider = new EmbeddedFileProvider(assembly, "AspNetCoreComponentLibrary");
            services.Configure<RazorViewEngineOptions>(options => { options.FileProviders.Add(embededFileProvider); });

            // https://docs.microsoft.com/ru-ru/aspnet/core/performance/caching/middleware
            services.AddResponseCaching();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory, IStorage Storage)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            loggerFactory.AddNLog();
            app.AddNLogWeb();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseMvc(routes =>
            {
                //routes.MapRoute(name: "default", template: "{controller=Home}/{action=Index}/{id?}");
                routes.MapRoute("Setup", "setup", new { controller = "Setup", action = "Index" });
                routes.MapRoute("Page", "{lang?}/{page?}/", new { controller = "Home", action = "Index" });
            });

            //DbInitializer.Initialize(context);
            Storage.UpdateDBs();
        }
    }
}
