using System;
using System.IO;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.DataAccess;

namespace PromoCodeFactory.WebHost;

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
        services.AddControllers();
        services.AddScoped(typeof(IRepository<>), typeof(EFRepository<>));

        services.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseSqlite(Configuration.GetConnectionString("SqliteConnection"));
            options.UseLazyLoadingProxies();
        });


        services.AddOpenApiDocument(options =>
        {
            options.Title = "PromoCode Factory API Doc";
            options.Version = "1.0";
        });

        //https://stackoverflow.com/questions/39459348/asp-net-core-web-api-no-route-matches-the-supplied-values
        services.AddMvc(options => { options.SuppressAsyncSuffixInActionNames = false; });
        services.AddAutoMapper(typeof(Startup).Assembly);
        services.AddSwaggerGen(options =>
        {
            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            if (File.Exists(xmlPath))
            {
                options.IncludeXmlComments(xmlPath);
            }
        });
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ApplicationDbContext dbContext)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
            dbContext.Database.Migrate();
        }
        else
        {
            app.UseHsts();
        }

        app.UseOpenApi();
        app.UseSwaggerUi(x => { x.DocExpansion = "list"; });

        app.UseHttpsRedirection();

        app.UseRouting();

        app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
    }
}