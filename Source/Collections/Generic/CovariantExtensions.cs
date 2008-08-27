using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Collections.Generic
{
	/// <summary>
	/// Provides extensions to implement covariance of generic types.
	/// </summary>
#if NetFx	
	public static class CovariantExtensions
#else
	internal static class CovariantExtensions
#endif
	{
		/// <summary>
		/// Allows for covariance of generic ICollections. Adapts a collection of type
		/// <typeparam name="T" /> into a collection of type <typeparam name="U" />
		/// </summary>
		public static ICollection<U> ToCovariant<T, U>(this ICollection<T> source)
			where T : U
		{
			return new CollectionInterfaceAdapter<T, U>(source);
		}

		/// <summary>
		/// Allows for covariance of generic ILists. Adapts a collection of type
		/// <typeparam name="T" /> into a collection of type <typeparam name="U" />
		/// </summary>
		public static IList<U> ToCovariant<T, U>(this IList<T> source)
			where T : U
		{
			return new ListInterfaceAdapter<T, U>(source);
		}

		/// <summary>
		/// Allows for covariance of generic IEnumerables. Adapts a collection of type
		/// <typeparam name="T" /> into a collection of type <typeparam name="U" />
		/// </summary>
		public static IEnumerable<U> ToCovariant<T, U>(this IEnumerable<T> source)
			where T : U
		{
			return new EnumerableInterfaceAdapter<T, U>(source);
		}

		/* Credits go to the Umbrella (http://codeplex.com/umbrella) project */

		/// <summary>
		/// Allows for covariance of generic ICollections. Adapts a collection of type
		/// <typeparam name="T" /> into a collection of type <typeparam name="U" />
		/// </summary>
		class CollectionInterfaceAdapter<T, U>
			: EnumerableInterfaceAdapter<T, U>, ICollection<U> where T : U
		{
			new ICollection<T> Target { get; set; }

			public void Add(U item)
			{
				Target.Add((T)item);
			}

			public void Clear()
			{
				Target.Clear();
			}

			public bool Contains(U item)
			{
				return Target.Contains((T)item);
			}

			public void CopyTo(U[] array, int arrayIndex)
			{
				for (int i = arrayIndex; i < Target.Count; i++)
				{
					array[i] = Target.ElementAt(i);
				}
			}

			public bool Remove(U item)
			{
				return Target.Remove((T)item);
			}

			public int Count
			{
				get { return Target.Count; }
			}

			public bool IsReadOnly
			{
				get { return Target.IsReadOnly; }
			}

			public CollectionInterfaceAdapter(ICollection<T> target)
				: base(target)
			{
			}
		}

		/// <summary>
		/// Allows for covariance of generic IEnumerables. Adapts a collection of type
		/// <typeparam name="T" /> into a collection of type <typeparam name="U" />
		/// </summary>
		class EnumerableInterfaceAdapter<T, U> : IEnumerable<U> where T : U
		{
			public IEnumerable<T> Target { get; set; }

			IEnumerator IEnumerable.GetEnumerator()
			{
				return GetEnumerator();
			}

			public IEnumerator<U> GetEnumerator()
			{
				foreach (T item in Target)
					yield return item;
			}

			public EnumerableInterfaceAdapter(IEnumerable<T> target)
			{
				Target = target;
			}
		}

		/// <summary>
		/// Allows for covariance of generic ILists. Adapts a collection of type
		/// <typeparam name="T" /> into a collection of type <typeparam name="U" />
		/// </summary>
		class ListInterfaceAdapter<T, U> : CollectionInterfaceAdapter<T, U>, IList<U>
			where T : U
		{
			new IList<T> Target { get; set; }

			public int IndexOf(U item)
			{
				return Target.IndexOf((T)item);
			}

			public void Insert(int index, U item)
			{
				Target.Insert(index, (T)item);
			}

			public void RemoveAt(int index)
			{
				Target.RemoveAt(index);
			}

			public U this[int index]
			{
				get { return Target[index]; }
				set { Target[index] = (T)value; }
			}

			public ListInterfaceAdapter(IList<T> target)
				: base(target)
			{
			}
		}
	}
}
