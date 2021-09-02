using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Threading.Tasks;
using AngleSharp;
using AngleSharp.Html.Dom;
using Microsoft.Extensions.Logging;

namespace Notifications.Repositories
{
	[ExcludeFromCodeCoverage(Justification = "Not testing an external API.")]
	public class ChannelInfoRepository : IChannelInfoRepository
	{
		private readonly IBrowsingContext _browsingContext;
		private readonly ILogger<ChannelInfoRepository> _logger;
		
		public ChannelInfoRepository(IBrowsingContext browsingContext, ILogger<ChannelInfoRepository> logger)
		{
			_browsingContext = browsingContext;
			_logger = logger;
		}


		public async Task<(string?, string?)> GetChannelInfoAsync(string channelUrl)
		{
			var document = await _browsingContext.OpenAsync(channelUrl);
			if (document.StatusCode != HttpStatusCode.OK)
			{
				_logger.LogError("Did not recieve OK response from youtube for channel url of {Url} - instead received {Status}", channelUrl, document.StatusCode);
				return (null, null);
			}

			var cell = document.QuerySelector("meta[itemprop='channelId']") as IHtmlMetaElement;

			if (cell is null)
			{
				_logger.LogError("Could not find <meta> tag for the channel-id for url {Url}", channelUrl);
				return (null, null);
			}

			var name = document.QuerySelector("meta[property='og:title']").Attributes["content"].Value;

			if (name is null)
			{
				_logger.LogError("Could not find 'og:title' tag for url {Url}", channelUrl);
				return (null, null);
			}

			return (cell.Content, name);
		}
	}
}
