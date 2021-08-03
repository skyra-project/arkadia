using System;
using Database.Models.Entities;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace Database
{
	/// <inheritdoc />
	public class ArkadiaDbContext : DbContext
	{
		public DbSet<CdnEntry> CdnEntries { get; set; }
		public DbSet<Guild> Guilds { get; set; }
		public DbSet<YoutubeSubscription> YoutubeSubscriptions { get; set; }

		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			if (optionsBuilder.IsConfigured)
			{
				return;
			}

			var user = Environment.GetEnvironmentVariable("POSTGRES_USER") ?? "postgres";
			var password = Environment.GetEnvironmentVariable("POSTGRES_PASSWORD") ?? "postgres";
			var host = Environment.GetEnvironmentVariable("POSTGRES_HOST") ?? "localhost";
			var port = Environment.GetEnvironmentVariable("POSTGRES_PORT") ?? "5432";
			var name = Environment.GetEnvironmentVariable("POSTGRES_DB") ?? "arkadia";

			optionsBuilder.UseNpgsql(
				$"User ID={user};Password={password};Server={host};Port={port};Database={name};Pooling=true;",
				options => options.EnableRetryOnFailure()).UseSnakeCaseNamingConvention();
		}
	}
}