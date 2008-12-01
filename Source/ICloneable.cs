using System;

namespace System
{
	/// <summary>
	/// Generic interface for cloning objects.
	/// </summary>
	public interface ICloneable<T>
	{
		/// <summary>
		/// Clones the object.
		/// </summary>
		T Clone();
	}
}
