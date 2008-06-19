using System.Collections.Generic;

namespace System.Linq
{
	/// <summary>
	/// Extensions to <see cref="IEnumerable{T}"/>.
	/// </summary>
#if NetFx	
	public static class EnumerableExtensions
#else
	internal static class EnumerableExtensions
#endif
	{
		/// <summary>
		/// Iterates the <paramref name="source"/> and applies the <paramref name="action"/> 
		/// to each item.
		/// </summary>
		public static void ForEach<TItem>(this IEnumerable<TItem> source, Action<TItem> action)
		{
			foreach (var item in source)
			{
				action(item);
			}
		}

		/// <summary>
		/// Allows chaining actions on a set of items for later processing, filtering 
		/// or projection (transformation).
		/// </summary>
		public static IEnumerable<T> Act<T>(this IEnumerable<T> source, Action<T> action)
		{
			foreach (var item in source)
			{
				action(item);
				yield return item;
			}
		}
	}
}
