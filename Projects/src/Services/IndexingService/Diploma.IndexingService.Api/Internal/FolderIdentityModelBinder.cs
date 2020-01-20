using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Diploma.IndexingService.Api.Interfaces;
using Diploma.IndexingService.Core.Objects;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.DependencyInjection;

namespace Diploma.IndexingService.Api.Internal
{
	public class FolderIdentityModelBinder : IModelBinder
	{
		public async Task BindModelAsync(ModelBindingContext bindingContext)
		{
			if (bindingContext == null)
			{
				throw new ArgumentNullException(nameof(bindingContext));
			}

			var valueProviderResult = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
			if (valueProviderResult == ValueProviderResult.None)
			{
				return;
			}

			bindingContext.ModelState.SetModelValue(bindingContext.ModelName, valueProviderResult);
			var stringFolderId = valueProviderResult.FirstValue;

			if (string.IsNullOrEmpty(stringFolderId))
			{
				return;
			}

			if (Guid.TryParse(stringFolderId, out var folderId))
			{
				var currentUser = await bindingContext.HttpContext.RequestServices.GetRequiredService<IUserService>().GetCurrentUser();
				bindingContext.Result = ModelBindingResult.Success(new FolderIdentity(folderId, currentUser.Id));
			}
			else
			{
				bindingContext.ModelState.TryAddModelError(bindingContext.ModelName, "Folder Id must be a guid");
			}
		}
	}
}
