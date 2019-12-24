using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Diploma.IndexingService.Core.Commands;
using Diploma.IndexingService.Core.Interfaces;
using Diploma.IndexingService.Core.Objects;
using MediatR;

namespace Diploma.IndexingService.Core.CommandHandlers
{
	public class AddDocumentsCommandHandler : IRequestHandler<AddDocumentsCommand, AddDocumentsResult>
	{
		private readonly IIndexingQueue indexingQueue;

		public AddDocumentsCommandHandler(IIndexingQueue indexingQueue)
		{
			this.indexingQueue = indexingQueue ?? throw new ArgumentNullException(nameof(indexingQueue));
		}

		public async Task<AddDocumentsResult> Handle(AddDocumentsCommand request, CancellationToken cancellationToken)
		{
			await indexingQueue.Enqueue(request.Documents, cancellationToken);

			
			return new AddDocumentsResult(request.Documents.ToDictionary(x => x.Id, x => InProcessDocumentState.InQueue));
		}
	}
}
