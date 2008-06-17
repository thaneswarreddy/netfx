using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Specialized;

namespace System.Web
{
	public static class UriExtensions
	{
		/// <summary>
		/// Indicate whether the uri has or not a querystring parameter with name=value
		/// </summary>
		/// <param name="uri"></param>
		/// <param name="name"></param>
		/// <param name="value"></param>
		/// <returns></returns>
		public static bool HasValue(this Uri uri, string name, string value)
		{
			var query = HttpUtility.ParseQueryString(uri.Query);
			return query.AllKeys.Contains(name) && query.GetValues(name).Contains(value);
		}

		/// <summary>
		/// Override QueryString value
		/// </summary>
		/// <param name="uri"></param>
		/// <param name="name"></param>
		/// <param name="value"></param>
		/// <returns></returns>
		public static Uri Override(this Uri uri, string name, string value)
		{
			var query = HttpUtility.ParseQueryString(uri.Query);
			query.Set(name, value);
			return new Uri(uri.WithoutQueryString() + query.ToQueryString(), UriKind.RelativeOrAbsolute);
		}

		/// <summary>
		/// Append QueryString value
		/// </summary>
		/// <param name="uri"></param>
		/// <param name="name"></param>
		/// <param name="value"></param>
		/// <returns></returns>
		public static Uri Append(this Uri uri, string name, string value)
		{
			var query = HttpUtility.ParseQueryString(uri.Query);
			query.Add(name, value);
			return new Uri(uri.WithoutQueryString() + query.ToQueryString(), UriKind.RelativeOrAbsolute);
		}

		/// <summary>
		/// Remove QueryString value
		/// </summary>
		/// <param name="uri"></param>
		/// <param name="name"></param>
		/// <param name="value"></param>
		/// <returns></returns>
		public static Uri Remove(this Uri uri, string name)
		{
			var query = HttpUtility.ParseQueryString(uri.Query);
			query.Remove(name);
			return new Uri(uri.WithoutQueryString() + query.ToQueryString(), UriKind.RelativeOrAbsolute);
		}

		public static Uri WithoutQueryString(this Uri uri)
		{
			return new Uri(uri.GetComponents(UriComponents.SchemeAndServer | UriComponents.Path, UriFormat.UriEscaped));
		}

		public static string ToQueryString(this NameValueCollection dictionary)
		{
			var builder = new StringBuilder();
			bool first = true;
			foreach (string key in dictionary.Keys)
			{
				foreach (string v in dictionary.GetValues(key))
				{
					builder.Append(first ? '?' : '&');
					first = false;
					builder.Append(Uri.EscapeDataString(key));
					builder.Append('=');
					builder.Append(Uri.EscapeDataString(v));
				}
			}
			return builder.ToString();
		}
	}
}
