﻿using System;
using System.ComponentModel;

namespace System
{
	/// <summary>
	/// Helper interface used to hide the base <see cref="Object"/> 
	/// members from the fluent API to make it much cleaner 
	/// in Visual Studio intellisense.
	/// </summary>
	[EditorBrowsable(EditorBrowsableState.Never)]
	public interface IHideObjectMembers
	{
		/// <summary/>
		[EditorBrowsable(EditorBrowsableState.Never)]
		Type GetType();

		/// <summary/>
		[EditorBrowsable(EditorBrowsableState.Never)]
		int GetHashCode();

		/// <summary/>
		[EditorBrowsable(EditorBrowsableState.Never)]
		string ToString();

		/// <summary/>
		[EditorBrowsable(EditorBrowsableState.Never)]
		bool Equals(object obj);
	}
}
