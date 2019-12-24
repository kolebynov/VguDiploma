using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Diploma.IndexingService.Core.Database;
using Diploma.IndexingService.Core.Events;
using Diploma.IndexingService.Core.Interfaces;
using Diploma.IndexingService.Core.Objects;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Diploma.IndexingService.Core.Internal
{
	internal class EfInProgressDocumentsStorage : IInProgressDocumentsStorage
	{
		private readonly DatabaseContext context;
		private readonly IMediator mediator;

		public EfInProgressDocumentsStorage(DatabaseContext context, IMediator mediator)
		{
			this.context = context ?? throw new ArgumentNullException(nameof(context));
			this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
		}

		public Task UpdateState(DocumentInfo document, InProcessDocumentState newState, CancellationToken cancellationToken)
		{
			return UpdateStateInternal(document, newState, null, cancellationToken);
		}

		public Task SetErrorState(DocumentInfo document, string errorInfo, CancellationToken cancellationToken)
		{
			return UpdateStateInternal(document, InProcessDocumentState.Error, errorInfo, cancellationToken);
		}

		public Task<IReadOnlyCollection<InProgressDocument>> GetInProgressDocuments(int limit, int skip, CancellationToken cancellationToken)
		{
			throw new NotImplementedException();
		}

		private async Task UpdateStateInternal(DocumentInfo document, InProcessDocumentState newState, string errorInfo,
			CancellationToken cancellationToken)
		{
			InProgressDocument inProgressDocument = null;
			await ResilientTransaction.New(context).ExecuteAsync(async () =>
			{
				inProgressDocument =
					await context.InProgressDocuments.FindAsync(new object[] { document.Id }, cancellationToken);
				if (inProgressDocument != null)
				{
					if (newState == InProcessDocumentState.Done)
					{
						context.InProgressDocuments.Remove(inProgressDocument);
						inProgressDocument = new InProgressDocument(inProgressDocument.Id, inProgressDocument.FileName, InProcessDocumentState.Done);
					}
					else if (inProgressDocument.State < newState)
					{
						context.InProgressDocuments.Attach(inProgressDocument).State = EntityState.Detached;
						inProgressDocument = new InProgressDocument(inProgressDocument.Id,
							inProgressDocument.FileName, newState, errorInfo);
						context.InProgressDocuments.Update(inProgressDocument);
					}
					else if (inProgressDocument.State > newState)
					{
						inProgressDocument = null;
					}
				}
				else if (newState != InProcessDocumentState.Done)
				{
					inProgressDocument = new InProgressDocument(document.Id, document.FileName, newState, errorInfo);
					context.InProgressDocuments.Add(inProgressDocument);
				}

				await context.SaveChangesAsync(cancellationToken);
			});

			if (inProgressDocument != null)
			{
				await mediator.Publish(new InProgressDocumentStateChanged(inProgressDocument), cancellationToken);
			}
		}
	}
}
