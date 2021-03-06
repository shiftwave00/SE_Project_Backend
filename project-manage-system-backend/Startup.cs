using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using project_manage_system_backend.Hubs;
using project_manage_system_backend.Shares;
using System.Text;
using System.Threading.Tasks;

namespace project_manage_system_backend
{
    public class Startup
    {
        private readonly IWebHostEnvironment _env;
        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            Configuration = configuration;
            _env = env;
        }

        public IConfiguration Configuration { get; }

        private readonly string _developmentAllowOrigin = "http://localhost:8080";
        private readonly string _deployAllowOrigin = "localhost:8080";

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddLogging(builder =>
            {
                builder.AddConfiguration(Configuration.GetSection("Logging"))
                       .AddConsole()
                       .AddDebug();
            });

            if (_env.IsEnvironment("Testing"))
            {
                var connect = new SqliteConnection("DataSource=:memory:");
                connect.Open();

                services.AddDbContext<PMSContext>(options =>
                options.UseSqlite(connect));
            }
            else
            {
                services.AddDbContext<PMSContext>(options =>
                options.UseSqlite("Data Source=PMS_Database.db"));
            }

            services.AddSingleton<JwtHelper>();
            services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.SaveToken = true;
                options.IncludeErrorDetails = true; // ???????????? true????????????????????????

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    // ????????????????????????????????? "sub" ?????????????????? User.Identity.Name
                    NameClaimType = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier",
                    // ????????????????????????????????? "roles" ?????????????????? [Authorize] ????????????
                    RoleClaimType = "http://schemas.microsoft.com/ws/2008/06/identity/claims/role",

                    // ???????????????????????? Issuer
                    ValidateIssuer = true,
                    ValidIssuer = Configuration.GetValue<string>("JwtSettings:Issuer"),

                    // ???????????????????????? Audience
                    ValidateAudience = false,

                    // ???????????????????????? Token ???????????????
                    ValidateLifetime = true,

                    // ?????? Token ????????? key ?????????????????????????????????????????????
                    ValidateIssuerSigningKey = false,

                    // "1234567890123456" ????????? IConfiguration ??????
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration.GetValue<string>("JwtSettings:SignKey")))
                };

                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        var accessToken = context.Request.Headers["Authorization"];

                        // If the request is for our hub...
                        var path = context.HttpContext.Request.Path;
                        if (!string.IsNullOrEmpty(accessToken) &&
                            (path.StartsWithSegments("/hub/notify")))
                        {
                            // Read the token out of the query string
                            context.Token = accessToken;
                        }
                        return Task.CompletedTask;
                    }
                };
            });

            services.AddCors(options =>
            {
                options.AddPolicy("DevelopmentPolicy", policy =>
                {
                    policy.WithOrigins(_developmentAllowOrigin)
                          .AllowAnyHeader()
                          .AllowAnyMethod()
                          .AllowCredentials();
                });

                options.AddPolicy("DeployPolicy", policy =>
                {
                    policy.WithOrigins(_deployAllowOrigin)
                          .AllowAnyHeader()
                          .AllowAnyMethod()
                          .AllowCredentials();
                });
            });

            services.AddSignalR();
            services.AddControllers().AddNewtonsoftJson(opt => opt.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseExceptionHandler("/error-local-development");
            }
            else
            {
                app.UseExceptionHandler("/error");
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            if (env.IsDevelopment())
            {
                app.UseCors("DevelopmentPolicy");
            }
            else
            {
                app.UseCors("DeployPolicy");
            }
            app.UseAuthentication();

            app.UseAuthorization();


            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHub<NotifyHub>("/hub/notify");
                endpoints.MapControllers();
            });

        }
    }
}
