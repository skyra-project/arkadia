using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Cdn.Services;
using Microsoft.Extensions.Logging;

namespace Cdn
{
	public class Startup
	{
		// This method gets called by the runtime. Use this method to add services to the container.
		// For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
		public void ConfigureServices(IServiceCollection services)
		{
			services.AddGrpc();
			
			services.AddControllers();

			services.AddLogging(options =>
			{
				options.AddSimpleConsole(console =>
				{
					console.TimestampFormat = "[yyyy-MM-dd HH:mm:ss] ";
				});

				var dnsUrl = Environment.GetEnvironmentVariable("SENTRY_URL");

				if (dnsUrl is not null)
				{
					options.AddSentry(sentryOptions => sentryOptions.Dsn = dnsUrl);
				}
			});
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			app.UseRouting();

			app.UseEndpoints(endpoints =>
			{
				endpoints.MapGrpcService<GreeterService>();
			});
		}
	}
}