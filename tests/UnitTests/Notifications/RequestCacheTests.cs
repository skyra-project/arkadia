using Microsoft.Extensions.Logging.Abstractions;
using Notifications;
using NUnit.Framework;

namespace UnitTests.Notifications
{
	[TestFixture]
	public class RequestCacheTests
	{
		[Test]
		public void RequestCache_GetRequest_ShouldReturnFalse_WhenRequestDoesNotExist()
		{
			
			// arrange

			var cache = new RequestCache(new NullLogger<RequestCache>());
			var requestId = "test";
			var subscription = false;

			// act

			var response = cache.GetRequest(requestId, subscription);

			// assert
			
			Assert.IsFalse(response);

		}
		
		[Test]
		public void RequestCache_GetRequest_ShouldReturnFalse_WhenRequestDoesExist_ButHasDifferentValue()
		{
			
			// arrange

			var cache = new RequestCache(new NullLogger<RequestCache>());
			var requestId = "test";
			var subscription = false;

			// act

			cache.AddRequest(requestId, !subscription);
			var response = cache.GetRequest(requestId, subscription);

			// assert
			
			Assert.IsFalse(response);

		}
		
		[Test]
		public void RequestCache_GetRequest_ShouldReturnTrue_WhenRequestDoesExist()
		{
			
			// arrange

			var cache = new RequestCache(new NullLogger<RequestCache>());
			var requestId = "test";
			var subscription = false;

			// act

			cache.AddRequest(requestId, subscription);
			var response = cache.GetRequest(requestId, subscription);

			// assert
			
			Assert.IsTrue(response);
		}
	}
}
