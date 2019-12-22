using System.Collections.Generic;
using Diploma.IndexingService.Core.Objects;

namespace Diploma.IndexingService.EsDocumentStorage.Interfaces
{
	internal interface ITextHighlighter
	{
		string EscapeText(string text);

		IReadOnlyCollection<DocumentTextEntry> ParseHighlightedText(string text);
	}
}
