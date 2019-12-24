using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Diploma.IndexingService.Core.Database
{
	public class ResilientTransaction
	{
		private readonly DbContext context;

		private ResilientTransaction(DbContext context) =>
			this.context = context ?? throw new ArgumentNullException(nameof(context));

		public static ResilientTransaction New (DbContext context) =>
			new ResilientTransaction(context);

		public async Task ExecuteAsync(Func<Task> action)
		{
			var strategy = context.Database.CreateExecutionStrategy();
			await strategy.ExecuteAsync(async () =>
			{
				await using var transaction = context.Database.BeginTransaction();
				await action();
				transaction.Commit();
			});
		}
	}
}
