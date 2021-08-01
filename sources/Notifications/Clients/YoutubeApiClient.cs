using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Database.Models.Entities;
using Google.Protobuf.WellKnownTypes;
using Remora.Results;
using Shared;
using Shared.Extensions;

namespace Notifications
{
	public class YoutubeApiClient
	{
		private readonly string _youtubeApiKey;
		private const string ApiUrl = "https://youtube.googleapis.com/youtube/v3/videos?part=snippet&id=CBR77HKlO00&key=[YOUR_API_KEY]";

		public YoutubeApiClient()
		{
			_youtubeApiKey = Environment.GetEnvironmentVariable("YOUTUBE_API_KEY") ?? throw new EnvironmentVariableMissingException("YOUTUBE_API_KEY");
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
				return Result<bool>.FromSuccess(response.IsLive);
			}
			return Result<bool>.FromError(new YoutubeApiError(result.ReasonPhrase ?? ""));
		}

		private class YoutubeApiResponse
		{
			public List<Item> Items { get; set; }

			public class Item
			{
				public Snippet Snippet { get; set; }
			}
			
			public class Snippet
			{
				public string LiveBroadcastContent { get; set; }
			}

			public bool IsLive => Items[0].Snippet.LiveBroadcastContent == "live";
		}
		
		private class YoutubeApiError : IResultError
		{
			public YoutubeApiError(string message)
			{
				Message = message;
			}

			public string Message { get; }
		}
	}
}