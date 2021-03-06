﻿using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Diploma.IndexingService.Core.Objects;

namespace Diploma.IndexingService.Core.Interfaces
{
	public interface IInProgressDocumentsStorage
	{
		Task UpdateState(DocumentInfo document, InProcessDocumentState newState, CancellationToken cancellationToken);

		Task SetErrorState(DocumentInfo document, string errorInfo, CancellationToken cancellationToken);

		Task<IReadOnlyCollection<InProgressDocument>> GetInProgressDocuments(Guid userIdentity, int limit, int skip,
			CancellationToken cancellationToken);

		Task RemoveInProgressDocuments(
			IReadOnlyCollection<DocumentIdentity> documentIds,
			CancellationToken cancellationToken);

		Task RemoveAll(CancellationToken cancellationToken);
	}
}
