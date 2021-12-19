using System;
using Notifications.Errors;
using Notifications.Extensions;
using NUnit.Framework;
using Remora.Results;
using Services;

namespace UnitTests.Notifications.ResultExtensions;

[TestFixture]
[Parallelizable]
public class ResultExtensionsTests
{
	[Test]
	public void ResultExtension_ReturnsSuccessfulResponse_WhenResultIsSuccess()
	{
		// arrange

		var result = Result.FromSuccess();

		// act

		var youtubeResult = result.AsYoutubeServiceResponse();

		// assert

		Assert.That(youtubeResult.Status, Is.EqualTo(YoutubeServiceStatus.Success));
	}

	[Test]
	public void ResultExtension_ReturnsNotConfigured_WhenErrorIsUnconfiguredError()
	{
		// arrange

		var result = Result.FromError(new UnconfiguredError());

		// act

		var youtubeResult = result.AsYoutubeServiceResponse();

		// assert

		Assert.That(youtubeResult.Status, Is.EqualTo(YoutubeServiceStatus.NotConfigured));
	}

	[Test]
	public void ResultExtension_ReturnsIncorrectChannelInfo_WhenErrorIsChannelInfoRetrievalError()
	{
		// arrange

		var result = Result.FromError(new ChannelInfoRetrievalError());

		// act

		var youtubeResult = result.AsYoutubeServiceResponse();

		// assert

		Assert.That(youtubeResult.Status, Is.EqualTo(YoutubeServiceStatus.IncorrectChannelInfo));
	}

	[Test]
	public void ResultExtension_ThrowsArgumentException_WhenErrorIsUnknown()
	{
		// arrange

		var result = Result.FromError(new ArgumentNullError("test"));

		// assert

		Assert.Throws(Is.TypeOf<ArgumentOutOfRangeException>(),
			delegate { result.AsYoutubeServiceResponse(); });
	}
}
