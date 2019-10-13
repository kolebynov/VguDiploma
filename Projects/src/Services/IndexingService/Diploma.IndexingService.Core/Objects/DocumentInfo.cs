using System;

namespace Diploma.IndexingService.Core.Objects
{
	public class DocumentInfo
	{
		public DocumentIdentity Id { get; }

		public string FileName { get; }

		public DateTimeOffset ModificationDate { get; }

		public DocumentInfo(DocumentIdentity id, string fileName, DateTimeOffset modificationDate)
		{
			if (string.IsNullOrEmpty(fileName))
			{
				throw new ArgumentException("Value cannot be null or empty.", nameof(fileName));
			}

			Id = id ?? throw new ArgumentNullException(nameof(id));
			FileName = fileName;
			ModificationDate = modificationDate;
		}
	}
}
