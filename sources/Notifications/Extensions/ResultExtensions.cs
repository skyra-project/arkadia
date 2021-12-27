using System;
using Notifications.Errors;
using Remora.Results;
using Services;

namespace Notifications.Extensions;

public static class ResultExtensions
{
	public static YoutubeServiceResponse AsYoutubeServiceResponse(this Result result)
	{
		return new YoutubeServiceResponse
		{
			Result = (result.IsSuccess, result.Error) switch
			{
				(true, _) => YoutubeServiceResult.Success,
				(false, UnconfiguredError) => YoutubeServiceResult.NotConfigured,
				(false, ChannelInfoRetrievalError) => YoutubeServiceResult.IncorrectChannelInfo,
				_ => throw new ArgumentOutOfRangeException()
			}
		};
	}
}
