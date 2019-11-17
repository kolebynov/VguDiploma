using System;
using System.Collections.Generic;
using Diploma.IndexingService.Core.Objects;

namespace Diploma.IndexingService.Core.Commands
{
	public class AddDocumentsResult
	{
		public IReadOnlyDictionary<DocumentIdentity, InProcessDocumentState> States { get; }

		public AddDocumentsResult(IReadOnlyDictionary<DocumentIdentity, InProcessDocumentState> states)
		{
			States = states ?? throw new ArgumentNullException(nameof(states));
		}
	}
}
