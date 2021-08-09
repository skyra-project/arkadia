using System;
using NUnit.Framework;
using Shared.Extensions;

namespace UnitTests.Shared.Extensions
{
	[TestFixture]
	public class UriBuilderExtensionTests
	{
		[TestCase("foo", "bar")]
		[TestCase("users", 0)]
		[TestCase("mr", 0.1f)]
		public void UriBuilder_ShouldAppendQuestionMark_WhenFirstQueryIsAppended(string queryName, object queryValue)
		{
			// arrange

			var baseUrl = "https://www.foobar.com";

			var builder = new UriBuilder(baseUrl);

			var expected = $"https://www.foobar.com/?{queryName}={queryValue}";

			// act

			builder.AddQueryParameter(queryName, queryValue);
			var fullUrl = builder.Uri.ToString();

			// assert

			Assert.That(fullUrl, Is.EqualTo(expected));
		}

		[Test]
		public void UriBuilder_ShouldAppendAmpersand_WhenSecondQueryIsAdded()
		{
			// arrange

			var baseUrl = "https://www.foobar.com";

			var builder = new UriBuilder(baseUrl);

			var firstQueryName = "foo";
			var secondQueryName = "bar";
			var thirdQueryName = "baz";

			var firstQueryValue = 1;
			var secondQueryValue = "ding";
			var thirdQueryValue = 3.142;

			var expected = $"https://www.foobar.com/?{firstQueryName}={firstQueryValue}&{secondQueryName}={secondQueryValue}&{thirdQueryName}={thirdQueryValue}";

			// act

			builder.AddQueryParameter(firstQueryName, firstQueryValue);
			builder.AddQueryParameter(secondQueryName, secondQueryValue);
			builder.AddQueryParameter(thirdQueryName, thirdQueryValue);
			var fullUrl = builder.Uri.ToString();

			// assert

			Assert.That(fullUrl, Is.EqualTo(expected));
		}
	}
}
