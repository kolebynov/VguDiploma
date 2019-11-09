using Microsoft.EntityFrameworkCore;

namespace Diploma.IndexingService.Core.Internal.ContentStorage
{
	internal class ContentStorageContext : DbContext
	{
		public DbSet<ContentStorageDbItem> Items { get; set; }

		public ContentStorageContext(DbContextOptions<ContentStorageContext> options) : base(options)
		{
		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);

			modelBuilder.Entity<ContentStorageDbItem>()
				.HasKey(x => new
				{
					x.Id, x.Category
				});
		}
	}
}
