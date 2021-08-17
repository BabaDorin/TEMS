using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System;
using System.Security.Claims;
using System.Text;
using temsAPI.Contracts;
using temsAPI.Data;
using temsAPI.Data.Entities.UserEntities;
using temsAPI.Data.Managers;
using temsAPI.Repository;
using temsAPI.Services;
using temsAPI.Services.JWT;
using temsAPI.Services.Report;
using temsAPI.Services.SICServices;
using temsAPI.System_Files;
using temsAPI.System_Files.Exceptions;
using temsAPI.System_Files.TEMSFileLogger;
using temsAPI.System_Files.TEMSInitializer;

namespace temsAPI
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
            //services.Configure<AppSettings>(Configuration.GetSection("AppSettings"));
            services.Configure<FileLoggerSettings>(Configuration.GetSection("FileLoggerSettings"));
            services.AddDistributedMemoryCache();
            services.AddSession();

            services.AddDbContextPool<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("DefaultConnection")));
            services.AddDatabaseDeveloperPageExceptionFilter();

            services.AddIdentityCore<TEMSUser>(options => options.SignIn.RequireConfirmedAccount = true)
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>();

            
            services.AddDefaultIdentity<TEMSUser>()
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>();

            services.Configure<FormOptions>(x =>
            {
                x.ValueLengthLimit = int.MaxValue;
                x.MultipartBodyLengthLimit = int.MaxValue; // In case of multipart
            });

            services.AddCors();

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "temsAPI", Version = "v1" });
            });

            services.Configure<IdentityOptions>(q =>
            {
                q.Password.RequireDigit = false;
                q.Password.RequiredLength = 5;
                q.Password.RequireNonAlphanumeric = false;
                q.Password.RequireLowercase = false;
                q.Password.RequireUppercase = false;
            });

            var key = Encoding.UTF8.GetBytes(Configuration["AppSettings:JWT_Secret"].ToString());

            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = false;
                x.SaveToken = false;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero
                };
            });

            services.AddAuthorization(options =>
            {
                options.AddPolicy("CanSendEmails", policy => policy.RequireClaim("Can send emails"));
            });

            services.AddTransient<IDBDesigner, DBDesigner>();
            services.AddTransient<IUnitOfWork, UnitOfWork>();
            services.AddTransient<ClaimsPrincipal>(s =>
                s.GetService<IHttpContextAccessor>().HttpContext?.User ?? null);

            // TEMS Services
            services.AddSingleton<SystemConfigurationService>();
            services.AddSingleton<CurrencyConvertor>();
            services.AddSingleton<TokenValidatorService>();

            services.AddScoped<RoutineCheckService>();
            services.AddScoped<ReportingService>();
            services.AddScoped<IdentityService>();
            services.AddScoped<EmailService>();
            services.AddScoped<SICService>();
            services.AddScoped<SICIntegrationService>();

            services.ConfigureWritable<AppSettings>(Configuration.GetSection("AppSettings"));

            // Scheduler
            services.AddSingleton<Scheduler>();

            // TEMS Entity managers
            services.AddScoped<ReportManager>();
            services.AddScoped<EquipmentManager>();
            services.AddScoped<EquipmentDefinitionManager>();
            services.AddScoped<EquipmentTypeManager>();
            services.AddScoped<EquipmentPropertyManager>();
            services.AddScoped<ArchieveManager>();
            services.AddScoped<AnnouncementManager>();
            services.AddScoped<TEMSUserManager>();
            services.AddScoped<KeyManager>();
            services.AddScoped<LibraryManager>();
            services.AddScoped<LogManager>();
            services.AddScoped<PersonnelManager>();
            services.AddScoped<RoomManager>();
            services.AddScoped<TicketManager>();
            services.AddScoped<AnalyticsManager>();
            services.AddScoped<NotificationManager>();
            services.AddScoped<BugReportManager>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(
            IApplicationBuilder app,
            IWebHostEnvironment env,
            UserManager<TEMSUser> userManager,
            RoleManager<IdentityRole> roleManager,
            ApplicationDbContext dbContext,
            ILogger<DefaultExceptionHandler> logger,
            SystemConfigurationService systemConfigurationService,
            Scheduler scheduler)
        {
            Initializer.PrepareSolution();
            SeedData.Seed(userManager, roleManager, dbContext, systemConfigurationService);
            DefaultExceptionHandler.logger = logger;

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    string swaggerJsonBasePath = string.IsNullOrWhiteSpace(c.RoutePrefix) ? "." : "..";
                    c.SwaggerEndpoint($"{swaggerJsonBasePath}/swagger/v1/swagger.json", "temsAPI v1");
                });
            }

            app.UseSession();
            app.UseRouting();
            app.UseCors(builder =>
                builder
                .SetIsOriginAllowedToAllowWildcardSubdomains()
                .WithOrigins(
                    Configuration["AppSettings:Client_Url"].ToString(), 
                    Configuration["AppSettings:Client_Url_Development"].ToString())
                .AllowAnyMethod()
                .AllowCredentials()
                .AllowAnyHeader()
                .Build()
            );
            app.UseAuthentication();
            app.UseRouting();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });

            scheduler.Start();
        }
    }
}
