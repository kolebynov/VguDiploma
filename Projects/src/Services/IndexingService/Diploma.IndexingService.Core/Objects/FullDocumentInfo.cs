using System;
using System.Collections.Generic;
using Diploma.Shared.Interfaces;

namespace Diploma.IndexingService.Core.Objects
{
	public class FullDocumentInfo : DocumentInfo
	{
		public string ExtractedText { get; }

		public FullDocumentInfo(DocumentInfo baseDocument, string extractedText) 
			: this(baseDocument.Id, baseDocument.FileName, baseDocument.ModificationDate, baseDocument.Content,
				baseDocument.FolderId, baseDocument.ParentFoldersPath, extractedText)
		{
		}

		public FullDocumentInfo(DocumentIdentity id, string fileName, DateTimeOffset modificationDate, IContent content,
			FolderIdentity folderId, IReadOnlyList<FolderIdentity> parentFoldersPath, string extractedText) 
			: base(id, fileName, modificationDate, content, folderId, parentFoldersPath)
		{
			if (string.IsNullOrEmpty(extractedText))
			{
				throw new ArgumentException("Value cannot be null or empty.", nameof(extractedText));
			}

			ExtractedText = extractedText;
		}
	}
}
