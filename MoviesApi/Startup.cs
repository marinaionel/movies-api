using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MoviesApi.ApiClient.AzureFunctions;
using MoviesApi.ApiClient.OMDbApi;
using MoviesApi.Data;
using MoviesApi.Worker;
using Newtonsoft.Json;
using System;

namespace MoviesApi
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
            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "movies_api", Version = "v1" });
            });

            services.AddControllers().AddNewtonsoftJson(opt =>
            {
                opt.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                opt.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
                opt.SerializerSettings.TypeNameHandling = TypeNameHandling.Auto;
                opt.SerializerSettings.MissingMemberHandling = MissingMemberHandling.Ignore;
                opt.SerializerSettings.Formatting = Formatting.None;
            });

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                    .AddJwtBearer(options =>
                    {
                        options.Authority = "https://securetoken.google.com/1:475689474726:web:2894f95c246ccbe3963267";
                        options.TokenValidationParameters = new TokenValidationParameters
                        {
                            ValidateIssuer = true,
                            ValidIssuer = "https://securetoken.google.com/1:475689474726:web:2894f95c246ccbe3963267",
                            ValidateAudience = true,
                            ValidAudience = "1:475689474726:web:2894f95c246ccbe3963267",
                            ValidateLifetime = true
                        };
                    });

            services.AddSession(options => { options.IdleTimeout = TimeSpan.FromMinutes(60); });
            services.AddDistributedMemoryCache();

            services.AddDbContext<MoviesContext>(op =>
            {
                op.UseSqlServer(Configuration.GetConnectionString("Sep6Database"),
                sqlOptions =>
                {
                    sqlOptions.EnableRetryOnFailure(
                    maxRetryCount: 10,
                    maxRetryDelay: TimeSpan.FromSeconds(30),
                    errorNumbersToAdd: null);
                });
            });
            services.AddApplicationInsightsTelemetry(Configuration["APPINSIGHTS_CONNECTIONSTRING"]);

            services.AddSingleton<OMDBbServiceClient>();
            services.AddSingleton<GetTrailerClient>();

            //services.AddHostedService<DataFillingService>();
            //services.AddScoped<IScopedProcessingService, ScopedProcessingService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddLog4Net();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "movies_api v1"));
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseCors(a => a.WithOrigins(new string[] { "https://marinaionel.github.io/", "http://localhost:4200/" })
                              .AllowAnyHeader()
                              .AllowAnyMethod()
                              .AllowCredentials());

            app.UseHttpsRedirection();

            app.UseDefaultFiles();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
