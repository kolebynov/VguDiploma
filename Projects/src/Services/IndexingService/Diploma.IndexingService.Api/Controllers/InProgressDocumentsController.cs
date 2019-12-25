using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Diploma.Api.Shared.Dto;
using Diploma.IndexingService.Api.Dto;
using Diploma.IndexingService.Api.Extensions;
using Diploma.IndexingService.Api.Interfaces;
using Diploma.IndexingService.Core.Interfaces;
using Diploma.IndexingService.Core.Objects;
using Microsoft.AspNetCore.Mvc;

namespace Diploma.IndexingService.Api.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class InProgressDocumentsController : ControllerBase
	{
		private readonly IInProgressDocumentsStorage inProgressDocumentsStorage;
		private readonly IUserService userService;

		public InProgressDocumentsController(IInProgressDocumentsStorage inProgressDocumentsStorage, IUserService userService)
		{
			this.inProgressDocumentsStorage = inProgressDocumentsStorage ?? throw new ArgumentNullException(nameof(inProgressDocumentsStorage));
			this.userService = userService ?? throw new ArgumentNullException(nameof(userService));
		}

		[HttpGet]
		public async Task<ApiResult<IReadOnlyCollection<InProgressDocumentDto>>> GetInProgressDocuments([FromQuery] GetQuery query)
		{
			var inProgressDocuments = await inProgressDocumentsStorage.GetInProgressDocuments(
				(await userService.GetCurrentUser()).Id, query.Limit, query.Skip, CancellationToken.None);

			return ApiResult.SuccessResultWithData<IReadOnlyCollection<InProgressDocumentDto>>(
				inProgressDocuments.Select(DtoExtensions.ToDto).ToArray());
		}

		[HttpDelete]
		public async Task<ApiResult> DeleteInProgressDocument(IReadOnlyCollection<string> documentIds)
		{
			var currentUser = await userService.GetCurrentUser();
			await inProgressDocumentsStorage.RemoveInProgressDocuments(
				documentIds.Select(x => new DocumentIdentity(x, currentUser.Id)).ToArray(),
				CancellationToken.None);

			return ApiResult.SuccessResult;
		}
	}
}
