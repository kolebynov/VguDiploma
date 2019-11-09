using System;
using Diploma.Shared.Interfaces;

namespace Diploma.IndexingService.Core.Objects
{
	public class FullDocumentInfo : DocumentInfo
	{
		public string ExtractedText { get; }

		public FullDocumentInfo(DocumentInfo baseDocument, string extractedText) 
			: this(baseDocument.Id, baseDocument.FileName, baseDocument.ModificationDate, baseDocument.Content, extractedText)
		{
		}

		public FullDocumentInfo(DocumentIdentity id, string fileName, DateTimeOffset modificationDate, IContent content, string extractedText) 
			: base(id, fileName, modificationDate, content)
		{
			if (string.IsNullOrEmpty(extractedText))
			{
				throw new ArgumentException("Value cannot be null or empty.", nameof(extractedText));
			}

			ExtractedText = extractedText;
		}
	}
}
