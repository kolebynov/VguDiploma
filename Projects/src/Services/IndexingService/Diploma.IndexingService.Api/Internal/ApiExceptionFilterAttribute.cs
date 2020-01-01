using System;
using System.Net;
using System.Threading.Tasks;
using Diploma.Api.Shared.Dto;
using Diploma.IndexingService.Api.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Diploma.IndexingService.Api.Internal
{
	internal class ApiExceptionFilterAttribute : ExceptionFilterAttribute
	{
		public override void OnException(ExceptionContext context)
		{
			context.ExceptionHandled = true;
			context.Result = new ObjectResult(ApiResult.ErrorResult(context.Exception.Message))
			{
				StatusCode = context.Exception is ApiServiceException || context.Exception is InvalidOperationException ? 400 : 500
			};
		}

		public override Task OnExceptionAsync(ExceptionContext context)
		{
			OnException(context);
			return Task.CompletedTask;
		}
	}
}
