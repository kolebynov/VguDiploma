using System;
using Diploma.IndexingService.Core.Objects;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.DependencyInjection;

namespace Diploma.IndexingService.Api.Internal
{
	internal class ObjectsModelBinderProvider : IModelBinderProvider
	{
		public IModelBinder GetBinder(ModelBinderProviderContext context)
		{
			if (context == null)
			{
				throw new ArgumentNullException(nameof(context));
			}

			if (context.Metadata.ModelType == typeof(FolderIdentity))
			{
				return context.Services.GetRequiredService<FolderIdentityModelBinder>();
			}

			if (context.Metadata.ModelType == typeof(DocumentIdentity))
			{
				return context.Services.GetRequiredService<DocumentIdentityModelBinder>();
			}

			return null;
		}
	}
}
