using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Diploma.IndexingService.Core;
using Diploma.IndexingService.Api.Extensions;
using Diploma.IndexingService.Api.Dto.DocIndex;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Diploma.IndexingService.Api.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class IndexController : ControllerBase
	{
		private readonly IDocumentIndexer documentIndexer;

		public IndexController(IDocumentIndexer documentIndexer)
		{
			this.documentIndexer = documentIndexer ?? throw new ArgumentNullException(nameof(documentIndexer));
		}

		[HttpPost]
		public async Task AddDocuments([FromBody] IReadOnlyCollection<Document> documents)
		{
			await documentIndexer.AddDocuments(documents.Select(x => x.ToIndexDocument()).ToArray());
		}

		[HttpPost("{documentId}/content")]
		public async Task AddDocumentContent(string documentId, IFormFile file)
		{
			await documentIndexer.AddDocumentContent(documentId, file.OpenReadStream());
		}
	}
}
