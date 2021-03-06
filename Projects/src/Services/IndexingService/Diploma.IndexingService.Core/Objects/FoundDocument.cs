﻿using System.Collections.Generic;

namespace Diploma.IndexingService.Core.Objects
{
	public class FoundDocument
	{
		public DocumentIdentity DocumentId { get; set; }

		public string FileName { get; set; }

		public IReadOnlyDictionary<string, IReadOnlyCollection<IReadOnlyCollection<DocumentTextEntry>>> Matches { get; set; }
	}
}
