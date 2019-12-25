using Microsoft.EntityFrameworkCore;

namespace Diploma.IndexingService.Core.Database
{
	internal class DatabaseContext : DbContext
	{
		public DbSet<ContentStorageDbItem> Items { get; set; }

		public DbSet<InProgressDocumentDbItem> InProgressDocuments { get; set; }

		public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
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

			var inProgressDocumentBuilder = modelBuilder.Entity<InProgressDocumentDbItem>();
			inProgressDocumentBuilder.HasKey(x => new { x.Id, x.UserIdentity });
		}
	}
}
