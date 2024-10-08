using Capstone.Context;
using Capstone.Helpers;
using Capstone.Models.Spotify;
using Capstone.Services;
using Capstone.Services.Auth;
using Capstone.Services.Interfaces;
using Capstone.Services.Interfaces.Auth;
using Capstone.Services.Master;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Stripe;
using System.Security.Claims;

namespace Capstone
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            //Connection Data Context
            var conn = builder.Configuration.GetConnectionString("Db");
            builder.Services.AddDbContext<DataContext>(opt => opt.UseSqlServer(conn));

            // Auth
            builder.Services
                .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.LoginPath = "/Auth/Login";
                    options.AccessDeniedPath = "/Home/Index";
                    options.ExpireTimeSpan = TimeSpan.FromHours(1);
                });

            // Policies
            builder.Services.AddAuthorization(options =>
            {
                // Master Policy
                options.AddPolicy("MasterPolicy", policy =>
                {
                    policy.RequireClaim(ClaimTypes.Role, "master"); // [Authorize(Policy = "MasterPolicy")]
                });
                // Admin Policy
                options.AddPolicy("AdminPolicy", policy =>
                {
                    policy.RequireClaim(ClaimTypes.Role, "admin"); // [Authorize(Policy = "AdminPolicy")]
                });
                options.AddPolicy("AdminOrMasterPolicy", policy =>
                {
                    policy.RequireClaim(ClaimTypes.Role, "admin", "master");
                });
            });

            //Stripe Settings Configuration
            StripeConfiguration.ApiKey = builder.Configuration["Stripe:SecretKey"];

            // Spotify Settings Configuration
            builder.Services.Configure<SpotifySettings>(builder.Configuration.GetSection("Spotify"));

            // Add HttpClient for API requests
            builder.Services.AddHttpClient();

            // Register SpotifyAuthService as a singleton
            builder.Services.AddSingleton<SpotifyService>();


            // Services
            builder.Services
                .AddScoped<IAuthService, AuthService>()
                .AddScoped<IUserService, UserService>()
                .AddScoped<IRoleService, RoleService>()
                .AddScoped<IMasterService, MasterService>()
                .AddScoped<IAdminService, AdminService>()
                .AddScoped<IEventService, Services.EventService>()
                .AddScoped<IGenreService, GenreService>()
                .AddScoped<ILocationService, LocationService>()
                .AddScoped<IDjService, DjService>()
                .AddScoped<IReviewService, Services.ReviewService>()
                .AddScoped<ICommentService, CommentService>()
                .AddScoped<ICommentLikeService, CommentLikeService>()
                .AddScoped<ITicketTypeService, TicketTypeService>()
                .AddScoped<ICartService, CartService>()
                .AddScoped<IPasswordHelper, PasswordHelper>()
                .AddScoped<ISpotifyService, SpotifyService>()
                .AddScoped<IQrCodeService, QRCodeService>()
                .AddScoped<IEmailService, EmailService>();

            builder.Services.AddHttpContextAccessor();
            // Add services to the container.
            builder.Services.AddControllersWithViews();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
