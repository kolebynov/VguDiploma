using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Diploma.IndexingService.Api.Events;
using Diploma.IndexingService.Core.Interfaces;
using Diploma.IndexingService.Core.Objects;
using MediatR;

namespace Diploma.IndexingService.Api.Internal
{
	internal class RootFolderForNewUserCreator : INotificationHandler<NewUserAdded>
	{
		private readonly IFoldersStorage foldersStorage;

		public RootFolderForNewUserCreator(IFoldersStorage foldersStorage)
		{
			this.foldersStorage = foldersStorage ?? throw new ArgumentNullException(nameof(foldersStorage));
		}

		public Task Handle(NewUserAdded notification, CancellationToken cancellationToken)
		{
			return foldersStorage.AddFolder(
				new Folder(
					FolderIdentity.RootFolderId(notification.AddedUser.Id), 
					"root", null, new List<FolderIdentity>()),
				cancellationToken);
		}
	}
}
