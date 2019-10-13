using System;
using System.Collections.Generic;
using Diploma.IndexingService.Core.Objects;
using MediatR;

namespace Diploma.IndexingService.Core.Commands
{
	internal class SaveDocumentsToDbCommand : IRequest<SaveDocumentsToDbResult>
	{
		public IReadOnlyCollection<ProcessedDocument> Documents { get; }

		public SaveDocumentsToDbCommand(IReadOnlyCollection<ProcessedDocument> documents)
		{
			Documents = documents ?? throw new ArgumentNullException(nameof(documents));
		}
	}
}
