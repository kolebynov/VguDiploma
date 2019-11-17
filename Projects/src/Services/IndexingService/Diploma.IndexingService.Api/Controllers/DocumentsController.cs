using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Diploma.Api.Shared.Dto;
using Diploma.IndexingService.Api.Dto;
using Diploma.IndexingService.Api.Extensions;
using Diploma.IndexingService.Api.Interfaces;
using Diploma.IndexingService.Api.Internal;
using Diploma.IndexingService.Core.Commands;
using Diploma.IndexingService.Core.Interfaces;
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
		private readonly IUserService userService;
		private readonly IDocumentStorage documentStorage;

		public DocumentsController(IMediator mediator, ITempContentStorage tempContentStorage, IUserService userService, IDocumentStorage documentStorage)
		{
			this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
			this.tempContentStorage = tempContentStorage ?? throw new ArgumentNullException(nameof(tempContentStorage));
			this.userService = userService ?? throw new ArgumentNullException(nameof(userService));
			this.documentStorage = documentStorage ?? throw new ArgumentNullException(nameof(documentStorage));
		}

		[HttpGet]
		public async Task<ApiResult<IReadOnlyCollection<GetDocument>>> GetDocuments()
		{
			var documents =
				await documentStorage.GetDocuments(await userService.GetCurrentUser(), 100, 0, CancellationToken.None);

			return ApiResult.SuccessResultWithData((IReadOnlyCollection<GetDocument>)documents.Select(x => new GetDocument
			{
				Id = x.Id.GetClientId(),
				FileName = x.FileName,
				ModificationDate = x.ModificationDate
			}).ToArray());
		}

		[HttpPost]
		public async Task<ApiResult> AddDocuments([FromBody] IReadOnlyCollection<AddDocument> documents)
		{
			var documentsToAdd = new List<DocumentInfo>();
			var currentUser = await userService.GetCurrentUser();

			foreach (var documentDto in documents)
			{
				var content = await tempContentStorage.GetTempContent(documentDto.ContentToken, CancellationToken.None);
				documentsToAdd.Add(documentDto.ToDocumentInfo(content, currentUser));
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
