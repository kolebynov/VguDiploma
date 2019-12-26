using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Diploma.IndexingService.Core.Configuration;
using Diploma.IndexingService.Core.Database;
using Diploma.IndexingService.Core.Exceptions;
using Diploma.IndexingService.Core.Interfaces;
using Diploma.IndexingService.Core.Objects;
using Diploma.Shared.Extensions;
using Diploma.Shared.Interfaces;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Diploma.IndexingService.Core.Internal
{
	internal class EfContentStorage : IContentStorage
	{
		private static readonly Expression<Func<ContentStorageDbItem, ContentStorageDbItem>> LoadItemExpression =
			x => new ContentStorageDbItem
			{
				Id = x.Id,
				Category = x.Category,
				Size = x.Size,
				Timestamp = x.Timestamp
			};

		private readonly DatabaseContext context;
		private readonly ContentStorageOptions options;
		private readonly IServiceScopeFactory serviceScopeFactory;

		public EfContentStorage(DatabaseContext context, IOptions<ContentStorageOptions> options, IServiceScopeFactory serviceScopeFactory)
		{
			this.context = context ?? throw new ArgumentNullException(nameof(context));
			this.serviceScopeFactory = serviceScopeFactory ?? throw new ArgumentNullException(nameof(serviceScopeFactory));
			this.options = options?.Value ?? throw new ArgumentNullException(nameof(options));
		}

		public async Task<ContentStorageItem> Save(string id, string category, IContent content,
			CancellationToken cancellationToken)
		{
			var dbItem = await FindWithoutContent(id, category, cancellationToken);
			if (dbItem == null)
			{
				dbItem = new ContentStorageDbItem
				{
					Id = id,
					Category = category
				};
				context.Items.Add(dbItem);
			}
			else
			{
				try
				{
					context.Items.Update(dbItem);
				}
				catch (InvalidOperationException)
				{
					dbItem = context.Items.Find(id, category);
					context.Items.Update(dbItem);
				}
			}

			dbItem.Timestamp = DateTimeOffset.UtcNow;
			dbItem.Content = await content.ReadAsByteArray(cancellationToken);
			dbItem.Size = content.Size;
			await context.SaveChangesAsync(cancellationToken);

			return new ContentStorageItem(id, category, content, dbItem.Timestamp);
		}

		public async Task<ContentStorageItem> Get(string id, string category, CancellationToken cancellationToken)
		{
			var dbItem = (await FindWithoutContent(id, category, cancellationToken))
				?? throw new ContentNotFoundException($"Content with id {id} and category {category} not found");

			return new ContentStorageItem(id, category, GetItemContent(dbItem), dbItem.Timestamp);
		}

		public async Task Delete(string id, string category, CancellationToken cancellationToken)
		{
			var dbItem = await FindWithoutContent(id, category, cancellationToken);
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
					.Select(LoadItemExpression)
					.Skip(skip)
					.Take(options.PageSize)
					.ToArrayAsync(cancellationToken))
					.Select(x => new ContentStorageItem(
						x.Id, x.Category,
						GetItemContent(x),
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

		private Task<ContentStorageDbItem> FindWithoutContent(string id, string category, CancellationToken cancellationToken)
		{
			CheckParams(id, category);

			return context.Items
				.Where(x => x.Id == id && x.Category == category)
				.Select(LoadItemExpression)
				.FirstOrDefaultAsync(cancellationToken);
		}

		private ContentStorageItemContent GetItemContent(ContentStorageDbItem dbItem) =>
			new ContentStorageItemContent(dbItem, serviceScopeFactory);

		private static void CheckParams(string id, string category)
		{
			if (string.IsNullOrEmpty(id))
			{
				throw new ArgumentException("Value cannot be null or empty.", nameof(id));
			}

			if (string.IsNullOrEmpty(category))
			{
				throw new ArgumentException("Value cannot be null or empty.", nameof(category));
			}
		}

		private class ContentStorageItemContent : IContent
		{
			private readonly ContentStorageDbItem dbItem;
			private readonly IServiceScopeFactory serviceScopeFactory;

			public IContentHash Hash => throw new InvalidOperationException();

			public long Size => dbItem.Size;

			public ContentStorageItemContent(ContentStorageDbItem dbItem, IServiceScopeFactory serviceScopeFactory)
			{
				this.dbItem = dbItem ?? throw new ArgumentNullException(nameof(dbItem));
				this.serviceScopeFactory = serviceScopeFactory ?? throw new ArgumentNullException(nameof(serviceScopeFactory));
			}

			public async Task<Stream> OpenReadStream(CancellationToken cancellationToken)
			{
				var scope = serviceScopeFactory.CreateScope();
				var context = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
				await using var command = context.Database.GetDbConnection().CreateCommand();
				command.CommandText = $"select {nameof(ContentStorageDbItem.Content)} from {nameof(DatabaseContext.Items)} " +
					$"where {nameof(ContentStorageDbItem.Id)}=@id and {nameof(ContentStorageDbItem.Category)}=@category";
				command.Parameters.Add(new SqlParameter("@id", SqlDbType.NVarChar) { Value = dbItem.Id });
				command.Parameters.Add(new SqlParameter("@category", SqlDbType.NVarChar) { Value = dbItem.Category });

				DbDataReader reader = null;

				try
				{
					if (command.Connection.State != ConnectionState.Open)
					{
						await command.Connection.OpenAsync(cancellationToken);
					}

					reader = await command.ExecuteReaderAsync(CommandBehavior.SequentialAccess, cancellationToken);
					reader.Read();
					var stream = reader.GetStream(0);
					return new DatabaseColumnStream(stream, reader, context, scope);
				}
				catch
				{
					if (reader != null)
					{
						await reader.DisposeAsync();
					}

					await command.Connection.CloseAsync();

					throw;
				}
			}
		}

		private class DatabaseColumnStream : Stream
		{
			private readonly Stream columnStream;
			private readonly DbDataReader reader;
			private readonly DatabaseContext context;
			private readonly IServiceScope scope;

			public override bool CanRead => columnStream.CanRead;

			public override bool CanSeek => columnStream.CanSeek;

			public override bool CanWrite => columnStream.CanWrite;

			public override long Length => columnStream.Length;

			public override long Position
			{
				get => columnStream.Position;
				set => columnStream.Position = value;
			}

			public override bool CanTimeout => columnStream.CanTimeout;

			public override int ReadTimeout
			{
				get => columnStream.ReadTimeout;
				set => columnStream.ReadTimeout = value;
			}

			public override int WriteTimeout
			{
				get => columnStream.WriteTimeout;
				set => columnStream.WriteTimeout = value;
			}

			public DatabaseColumnStream(Stream columnStream, DbDataReader reader, DatabaseContext context, IServiceScope scope)
			{
				this.columnStream = columnStream ?? throw new ArgumentNullException(nameof(columnStream));
				this.reader = reader ?? throw new ArgumentNullException(nameof(reader));
				this.context = context ?? throw new ArgumentNullException(nameof(context));
				this.scope = scope ?? throw new ArgumentNullException(nameof(scope));
			}

			public override void Flush() => columnStream.Flush();

			public override int Read(byte[] buffer, int offset, int count) => columnStream.Read(buffer, offset, count);

			public override long Seek(long offset, SeekOrigin origin) => columnStream.Seek(offset, origin);

			public override void SetLength(long value) => columnStream.SetLength(value);

			public override void Write(byte[] buffer, int offset, int count) => columnStream.Write(buffer, offset, count);

			public override IAsyncResult BeginRead(byte[] buffer, int offset, int count, AsyncCallback callback, object state) =>
				columnStream.BeginRead(buffer, offset, count, callback, state);

			public override IAsyncResult BeginWrite(byte[] buffer, int offset, int count, AsyncCallback callback, object state) => 
				columnStream.BeginWrite(buffer, offset, count, callback, state);

			public override void Close() => columnStream.Close();

			public override void CopyTo(Stream destination, int bufferSize) => columnStream.CopyTo(destination, bufferSize);

			public override Task CopyToAsync(Stream destination, int bufferSize, CancellationToken cancellationToken)
			{
				return columnStream.CopyToAsync(destination, bufferSize, cancellationToken);
			}

			public override async ValueTask DisposeAsync()
			{
				await base.DisposeAsync();
				await columnStream.DisposeAsync();
				await reader.DisposeAsync();
				await context.DisposeAsync();
				scope.Dispose();
			}

			public override int EndRead(IAsyncResult asyncResult) => columnStream.EndRead(asyncResult);

			public override void EndWrite(IAsyncResult asyncResult) => columnStream.EndWrite(asyncResult);

			public override Task FlushAsync(CancellationToken cancellationToken) => columnStream.FlushAsync(cancellationToken);

			public override int Read(Span<byte> buffer) => columnStream.Read(buffer);

			public override Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken) => 
				columnStream.ReadAsync(buffer, offset, count, cancellationToken);

			public override ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = new CancellationToken()) => 
				columnStream.ReadAsync(buffer, cancellationToken);

			public override int ReadByte() => columnStream.ReadByte();

			public override void Write(ReadOnlySpan<byte> buffer) => columnStream.Write(buffer);

			public override Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken) => 
				columnStream.WriteAsync(buffer, offset, count, cancellationToken);

			public override ValueTask WriteAsync(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken = new CancellationToken()) => 
				columnStream.WriteAsync(buffer, cancellationToken);

			public override void WriteByte(byte value) => columnStream.WriteByte(value);

			public override object InitializeLifetimeService() => columnStream.InitializeLifetimeService();
		}
	}
}
