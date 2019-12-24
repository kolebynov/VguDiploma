using System;
using System.Threading;
using System.Threading.Tasks;
using Diploma.IndexingService.Api.Dto;
using Diploma.IndexingService.Api.Extensions;
using Diploma.IndexingService.Core.Events;
using MediatR;
using Microsoft.AspNetCore.SignalR;

namespace Diploma.IndexingService.Api.Internal
{
	internal class InProgressDocumentStateChangedHandler : INotificationHandler<InProgressDocumentStateChanged>
	{
		private readonly IHubContext<SignalrHub> hubContext;

		public InProgressDocumentStateChangedHandler(IHubContext<SignalrHub> hubContext)
		{
			this.hubContext = hubContext ?? throw new ArgumentNullException(nameof(hubContext));
		}

		public Task Handle(InProgressDocumentStateChanged notification, CancellationToken cancellationToken)
		{
			var inProgressDocument = notification.InProgressDocument;
			return hubContext.Clients.User(inProgressDocument.Id.UserIdentity)
				.SendAsync(
					"inProgressDocumentStateChanged",
					new InProgressDocumentDto
					{
						Document = new GetDocumentDto { Id = inProgressDocument.Id.GetClientId(), FileName = inProgressDocument.FileName },
						State = inProgressDocument.State,
						ErrorInfo = inProgressDocument.ErrorInfo
					},
					cancellationToken);
		}
	}
}
