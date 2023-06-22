using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using FamilyRealtor.Helpers;
using FamilyRealtor.Models;
using FamilyRealtor.Models.DomainModels;
using FamilyRealtor.Models.SeedData;

namespace FamilyRealtor.Data
{
    public class RealtorContext : IdentityDbContext<User>
	{
		public RealtorContext(DbContextOptions<RealtorContext> options) : base(options)
		{
		}

		public DbSet<Rental> Rentals { get; set; }
		public DbSet<Photo> Photos { get; set; }
		public DbSet<Booking> Bookings { get; set; }
		public DbSet<Client> Clients { get; set; }
		public DbSet<Review> Reviews { get; set; }
		public DbSet<Category> Categories { get; set; }
		public DbSet<Facility> Facilities { get; set; }
		public DbSet<Country> Countries { get; set; }
		public DbSet<City> Cities { get; set; }

		protected override void OnModelCreating(ModelBuilder builder)
		{
			builder.Entity<Client>()
				.HasOne(c => c.User)
				.WithOne(u => u.Client)
				.HasForeignKey<User>(u => u.ClientId);

			builder.Entity<Rental>()
				.Property(r => r.TimeModified)
				.HasDefaultValueSql("getdate() at time zone 'Russian Standard Time'");

			builder.ApplyConfiguration(new SeedCategories());
			builder.ApplyConfiguration(new SeedFacilities());
			builder.ApplyConfiguration(new SeedCountries());
			builder.ApplyConfiguration(new SeedCities());

			base.OnModelCreating(builder);
		}

		protected override void ConfigureConventions(ModelConfigurationBuilder builder)
		{
			builder.Properties<DateOnly>()
				.HaveConversion<DateOnlyConverter>()
				.HaveColumnType("date");

			base.ConfigureConventions(builder);
		}

		public static async Task CreateAdminUser(IServiceProvider serviceProvider)
		{
			UserManager<User> userManager = serviceProvider.GetRequiredService<UserManager<User>>();
			RoleManager<IdentityRole> roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

			string email = "admin@family.real";
			string password = "uTgC2Gv";
			string roleName = "администратор";

			if (await roleManager.FindByNameAsync(roleName) == null) await roleManager.CreateAsync(new IdentityRole(roleName));

			if (await userManager.FindByEmailAsync(email) == null)
			{
				User user = new() { UserName = email, Email = email };
				IdentityResult result = await userManager.CreateAsync(user, password);
				if (result.Succeeded) await userManager.AddToRoleAsync(user, roleName);
			}
		}

		public static async Task CreateRealtorUser(IServiceProvider serviceProvider)
		{
			UserManager<User> userManager = serviceProvider.GetRequiredService<UserManager<User>>();
			RoleManager<IdentityRole> roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

			string email = "realtor@family.real";
			string password = "CeX4Peg";
			string roleName = "риэлтор";

			if (await roleManager.FindByNameAsync(roleName) == null) await roleManager.CreateAsync(new IdentityRole(roleName));

			if (await userManager.FindByEmailAsync(email) == null)
			{
				User user = new() { UserName = email, Email = email };
				IdentityResult result = await userManager.CreateAsync(user, password);
				if (result.Succeeded) await userManager.AddToRoleAsync(user, roleName);
			}
		}
	}
}