using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Diploma.IndexingService.Core.Configuration;
using Diploma.IndexingService.Core.Exceptions;
using Diploma.IndexingService.Core.Interfaces;
using Diploma.IndexingService.Core.Objects;
using Diploma.Shared.Extensions;
using Diploma.Shared.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Diploma.IndexingService.Core.Internal.ContentStorage
{
	internal class EfContentStorage : IContentStorage
	{
		private readonly ContentStorageContext context;
		private readonly ContentStorageOptions options;

		public EfContentStorage(ContentStorageContext context, IOptions<ContentStorageOptions> options)
		{
			this.context = context ?? throw new ArgumentNullException(nameof(context));
			this.options = options?.Value ?? throw new ArgumentNullException(nameof(options));
		}

		public async Task<ContentStorageItem> Save(string id, string category, IContent content,
			CancellationToken cancellationToken)
		{
			var dbItem = await Find(id, category, cancellationToken);
			if (dbItem == null)
			{
				dbItem = new ContentStorageDbItem
				{
					Id = id,
					Category = category,
				};
				context.Add(dbItem);
			}
			else
			{
				context.Update(dbItem);
			}

			dbItem.Timestamp = DateTimeOffset.UtcNow;
			dbItem.Content = await content.ReadAsByteArray(cancellationToken);
			await context.SaveChangesAsync(cancellationToken);

			return new ContentStorageItem(id, category, content, dbItem.Timestamp);
		}

		public async Task<ContentStorageItem> Get(string id, string category, CancellationToken cancellationToken)
		{
			var dbItem = (await Find(id, category, cancellationToken))
				?? throw new ContentNotFoundException($"Content with id {id} and category {category} not found");

			return new ContentStorageItem(id, category, new ByteArrayContent(dbItem.Content), dbItem.Timestamp);
		}

		public async Task Delete(string id, string category, CancellationToken cancellationToken)
		{
			var dbItem = await Find(id, category, cancellationToken);
			if (dbItem != null)
			{
				context.Items.Remove(dbItem);
			}

			await context.SaveChangesAsync(cancellationToken);
		}

		public async IAsyncEnumerable<IReadOnlyCollection<ContentStorageItem>> GetAll(
			string category,
			[EnumeratorCancellation] CancellationToken cancellationToken)
		{
			if (string.IsNullOrEmpty(category))
			{
				throw new ArgumentException("Value cannot be null or empty.", nameof(category));
			}

			var skip = 0;
			ContentStorageItem[] page;
			do
			{
				cancellationToken.ThrowIfCancellationRequested();

				page = (await context.Items.AsNoTracking()
					.Where(x => x.Category == category)
					.Select(x => new
					{
						x.Id, x.Category, x.Timestamp
					})
					.Skip(skip)
					.Take(options.PageSize)
					.ToArrayAsync(cancellationToken))
					.Select(x => new ContentStorageItem(
						x.Id, x.Category,
						new LazyContent(async () => new ByteArrayContent((await Find(x.Id, x.Category, CancellationToken.None)).Content)),
						x.Timestamp))
					.ToArray();

				if (!page.Any())
				{
					yield break;
				}

				yield return page;
				skip += options.PageSize;
			}
			while (page.Length == options.PageSize);
		}

		private ValueTask<ContentStorageDbItem> Find(string id, string category,
			CancellationToken cancellationToken)
		{
			if (string.IsNullOrEmpty(id))
			{
				throw new ArgumentException("Value cannot be null or empty.", nameof(id));
			}

			if (string.IsNullOrEmpty(category))
			{
				throw new ArgumentException("Value cannot be null or empty.", nameof(category));
			}

			return context.Items.FindAsync(new object[] { id, category }, cancellationToken);
		}
	}
}
