using ApiPeliculas.Services;
using Microsoft.EntityFrameworkCore;

namespace ApiPeliculas
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // injectamos DbContext and other services
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
               options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            services.AddControllers()
                .AddNewtonsoftJson();
            //services.AddEndpointsApiExplorer();

            services.AddAutoMapper(typeof(Startup));
            
            //to store file in wwwroot filder
            services.AddTransient<IFileStorage, FileLocalStorage>();
            services.AddHttpContextAccessor();

            //redis 
            services.AddScoped<ICacheService, CacheService>();
            
            services.AddStackExchangeRedisCache(option =>
            {         
                option.Configuration = Configuration.GetValue<string>("Caching:RedisConnection");
                option.InstanceName = "redisTest";
            });


        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            //app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            //app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
