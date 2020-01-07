using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;
using Diploma.Api.Shared.Dto;
using Diploma.IndexingService.Api.Dto;
using Diploma.IndexingService.Api.Exceptions;
using Diploma.IndexingService.Api.Extensions;
using Diploma.IndexingService.Api.Interfaces;
using Diploma.IndexingService.Api.Internal;
using Diploma.IndexingService.Core.Commands;
using Diploma.IndexingService.Core.Interfaces;
using Diploma.IndexingService.Core.Objects;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;

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
		private readonly IContentTypeProvider contentTypeProvider;
		private readonly IFoldersStorage foldersStorage;
		private readonly ITimeLimitedDataProtector dataProtector;

		public DocumentsController(
			IMediator mediator,
			ITempContentStorage tempContentStorage,
			IUserService userService,
			IDocumentStorage documentStorage,
			IContentTypeProvider contentTypeProvider,
			IFoldersStorage foldersStorage,
			IDataProtectionProvider dataProtectionProvider)
		{
			if (dataProtectionProvider == null)
			{
				throw new ArgumentNullException(nameof(dataProtectionProvider));
			}

			this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
			this.tempContentStorage = tempContentStorage ?? throw new ArgumentNullException(nameof(tempContentStorage));
			this.userService = userService ?? throw new ArgumentNullException(nameof(userService));
			this.documentStorage = documentStorage ?? throw new ArgumentNullException(nameof(documentStorage));
			this.contentTypeProvider = contentTypeProvider ?? throw new ArgumentNullException(nameof(contentTypeProvider));
			this.foldersStorage = foldersStorage ?? throw new ArgumentNullException(nameof(foldersStorage));

			dataProtector = dataProtectionProvider
				.CreateProtector(nameof(DocumentsController))
				.ToTimeLimitedDataProtector();
		}

		[HttpGet("{documentId}/downloadLink")]
		[Authorize]
		public async Task<ApiResult<string>> RedirectToDownloadDocument(string documentId)
		{
			var currentUser = await userService.GetCurrentUser();
			var encryptedData = dataProtector.Protect($"{documentId}_{currentUser.Id}", DateTimeOffset.UtcNow.AddMinutes(10));
			return ApiResult.SuccessResultWithData(Url.Action("GetDocumentContent", new { encryptedData }));
		}

		[HttpGet("download/{encryptedData}")]
		public async Task<FileStreamResult> GetDocumentContent(string encryptedData)
		{
			string decryptedData;

			try
			{
				decryptedData = dataProtector.Unprotect(encryptedData);
			}
			catch (CryptographicException)
			{
				throw new ApiServiceException("Download data is not valid");
			}

			var index = decryptedData.LastIndexOf('_');
			var documentId = decryptedData.Substring(0, index);
			var userId = new Guid(decryptedData.Substring(index + 1));
			var document = await documentStorage.GetDocument(
				new DocumentIdentity(documentId, userId),
				CancellationToken.None);
			contentTypeProvider.TryGetContentType(document.FileName, out var contentType);

			return new FileStreamResult(
				await document.Content.OpenReadStream(CancellationToken.None),
				contentType ?? "application/octet-stream")
			{
				FileDownloadName = document.FileName,
				EnableRangeProcessing = true
			};
		}

		[HttpPost]
		[Authorize]
		public async Task<ApiResult<IReadOnlyCollection<AddDocumentResultDto>>> AddDocuments(
			[FromBody]AddDocumentsInput input)
		{
			var documentsToAdd = new List<DocumentInfo>();
			var currentUser = await userService.GetCurrentUser();
			var parentFolder = await foldersStorage.GetFolder(new FolderIdentity(input.FolderId, currentUser.Id),
				CancellationToken.None);

			foreach (var documentDto in input.Documents)
			{
				var content = await tempContentStorage.GetTempContent(documentDto.ContentToken, CancellationToken.None);
				documentsToAdd.Add(documentDto.ToDocumentInfo(content, currentUser, parentFolder));
			}

			var result = (await mediator.Send(new AddDocumentsCommand(documentsToAdd)));
			return ApiResult.SuccessResultWithData(
				(IReadOnlyCollection<AddDocumentResultDto>)result
					.States
					.Select(x => new AddDocumentResultDto
					{
						Id = x.Key.GetClientId(),
						State = x.Value
					})
					.ToArray());
		}

		[HttpPost("upload")]
		[RequestSizeLimit(104857600)]
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
