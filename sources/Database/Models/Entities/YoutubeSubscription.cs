﻿using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using NpgsqlTypes;

namespace Database.Models.Entities
{
	[Table("youtube_subscription")]
	public class YoutubeSubscription
	{
		[Key]
		[Column("id")]
		[StringLength(24)]
		public string Id { get; set; } = null!;

		[Required]
		[Column("already_seen_ids", TypeName = "character varying(11)[]")]
		public string[] AlreadySeenIds { get; set; } = Array.Empty<string>();

		[Column("expires_at")]
		public DateTime ExpiresAt { get; set; }

		[Required]
		[Column("guild_ids", TypeName = "character varying(19)[]")]
		public string[] GuildIds { get; set; } = Array.Empty<string>();

		[Required]
		[Column("channel_title")]
		public string ChannelTitle { get; set; } = "";
	}
}
