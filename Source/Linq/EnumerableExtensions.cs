using System.Collections.Generic;

namespace System.Linq
{
	internal static class EnumerableExtensions
	{
		public static void ForEach<TItem>(this IEnumerable<TItem> source, Action<TItem> action)
		{
			foreach (var item in source)
			{
				action(item);
			}
		}
	}
}
