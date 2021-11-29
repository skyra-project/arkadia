﻿using System;
using System.Collections.Concurrent;
using System.Net.Http;
using AngleSharp;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Notifications.Clients;
using Notifications.Factories;
using Notifications.Managers;
using Notifications.Models;
using Notifications.Repositories;
using Notifications.Services;

namespace Notifications
{
	public class Startup
	{
		// This method gets called by the runtime. Use this method to add services to the container.
		// For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
		public void ConfigureServices(IServiceCollection services)
		{
			services.AddSingleton(new ConcurrentQueue<Notification>());
			services.AddSingleton<YoutubeApiClient>();
			services.AddSingleton<IPubSubClient, PubSubClient>();
			services.AddSingleton<SubscriptionManager>();
			services.AddSingleton<RequestCache>();
			services.AddSingleton<HttpClient>();
			services.AddSingleton<IYoutubeRepositoryFactory, DefaultYoutubeRepositoryFactory>();
			services.AddSingleton(BrowsingContext.New(Configuration.Default.WithDefaultLoader()));
			services.AddSingleton<IChannelInfoRepository, ChannelInfoRepository>();
			services.AddSingleton<IDateTimeRepository, DateTimeRepository>();

			services.AddGrpc();

			services.AddControllers();

			services.AddLogging(options =>
			{
				options.AddSimpleConsole(console =>
				{
					console.TimestampFormat = "[yyyy-MM-dd HH:mm:ss] ";
				});

				var dnsUrl = Environment.GetEnvironmentVariable("SENTRY_URL");

				if (dnsUrl is not null) options.AddSentry(sentryOptions => sentryOptions.Dsn = dnsUrl);
			});
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			app.UseRouting();

			app.UseEndpoints(endpoints =>
			{
				endpoints.MapControllers();
				endpoints.MapGrpcService<YoutubeService>();
			});
		}
	}
}
