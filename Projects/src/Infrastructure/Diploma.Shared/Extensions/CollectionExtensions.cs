using System;

namespace Diploma.Shared.Extensions
{
	public static class CollectionExtensions
	{
		public static T[] AsArray<T>(this T obj)
		{
			if (obj == null)
			{
				throw new ArgumentNullException(nameof(obj));
			}

			return new[] { obj };
		}
	}
}
