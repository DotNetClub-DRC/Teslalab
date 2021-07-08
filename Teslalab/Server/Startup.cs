using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;
using Teslalab.Repositories;
using Teslalab.Server.Infrastructure;
using Teslalab.Server.Models.DataSeeding;
using Teslalab.Server.Models.Models;
using Teslalab.Server.Models.Profiles;
using Teslalab.Server.Services;
using Teslalab.Server.Services.Utilities;

namespace Teslalab.Server
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"), sqlOptions =>
                {
                    sqlOptions.MigrationsAssembly("Teslalab.Server");
                });
            });

            services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequiredLength = 5;
                options.Password.RequireLowercase = true;
            }).AddEntityFrameworkStores<ApplicationDbContext>()
              .AddDefaultTokenProviders(); // this will allow us to implement user roles

            services.AddAuthentication(auth =>
            {
                auth.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                auth.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidAudience = Configuration["AuthSettings:Audience"],
                    ValidIssuer = Configuration["AuthSettings:Issuer"],
                    RequireExpirationTime = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["AuthSettings:Key"])),
                    ValidateIssuerSigningKey = true
                };
            });

            services.AddScoped<IUnitOfWork, UnitOfWork>();

            services.AddScoped(sp => new AuthOptions
            {
                Audience = Configuration["AuthSettings:Audience"],
                Issuer = Configuration["AuthSettings:Issuer"],
                Key = Configuration["AuthSettings:Key"]
            });

            services.AddScoped<IUserService, UserService>();

            services.AddScoped(sp =>
            {
                var httpContext = sp.GetService<IHttpContextAccessor>().HttpContext;

                var identityOptions = new Teslalab.Server.Infrastructure.IdentityOptions();

                if (httpContext.User.Identity.IsAuthenticated)
                {
                    string id = httpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
                    string firstName = httpContext.User.FindFirst(ClaimTypes.GivenName).Value;
                    string lastName = httpContext.User.FindFirst(ClaimTypes.Surname).Value;
                    string email = httpContext.User.FindFirst(ClaimTypes.Email).Value;
                    string role = httpContext.User.FindFirst(ClaimTypes.Role).Value;

                    identityOptions.UserId = id;
                    identityOptions.Email = email;
                    identityOptions.FullName = $"{firstName} {lastName}";
                    identityOptions.IsAdmin = role == "Admin" ? true : false;
                }

                return identityOptions;
            });

            services.AddSingleton(sp =>
            {
                return new EnvironmentOptions()
                {
                    ApiUrl = Configuration["ApiUrl"]
                };
            });

            services.AddHttpContextAccessor();
            services.AddScoped<IPlaylistService, PlaylistService>();
            services.AddScoped<IVideoService, VideoService>();
            services.AddScoped<IFileStorageService, LocalFileStorageService>();
            services.AddScoped<ICommentsService, CommentsService>();

            services.AddSingleton(provider => new MapperConfiguration(config =>
            {
                config.AddProfile(new VideoProfile(provider.GetService<EnvironmentOptions>()));
            }).CreateMapper());

            //services.AddAutoMapper(typeof(VideoProfile));

            services.AddSwaggerGen();

            services.AddControllersWithViews();
            services.AddRazorPages();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, UserManager<ApplicationUser> userManager,
                              RoleManager<IdentityRole> roleManager)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseWebAssemblyDebugging();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production
                // scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            var dataSeeding = new UsersSeeding(userManager, roleManager);
            dataSeeding.SeedData().Wait();

            app.UseHttpsRedirection();
            app.UseBlazorFrameworkFiles();
            app.UseStaticFiles();

            app.UseSwagger();

            app.UseSwaggerUI(swagger =>
            {
                swagger.SwaggerEndpoint("/swagger/v1/swagger.json", "Teslalab API");
            });

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
                endpoints.MapControllers();
                endpoints.MapFallbackToFile("index.html");
            });
        }
    }
}