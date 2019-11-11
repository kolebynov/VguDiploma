﻿using System;
using System.Collections.Generic;
using System.Threading;
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
	public class DocumentsController : ControllerBase
	{
		private readonly IMediator mediator;
		private readonly ITempContentStorage tempContentStorage;

		public DocumentsController(IMediator mediator, ITempContentStorage tempContentStorage)
		{
			this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
			this.tempContentStorage = tempContentStorage ?? throw new ArgumentNullException(nameof(tempContentStorage));
		}

		[HttpPost]
		public async Task<ApiResult> AddDocuments([FromBody] IReadOnlyCollection<Document> documents)
		{
			var documentsToAdd = new List<DocumentInfo>();
			foreach (var documentDto in documents)
			{
				var content = await tempContentStorage.GetTempContent(documentDto.ContentToken, CancellationToken.None);
				documentsToAdd.Add(documentDto.ToDocumentInfo(content));
			}

			await mediator.Send(new AddDocumentsCommand(documentsToAdd));
			return ApiResult.SuccessResult;
		}

		[HttpPost("upload")]
		public async Task<ApiResult<IEnumerable<string>>> Upload(IFormFileCollection files)
		{
			var tokens = new List<string>();
			foreach (var file in files)
			{
				tokens.Add(await tempContentStorage.SaveTempContent(new FormFileContent(file), CancellationToken.None));
			}

			return ApiResult.SuccessResultWithData((IEnumerable<string>)tokens);
		}
	}
}