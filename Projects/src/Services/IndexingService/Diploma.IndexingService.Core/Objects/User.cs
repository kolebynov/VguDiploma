using System;

namespace Diploma.IndexingService.Core.Objects
{
	public class User
	{
		public string Id { get; private set; }

		public User(string id)
		{
			if (string.IsNullOrEmpty(id))
			{
				throw new ArgumentException("Value cannot be null or empty.", nameof(id));
			}

			Id = id;
		}

		private User()
		{
		}
	}
}
