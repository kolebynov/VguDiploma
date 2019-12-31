using System;
using Diploma.IndexingService.Core.Objects;
using MediatR;

namespace Diploma.IndexingService.Api.Events
{
	public class NewUserAdded : INotification
	{
		public User AddedUser { get; }

		public NewUserAdded(User addedUser)
		{
			AddedUser = addedUser ?? throw new ArgumentNullException(nameof(addedUser));
		}
	}
}
