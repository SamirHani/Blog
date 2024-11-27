using Microsoft.EntityFrameworkCore;
using Blog.Context;
using Blog.Models;
using Microsoft.AspNetCore.Identity;
using Blog.Repositories;
using Blog.Services;
namespace Blog
{
	public class Program
	{
		public static void Main(string[] args)
		{
			var builder = WebApplication.CreateBuilder(args);

			// Add services to the container.
			builder.Services.AddControllersWithViews();

			// services that i made
			builder.Services.AddScoped<AccountRepository>();
			builder.Services.AddScoped<AccountService>();
			builder.Services.AddScoped<ProfileService>();
			builder.Services.AddScoped<ProfileRepository>();
			builder.Services.AddScoped<PostService>();
			// context
			builder.Services.AddDbContext<ApplicationDbContext>(options =>
				options.UseSqlServer(builder.Configuration.GetConnectionString("Local")));

			// sessions
			builder.Services.AddSession(options =>
			{
				options.IdleTimeout = TimeSpan.FromMinutes(30);
			});

			// all identity services
			builder.Services.AddIdentity<ApplicationUser, IdentityRole>(
			option =>
			{
				option.Password.RequireDigit = false;
				option.Password.RequireLowercase = false;
				option.Password.RequireNonAlphanumeric = false;
				option.Password.RequireUppercase = false;
				option.User.RequireUniqueEmail = true;
			}).AddEntityFrameworkStores<ApplicationDbContext>()
			  .AddDefaultTokenProviders();

			builder.Services.AddScoped<UserManager<ApplicationUser>>();



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

			app.UseAuthorization();

			app.MapControllerRoute(
				name: "default",
				pattern: "{controller=Feed}/{action=Index}/{id?}");

			app.Run();
		}
	}
}
