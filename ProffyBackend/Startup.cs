using System;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using AspNetCore.Authentication.ApiKey;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using ProffyBackend.Models;
using ProffyBackend.Providers.APIKey;
using ProffyBackend.Services.Auth;

namespace ProffyBackend
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        private User GetSuperAdminFromSettings()
        {
            var user = new User
            {
                Email = Configuration["SUPER_ADMIN_EMAIL"],
                Password = BCrypt.Net.BCrypt.HashPassword(Configuration["SUPER_ADMIN_PASSWORD"]),
                FirstName = "Super",
                LastName = "Admin",
                PhoneNumber = Configuration["SUPER_ADMIN_PHONE_NUMBER"] ?? "+55912345678",
                Role = Role.SuperAdmin
            };
            return user;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            try
            {
                var dbContextOptionsBuilder = new DbContextOptionsBuilder<DataContext>();
                dbContextOptionsBuilder.UseSqlite("FileName=data.db");
                var dbContext = new DataContext(dbContextOptionsBuilder.Options);
                var admin = GetSuperAdminFromSettings();
                if (admin != null && dbContext.Users.Count(u => u.Email == admin.Email) == 0)
                    dbContext.Users.Add(admin);
                dbContext.SaveChanges();
            }
            catch (Exception)
            {
                Console.WriteLine("Could not insert admin account");
            }

            services.AddDbContext<DataContext>(opt => opt.UseSqlite("FileName=data.db"));

            services.AddAuthentication(x =>
                {
                    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(x =>
                {
                    x.Events = new JwtBearerEvents
                    {
                        OnMessageReceived = ctx =>
                        {
                            ctx.Token = ctx.Request.Headers.FirstOrDefault(p => p.Key == "Authorization").Value
                                .ToString() ?? "";

                            if (ctx.Token != "" && ctx.Token.Split(" ").Length > 1)
                                ctx.Token = ctx.Token.Split(" ")[1];
                            else
                                ctx.Token = ctx.Request.Cookies["proffy-refresh"];
                            return Task.CompletedTask;
                        }
                    };
                    x.RequireHttpsMetadata = false;
                    x.SaveToken = true;
                    x.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey =
                            new SymmetricSecurityKey(Encoding.ASCII.GetBytes(Configuration["JWT_SECRET"])),
                        ValidateIssuer = false,
                        ValidateAudience = false
                    };
                }).AddApiKeyInHeader<ApiKeyProvider>(x =>
                {
                    x.Realm = "PROFFY";
                    x.KeyName = "X-API-KEY";
                });

            services.AddAuthorization(options =>
            {
                options.AddPolicy(AuthorizationPolicies.SuperAdmin,
                    policy => { policy.RequireClaim(ClaimTypes.Role, Role.SuperAdmin); });
                options.AddPolicy(AuthorizationPolicies.Admin,
                    policy => policy.RequireClaim(
                        ClaimTypes.Role,
                        Role.Admin,
                        Role.SuperAdmin)
                );
                options.AddPolicy(AuthorizationPolicies.User,
                    policy =>
                    {
                        policy.RequireClaim(
                            ClaimTypes.Role,
                            Role.User,
                            Role.Admin,
                            Role.SuperAdmin
                        );
                        policy.AuthenticationSchemes.Add(JwtBearerDefaults.AuthenticationScheme);
                    });
                options.AddPolicy(AuthorizationPolicies.RefreshToken, policy => policy.RequireClaim(ClaimTypes.Email));
                options.DefaultPolicy = options.GetPolicy(Role.SuperAdmin);
            });

            services.AddSingleton(new AuthService(Configuration["JWT_SECRET"]));

            services
                .AddControllers(config =>
                {
                    var policy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
                    config.Filters.Add(new AuthorizeFilter(policy));
                })
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment()) app.UseDeveloperExceptionPage();

            // app.UseHttpsRedirection();

            app.UseCors(
                x => x.AllowCredentials().AllowAnyHeader().AllowAnyMethod().WithOrigins("http://localhost:3000"));
            app.UseAuthentication();

            app.UseRouting();
            app.UseAuthorization();
            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}