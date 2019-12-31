using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Diploma.IndexingService.Core.Database;
using Diploma.IndexingService.Core.Events;
using Diploma.IndexingService.Core.Interfaces;
using Diploma.IndexingService.Core.Objects;
using EFCore.BulkExtensions;
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

		public async Task<IReadOnlyCollection<InProgressDocument>> GetInProgressDocuments(Guid userIdentity, int limit,
			int skip,
			CancellationToken cancellationToken) =>
			await context.InProgressDocuments.Where(x => x.UserIdentity == userIdentity)
				.Skip(skip)
				.Take(limit)
				.Select(x => new InProgressDocument(new DocumentIdentity(x.Id, userIdentity), x.FileName, x.State,
					x.LastStatusUpdateTime, x.ErrorInfo))
				.ToArrayAsync(cancellationToken);

		public async Task RemoveInProgressDocuments(IReadOnlyCollection<DocumentIdentity> documentIds, CancellationToken cancellationToken)
		{
			foreach (var documentId in documentIds)
			{
				var document = await context.InProgressDocuments.FindAsync(
					new object[] { documentId.Id, documentId.UserIdentity }, cancellationToken);
				if (document != null)
				{
					context.InProgressDocuments.Remove(document);
				}
			}

			await context.SaveChangesAsync(cancellationToken);
		}

		public Task RemoveAll(CancellationToken cancellationToken)
		{
			return context.InProgressDocuments.BatchDeleteAsync(cancellationToken);
		}

		private async Task UpdateStateInternal(DocumentInfo document, InProcessDocumentState newState, string errorInfo,
			CancellationToken cancellationToken)
		{
			InProgressDocumentDbItem inProgressDocument = null;
			await ResilientTransaction.New(context).ExecuteAsync(async () =>
			{
				var (id, userIdentity) = document.Id;
				inProgressDocument =
					await context.InProgressDocuments.FindAsync(new object[] { id, userIdentity }, cancellationToken);
				if (inProgressDocument != null)
				{
					context.InProgressDocuments.Update(inProgressDocument);
				}
				else
				{
					inProgressDocument = new InProgressDocumentDbItem
					{
						Id = id,
						UserIdentity = userIdentity,
					};
					context.InProgressDocuments.Add(inProgressDocument);
				}

				inProgressDocument.ErrorInfo = errorInfo;
				inProgressDocument.FileName = document.FileName;
				inProgressDocument.State = newState;
				inProgressDocument.LastStatusUpdateTime = DateTimeOffset.UtcNow;

				await context.SaveChangesAsync(cancellationToken);
			});

			if (inProgressDocument.State == newState)
			{
				await mediator.Publish(
					new InProgressDocumentStateChanged(
						new InProgressDocument(
							document.Id,
							inProgressDocument.FileName, inProgressDocument.State,
							inProgressDocument.LastStatusUpdateTime, inProgressDocument.ErrorInfo)),
					cancellationToken);
			}
		}
	}
}
