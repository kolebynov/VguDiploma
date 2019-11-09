using System;
using Diploma.Shared.Interfaces;

namespace Diploma.IndexingService.Core.Objects
{
	public class ContentStorageItem
	{
		public string Id { get; }

		public string Category { get; }

		public IContent Content { get; }

		public DateTimeOffset Timestamp { get; }

		public ContentStorageItem(string id, string category, IContent content, DateTimeOffset timestamp)
		{
			Id = id ?? throw new ArgumentNullException(nameof(id));
			Category = category ?? throw new ArgumentNullException(nameof(category));
			Content = content ?? throw new ArgumentNullException(nameof(content));
			Timestamp = timestamp;
		}
	}
}
