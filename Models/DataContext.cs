using Microsoft.EntityFrameworkCore;

namespace WebApp.Models
{
	public class DataContext(DbContextOptions<DataContext> opts) : DbContext(opts)
	{
		public DbSet<Product> Products { get; set; }

		public DbSet<Category> Categories { get; set; }

		public DbSet<Supplier> Suppliers { get; set; }
	}
}