using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Diploma.IndexingService.Api.Internal
{
	public class DocumentIdentityModelBinder : IModelBinder
	{
		public Task BindModelAsync(ModelBindingContext bindingContext)
		{
			return Task.CompletedTask;
		}
	}
}
