using Microsoft.EntityFrameworkCore;

namespace WebApp.Models
{
	public static class SeedData
	{
		public static void SeedDatabase(DataContext context)
		{
			context.Database.Migrate();
			if (!context.Products.Any() && !context.Suppliers.Any() && !context.Categories.Any())
			{
				Supplier s1 = new() { Name = "Splash Dudes", City = "San Jose" };
				Supplier s2 = new() { Name = "Soccer Town", City = "Chicago" };
				Supplier s3 = new() { Name = "Chess Co", City = "New York" };

				Category c1 = new() { Name = "Watersports" };
				Category c2 = new() { Name = "Soccer" };
				Category c3 = new() { Name = "Chess" };

				context.Products.AddRange(
					new Product
					{
						Name = "Kayak",
						Price = 275,
						Category = c1,
						Supplier = s1
					},
					new Product
					{
						Name = "Lifejacket",
						Price = 48.95m,
						Category = c1,
						Supplier = s1
					},
					new Product
					{
						Name = "Soccer Ball",
						Price = 19.50m,
						Category = c2,
						Supplier = s2
					},
					new Product
					{
						Name = "Corner Flags",
						Price = 34.95m,
						Category = c2,
						Supplier = s2
					},
					new Product
					{
						Name = "Stadium",
						Price = 79500,
						Category = c2,
						Supplier = s2
					},
					new Product
					{
						Name = "Thinking Cap",
						Price = 16,
						Category = c3,
						Supplier = s3
					},
					new Product
					{
						Name = "Unsteady Chair",
						Price = 29.95m,
						Category = c3,
						Supplier = s3
					},
					new Product
					{
						Name = "Human Chess Board",
						Price = 75,
						Category = c3,
						Supplier = s3
					},
					new Product
					{
						Name = "Bling-Bling King",
						Price = 1200,
						Category = c3,
						Supplier = s3
					}
				);
				context.SaveChanges();
			}
		}
	}
}