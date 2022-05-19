using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using CelebrityJourneyTrackerV1.External;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using CelebrityJourneyTrackerV1.Configurations;
using CelebrityJourneyTrackerV1.Services;

namespace CelebrityJourneyTrackerV1
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public static IConfiguration Configuration { get; private set; }

        // This method gets called by the runtime. Use this method to add services to the container
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<YoutubeConfiguration>(Configuration.GetSection("YoutubeApi"));
            services.Configure<AwsConfiguration>(Configuration.GetSection("Aws"));

            services
                .AddSingleton<IYoutubeFactory, YoutubeFactory>()
                .AddSingleton<IYoutubeApi,YoutubeApi>()
                .AddSingleton<IAwsFactory, AwsFactory>()
                .AddSingleton<IAwsApi, AwsApi>();

            services.AddCors(options =>
            {
                options.AddDefaultPolicy(
                    builder =>
                    {
                        // If not overwritten by Env Variable, will use default empty string (from appsettings.json) which matches no origins
                        //var corsOrigin = Configuration.GetSection("SecuritySettings").GetValue<string>("CorsOrigin");
                        builder.AllowAnyOrigin().AllowAnyMethod();
                    });
            });
            services.AddControllers();
            
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                Console.WriteLine("ENVIRONMENT is development");
                app.UseDeveloperExceptionPage();
            }
            else
                Console.WriteLine("ENVIRONMENT is NOT development");

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseCors();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapGet("/", async context =>
                {
                    await context.Response.WriteAsync("Welcome to running ASP.NET Core on AWS Lambda");
                });
            });
        }
    }
}
