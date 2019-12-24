using Diploma.IndexingService.Core.Objects;
using Microsoft.EntityFrameworkCore;

namespace Diploma.IndexingService.Core.Database
{
	internal class DatabaseContext : DbContext
	{
		public DbSet<ContentStorageDbItem> Items { get; set; }

		public DbSet<InProgressDocument> InProgressDocuments { get; set; }

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

			var inProgressDocumentBuilder = modelBuilder.Entity<InProgressDocument>();
			inProgressDocumentBuilder
				.Property(x => x.Id)
				.HasConversion(
					id => id.ToString(),
					idStr => DocumentIdentity.FromString(idStr));
			inProgressDocumentBuilder.HasKey(x => x.Id);
			inProgressDocumentBuilder.Property(x => x.FileName);
			inProgressDocumentBuilder.Property(x => x.State);
			inProgressDocumentBuilder.Property(x => x.ErrorInfo);
		}
	}
}
