using System.Collections.Generic;

namespace System.Linq
{
#if NetFx	
	public static class EnumerableExtensions
#else
	internal static class EnumerableExtensions
#endif
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
