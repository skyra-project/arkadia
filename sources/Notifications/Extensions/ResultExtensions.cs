using System;
using Notifications.Errors;
using Remora.Results;
using Services;

namespace Notifications.Extensions
{
	public static class ResultExtensions
	{
		public static YoutubeServiceResponse AsYoutubeServiceResponse(this Result result)
		{
			return new YoutubeServiceResponse
			{
				Status = (result.IsSuccess, result.Error) switch
				{
					(true, _) => YoutubeServiceStatus.Success,
					(false, UnconfiguredError) => YoutubeServiceStatus.NotConfigured,
					(false, ChannelInfoRetrievalError) => YoutubeServiceStatus.IncorrectChannelInfo,
					_ => throw new ArgumentOutOfRangeException()
				}
			};
		}
	}
}