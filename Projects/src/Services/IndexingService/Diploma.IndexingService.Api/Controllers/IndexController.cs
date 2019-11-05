using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Diploma.Api.Shared.Dto;
using Diploma.IndexingService.Api.Dto;
using Diploma.IndexingService.Api.Extensions;
using Diploma.IndexingService.Api.Interfaces;
using Diploma.IndexingService.Api.Internal;
using Diploma.IndexingService.Core.Commands;
using Diploma.IndexingService.Core.Objects;
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
		private readonly ITempContentStorage tempContentStorage;

		public IndexController(IMediator mediator, ITempContentStorage tempContentStorage)
		{
			this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
			this.tempContentStorage = tempContentStorage ?? throw new ArgumentNullException(nameof(tempContentStorage));
		}

		[HttpPost]
		public async Task AddDocuments([FromBody] IReadOnlyCollection<Document> documents)
		{
			var documentsToAdd = new List<DocumentInfo>();
			foreach (var documentDto in documents)
			{
				var content = await tempContentStorage.GetTempContent(documentDto.ContentToken);
				documentsToAdd.Add(documentDto.ToDocumentInfo(content));
			}

			await mediator.Send(new AddDocumentsCommand(documentsToAdd));
		}

		[HttpPost("upload")]
		public async Task<ApiResult<IEnumerable<string>>> Upload(IFormFileCollection files)
		{
			var tokens = new List<string>();
			foreach (var file in files)
			{
				tokens.Add(await tempContentStorage.SaveTempContent(new FormFileContent(file)));
			}

			return ApiResult.SuccessResultWithData((IEnumerable<string>)tokens);
		}
	}
}
