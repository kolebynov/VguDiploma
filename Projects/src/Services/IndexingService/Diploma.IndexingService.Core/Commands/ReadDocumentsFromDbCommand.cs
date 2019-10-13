using System;
using System.Collections.Generic;
using Diploma.IndexingService.Core.Objects;
using MediatR;

namespace Diploma.IndexingService.Core.Commands
{
	internal class ReadDocumentsFromDbCommand : IRequest<ReadDocumentsFromDbResult>
	{
		public IReadOnlyCollection<DocumentIdentity> DocumentIds { get; }

		public ReadDocumentsFromDbCommand(IReadOnlyCollection<DocumentIdentity> documentIds)
		{
			DocumentIds = documentIds ?? throw new ArgumentNullException(nameof(documentIds));
		}
	}
}
