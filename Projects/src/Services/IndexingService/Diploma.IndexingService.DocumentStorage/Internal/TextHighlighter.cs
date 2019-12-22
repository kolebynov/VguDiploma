using System;
using System.Collections.Generic;
using Diploma.IndexingService.Core.Objects;
using Diploma.IndexingService.EsDocumentStorage.Interfaces;

namespace Diploma.IndexingService.EsDocumentStorage.Internal
{
	internal class TextHighlighter : ITextHighlighter
	{
		public string EscapeText(string text) => text;

		public IReadOnlyCollection<DocumentTextEntry> ParseHighlightedText(string text)
		{
			var list = new List<DocumentTextEntry>();

			var textSpan = text.AsSpan();
			int firstIndex;
			while ((firstIndex = textSpan.IndexOf("<em>")) > -1)
			{
				if (firstIndex > 0)
				{
					list.Add(new DocumentTextEntry
					{
						TextType = TextType.Text,
						Text = textSpan.Slice(0, firstIndex).ToString()
					});
				}

				var lastIndex = textSpan.IndexOf("</em>");
				if (lastIndex > firstIndex + 4)
				{
					list.Add(new DocumentTextEntry
					{
						TextType = TextType.HighlightedText,
						Text = textSpan.Slice(firstIndex + 4, lastIndex - firstIndex - 4).ToString()
					});
				}

				textSpan = textSpan.Slice(lastIndex + 5);
			}

			if (!textSpan.IsEmpty)
			{
				list.Add(new DocumentTextEntry
				{
					TextType = TextType.Text,
					Text = textSpan.ToString()
				});
			}

			return list;
		}
	}
}
