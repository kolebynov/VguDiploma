using System;

namespace Diploma.IndexingService.Core.Objects
{
	public class DocumentInfo
	{
		public string Id { get; }

		public string FileName { get; }

		public DateTimeOffset ModificationDate { get; }

		public string UserIdentity { get; }

		public DocumentInfo(string id, string fileName, DateTimeOffset modificationDate, string userIdentity)
		{
			if (string.IsNullOrEmpty(fileName))
			{
				throw new ArgumentException("Value cannot be null or empty.", nameof(fileName));
			}

			if (string.IsNullOrEmpty(id))
			{
				throw new ArgumentException("Value cannot be null or empty.", nameof(id));
			}

			if (string.IsNullOrEmpty(userIdentity))
			{
				throw new ArgumentException("Value cannot be null or empty.", nameof(userIdentity));
			}

			Id = id;
			FileName = fileName;
			ModificationDate = modificationDate;
			UserIdentity = userIdentity;
		}
	}
}
