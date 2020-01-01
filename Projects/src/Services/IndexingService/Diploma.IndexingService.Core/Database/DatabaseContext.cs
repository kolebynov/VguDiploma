using System;
using System.Linq;
using Diploma.IndexingService.Core.Objects;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Diploma.IndexingService.Core.Database
{
	internal class DatabaseContext : IdentityDbContext<User, IdentityRole<Guid>, Guid>
	{
		public DbSet<ContentStorageDbItem> Items { get; set; }

		public DbSet<InProgressDocumentDbItem> InProgressDocuments { get; set; }

		public DbSet<FolderDbItem> Folders { get; set; }

		public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
		{
		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);

			modelBuilder.Entity<ContentStorageDbItem>()
				.HasKey(x => new
				{
					x.Id,
					x.Category
				});

			var inProgressDocumentBuilder = modelBuilder.Entity<InProgressDocumentDbItem>();
			inProgressDocumentBuilder.HasKey(x => new { x.Id, x.UserIdentity });

			var folderBuilder = modelBuilder.Entity<FolderDbItem>();
			folderBuilder.HasKey(x => new { x.Id, x.UserIdentity });
			folderBuilder.Property(x => x.Name);
			folderBuilder.Property(x => x.ParentId);
			folderBuilder.Property(x => x.ParentsPath);
		}
	}
}
