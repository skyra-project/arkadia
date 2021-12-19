using System;

namespace Shared.Extensions;

public static class UriBuilderExtensions
{
	public static void AddQueryParameter<T>(this UriBuilder builder, string name, T value)
	{
		var queryToAppend = builder.Query is ""
			? $"?{name}={value}"
			: $"&{name}={value}";

		builder.Query += queryToAppend;
	}
}
