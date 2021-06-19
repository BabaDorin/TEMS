using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using temsAPI.Contracts;
using temsAPI.Data;
using temsAPI.Data.Entities.UserEntities;
using temsAPI.Data.Managers;
using temsAPI.Helpers;
using temsAPI.Mappings;
using temsAPI.Repository;
using temsAPI.Services;
using temsAPI.Services.Notification;
using temsAPI.Services.Report;
using temsAPI.System_Files;

namespace temsAPI
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        readonly string MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<AppSettings>(Configuration.GetSection("AppSettings"));
            services.AddDistributedMemoryCache();
            services.AddSession();

            services.AddDbContextPool<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("DefaultConnection")));
            services.AddDatabaseDeveloperPageExceptionFilter();

            services.AddIdentityCore<TEMSUser>(options => options.SignIn.RequireConfirmedAccount = true)
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>();

            services.AddTransient<IUnitOfWork, UnitOfWork>();
            services.AddAutoMapper(typeof(Maps));

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

            services.AddTransient<ClaimsPrincipal>(s =>
                s.GetService<IHttpContextAccessor>().HttpContext.User);

            // TEMS Services
            services.AddScoped<ReportingService>();
            services.AddScoped<IdentityService>();
            services.AddScoped<EmailService>();
            services.AddSingleton<CurrencyConvertor>();
            services.AddScoped<EmailNotificationService>();
            services.AddScoped<SMSNotificationService>();
            services.AddScoped<BrowserNotificationService>();
            services.AddScoped<NotificationService>();

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
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(
            IApplicationBuilder app,
            IWebHostEnvironment env,
            UserManager<TEMSUser> userManager,
            RoleManager<IdentityRole> roleManager,
            ApplicationDbContext dbContext)
        {

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                //app.UseSwagger();
                //app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "temsAPI v1"));
            }

            //app.UseHttpsRedirection();
            app.UseSession();
            app.UseRouting();
            //app.UseCors(MyAllowSpecificOrigins);
            
            SeedData.Seed(userManager, roleManager, dbContext);
            TemsStarter.Start();

            app.UseCors(builder =>
            builder.WithOrigins(Configuration["AppSettings:Client_URL"].ToString())
            .AllowAnyHeader()
            .AllowAnyMethod()

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

            //(new Scheduler(unitOfWork)).Start();
        }
    }
}
