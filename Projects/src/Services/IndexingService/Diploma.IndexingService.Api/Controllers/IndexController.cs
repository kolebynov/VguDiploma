using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Diploma.IndexingService.Api.Dto;
using Diploma.IndexingService.Api.Extensions;
using Diploma.IndexingService.Core.Commands;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Diploma.IndexingService.Api.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class IndexController : ControllerBase
	{
		private readonly IMediator mediator;

		public IndexController(IMediator mediator)
		{
			this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
		}

		[HttpPost]
		public Task AddDocuments([FromBody] IReadOnlyCollection<Document> documents)
		{
			return mediator.Send(new AddDocumentsCommand(documents.Select(x => x.ToDocumentInfo()).ToArray()));
		}

		[HttpPost("{documentId}/content")]
		public async Task AddDocumentContent(string documentId, IFormFile file)
		{
		}
	}
}
