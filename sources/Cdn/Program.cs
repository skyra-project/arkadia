using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Hosting;
using Shared;

namespace Cdn
{
	public class Program
	{
		public static void Main(string[] args)
		{
			AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
			AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2Support", true);

			CreateHostBuilder(args).Build().Run();
		}

		// Additional configuration is required to successfully run gRPC on macOS.
		// For instructions on how to configure Kestrel and gRPC clients on macOS, visit https://go.microsoft.com/fwlink/?linkid=2099682
		public static IHostBuilder CreateHostBuilder(string[] args)
		{
			return Host.CreateDefaultBuilder(args)
				.ConfigureWebHostDefaults(webBuilder =>
				{
					var httpPort = Environment.GetEnvironmentVariable("HTTP_PORT") ?? throw new EnvironmentVariableMissingException("HTTP_PORT");
					var grpcPort = Environment.GetEnvironmentVariable("GRPC_PORT") ?? throw new EnvironmentVariableMissingException("GRPC_PORT");
					webBuilder.ConfigureKestrel(options =>
					{
						options.ListenAnyIP(int.Parse(httpPort), listenOptions =>
						{
							listenOptions.Protocols = HttpProtocols.Http1AndHttp2;
						});
						options.ListenAnyIP(int.Parse(grpcPort), listenOptions =>
						{
							listenOptions.Protocols = HttpProtocols.Http2;
						});
					});

					webBuilder.UseStartup<Startup>();
				});
		}
	}
}