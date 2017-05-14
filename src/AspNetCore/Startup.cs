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
using AspNetCoreComponentLibrary;
using Microsoft.AspNetCore.Routing;
using System.Globalization;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Localization;

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

            if (env.IsProduction())
            {
                env.ConfigureNLog("nlog.Production.config");
            }
            else
            {
                env.ConfigureNLog("nlog.config");
            }
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddLocalization(options => options.ResourcesPath = "Resources");

            // Add framework services.
            services.AddMvc()
                // локализацию шаблонов мы не юзаем, но мы юзаем инжект на вьюхах (IViewLocalizer)
                .AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix) //Adds support for localized view files. In this sample view localization is based on the view file suffix. For example "fr" in the Index.fr.cshtml file.
                .AddDataAnnotationsLocalization();

            // Setup options with DI
            services.AddOptions();

            #region DB config

            services.Configure<SQLiteConfigure>(Configuration.GetSection("SQLiteConfigure"));
            // обязательно AddScoped (ибо в каждом запросе мы юзаем 2 БД, причем 1 БД может быть своя для каждого сайта)
            services.AddScoped<IStorage, AspNetCoreSqlite.Storage>();

            #endregion

            // чтобы во вьюхах русские символы не кодировались
            services.Configure<WebEncoderOptions>(options =>
            {
                options.TextEncoderSettings = new TextEncoderSettings(UnicodeRanges.All);
            });

            // генерируем урлы в низком регистре (так как для и линукса делаем, а там привычнее когда все пути в низком регистре)
            services.Configure<RouteOptions>(options => options.LowercaseUrls = true);

            // конфигурируем подгрузку представлений из библиотеки
            var assembly = typeof(AspNetCoreComponentLibrary.TestComponent).GetTypeInfo().Assembly;
            var embededFileProvider = new EmbeddedFileProvider(assembly, "AspNetCoreComponentLibrary");
            services.Configure<RazorViewEngineOptions>(options => { options.FileProviders.Add(embededFileProvider); });

            // https://docs.microsoft.com/ru-ru/aspnet/core/performance/caching/middleware
            services.AddResponseCaching();

            // необходимо для NLog (без этого в логах не будет текущего запроса)
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            // даем возможность переопределить локализацию библиотеки
            services.AddSingleton<IStringLocalizer, StringLocalizer<SharedResource>>();

            services.AddTransient<IControllerSettings, ControllerSettings>();
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
                app.UseExceptionHandler("/e");
            }

            // нам такой вариант не подходит у нас разный список культур, для каждого сайта свой
            /*var supportedCultures = new[]
            {
                new CultureInfo("en-US"),
                new CultureInfo("ru-RU"),
            };/**/
            /*app.UseRequestLocalization(new RequestLocalizationOptions
            {
                DefaultRequestCulture = new RequestCulture("en-US"), //new RequestCulture("en-US"),
                // Formatting numbers, dates, etc.
                //SupportedCultures = supportedCultures,
                // UI strings that we have localized.
                //SupportedUICultures = supportedCultures
            });/**/

            app.UseStaticFiles();

            Dictionary<string, RouteConfig> RoutesForReplace = null;
            if (env.IsDevelopment())
            {
                RoutesForReplace = new Dictionary<string, RouteConfig>
                {
                    // чтобы не было конфликта с именем страницы (и шаблоном культуры) используем /t/ (имя страницы обязательно должно иметь длину больше 2 и отличаться от шаблона культуры)
                    { "Test.Culture", new RouteConfig(100, "{culture}/t/{action}/{*path}", new { controller = "Test", action = "Index" }) },
                    { "Test",         new RouteConfig(101, "t/{action}/{*path}", new { controller = "Test", action = "Index" }) },

                    // если вдруг припрет сделать страницу с именем из 2 и менее букв, то необходимо для этой страницы переопределить правила вручную
                    // например так
                    { "ShortPageS.Culture", new RouteConfig(200, "{culture}/s/{*path}", new { controller = "Home", action = "Index", page="s" })},
                    { "ShortPageS",         new RouteConfig(201, "s/{*path}", new { controller = "Home", action = "Index", page="s" })},
                    
                    // для 2 букв
                    { "ShortPageSS.Culture", new RouteConfig(300, "{culture}/ss/{*path}", new { controller = "Home", action = "Index", page="ss" })},
                    { "ShortPageSS",         new RouteConfig(301, "ss/{*path}", new { controller = "Home", action = "Index", page="ss" })},
                };
            }
            app.UseMvc(routes =>
            {
                DefaultRoutes.Register(routes, app, RoutesForReplace);
            });

            // временно отключим миграции на тестовых машинах. слишком много шума в логах (не забываем про IsDevelopment, а то разок уже обновил продакшен и забыл про это)
            if (env.IsProduction())
            {
                Storage.UpdateDBs();
            }
        }
    }
}
