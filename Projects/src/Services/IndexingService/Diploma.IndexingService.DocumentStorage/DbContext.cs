using Diploma.IndexingService.DocumentStorage.Models;
using Microsoft.EntityFrameworkCore;

namespace Diploma.IndexingService.DocumentStorage
{
	internal class DbContext : Microsoft.EntityFrameworkCore.DbContext
	{
		public DbSet<DocumentInfo> Documents { get; set; }

		public DbContext(DbContextOptions<DbContext> options) : base(options)
		{
		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);

			modelBuilder
				.Entity<DocumentInfo>()
				.HasKey(x => new { x.Id, x.UserIdentity });
		}
	}
}
