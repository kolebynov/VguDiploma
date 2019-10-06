using System;
using System.Collections.Generic;
using Diploma.IndexingService.Core.Objects;
using MediatR;

namespace Diploma.IndexingService.Core.Commands
{
	public class AddDocumentsCommand : IRequest<AddDocumentsResult>
	{
		public IReadOnlyCollection<DocumentInfo> Documents { get; }

		public AddDocumentsCommand(IReadOnlyCollection<DocumentInfo> documents)
		{
			Documents = documents ?? throw new ArgumentNullException(nameof(documents));
		}
	}
}
