using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Remora.Results;
using Shared;
using Shared.Extensions;

namespace Notifications.Clients;

[ExcludeFromCodeCoverage(Justification = "Not testing an external API.")]
public class YoutubeApiClient
{
	private const string ApiUrl = "https://youtube.googleapis.com/youtube/v3/videos";
	private readonly string _youtubeApiKey;

	public YoutubeApiClient()
	{
		_youtubeApiKey = Environment.GetEnvironmentVariable("YOUTUBE_API_KEY") ??
		                 throw new EnvironmentVariableMissingException("YOUTUBE_API_KEY");
	}

	public async Task<Result<bool>> IsVideoLiveAsync(string videoId)
	{
		using var client = new HttpClient();

		var builder = new UriBuilder(ApiUrl);
		builder.AddQueryParameter("key", _youtubeApiKey);
		builder.AddQueryParameter("part", "snippet");
		builder.AddQueryParameter("id", videoId);

		var url = builder.Uri;

		var result = await client.GetAsync(url);

		if (result.IsSuccessStatusCode)
		{
			var body = await result.Content.ReadAsStreamAsync();
			var response = await JsonSerializer.DeserializeAsync<YoutubeApiResponse>(body);

			if (response is null) return Result<bool>.FromError(new YoutubeApiError("Body was not as expected."));

			return Result<bool>.FromSuccess(response.IsLive);
		}

		return Result<bool>.FromError(new YoutubeApiError(result.ReasonPhrase ?? ""));
	}

	[ExcludeFromCodeCoverage]
	private class YoutubeApiResponse
	{
		public List<Item> Items { get; set; } = null!;
		public bool IsLive => Items[0].Snippet.LiveBroadcastContent == "live";

		public class Item
		{
			public Snippet Snippet { get; } = null!;
		}

		public class Snippet
		{
			public string LiveBroadcastContent { get; } = null!;
		}
	}

	[ExcludeFromCodeCoverage]
	private class YoutubeApiError : IResultError
	{
		public YoutubeApiError(string message)
		{
			Message = message;
		}

		public string Message { get; }
	}
}
