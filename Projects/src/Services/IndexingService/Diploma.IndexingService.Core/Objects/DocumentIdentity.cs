using System;

namespace Diploma.IndexingService.Core.Objects
{
	public class DocumentIdentity
	{
		public string Id { get; }

		public string UserIdentity { get; }

		public DocumentIdentity(string id, string userIdentity)
		{
			if (string.IsNullOrEmpty(id))
			{
				throw new ArgumentException("Value cannot be null or empty.", nameof(id));
			}

			if (string.IsNullOrEmpty(userIdentity))
			{
				throw new ArgumentException("Value cannot be null or empty.", nameof(userIdentity));
			}

			Id = id;
			UserIdentity = userIdentity;
		}

		public void Deconstruct(out string id, out string userIdentity)
		{
			id = Id;
			userIdentity = UserIdentity;
		}

		public override string ToString() => $"{Id}_{UserIdentity}";

		public static DocumentIdentity FromString(string stringId)
		{
			var lastIndex = stringId.LastIndexOf('_');
			if (lastIndex < 0)
			{
				throw new FormatException("Input string doesn't have separator");
			}

			return new DocumentIdentity(stringId.Substring(0, lastIndex), stringId.Substring(lastIndex + 1));
		}
	}
}
