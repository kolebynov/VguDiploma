using System;
using Diploma.IndexingService.Core.Objects;
using MediatR;

namespace Diploma.IndexingService.Core.Events
{
	public class InProgressDocumentStateChanged : INotification
	{
		public InProgressDocument InProgressDocument { get; }

		public InProgressDocumentStateChanged(InProgressDocument inProgressDocument)
		{
			InProgressDocument = inProgressDocument ?? throw new ArgumentNullException(nameof(inProgressDocument));
		}
	}
}
