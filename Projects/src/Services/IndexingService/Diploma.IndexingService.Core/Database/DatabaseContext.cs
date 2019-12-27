using System;
using System.Linq;
using Diploma.IndexingService.Core.Objects;
using Microsoft.EntityFrameworkCore;

namespace Diploma.IndexingService.Core.Database
{
	internal class DatabaseContext : DbContext
	{
		public DbSet<ContentStorageDbItem> Items { get; set; }

		public DbSet<InProgressDocumentDbItem> InProgressDocuments { get; set; }

		public DbSet<Folder> Folders { get; set; }

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

			var folderBuilder = modelBuilder.Entity<Folder>();
			folderBuilder.HasKey(x => x.Id);
			folderBuilder.Property(x => x.Id)
				.HasConversion(x => x.ToString(), x => FolderIdentity.FromString(x));
			folderBuilder.Property(x => x.ParentId)
				.HasConversion(x => x.ToString(), x => FolderIdentity.FromString(x));
			folderBuilder.Property(x => x.Name);
			folderBuilder.Property(x => x.ParentsPath)
				.HasConversion(
					x => string.Join(",", x.Select(y => y.ToString())),
					x => x.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(FolderIdentity.FromString).ToList());
		}
	}
}
