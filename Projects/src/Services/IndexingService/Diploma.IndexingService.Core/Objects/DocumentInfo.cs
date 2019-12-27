using System;
using System.Collections.Generic;
using Diploma.Shared.Interfaces;

namespace Diploma.IndexingService.Core.Objects
{
	public class DocumentInfo
	{
		public DocumentIdentity Id { get; }

		public string FileName { get; }

		public DateTimeOffset ModificationDate { get; }

		public IContent Content { get; }

		public FolderIdentity FolderId { get; }

		public IReadOnlyList<FolderIdentity> ParentFoldersPath { get; }

		public DocumentInfo(DocumentIdentity id, string fileName, DateTimeOffset modificationDate, IContent content,
			FolderIdentity folderId, IReadOnlyList<FolderIdentity> parentFoldersPath)
		{
			if (string.IsNullOrEmpty(fileName))
			{
				throw new ArgumentException("Value cannot be null or empty.", nameof(fileName));
			}

			Id = id ?? throw new ArgumentNullException(nameof(id));
			FileName = fileName;
			ModificationDate = modificationDate;
			Content = content ?? throw new ArgumentNullException(nameof(content));
			FolderId = folderId ?? throw new ArgumentNullException(nameof(folderId));
			ParentFoldersPath = parentFoldersPath ?? throw new ArgumentNullException(nameof(parentFoldersPath));
		}
	}
}
