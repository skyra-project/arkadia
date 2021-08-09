using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Database.Models.Entities
{
	[Table("guilds")]
	public class Guild
	{
		[Key]
		[Column("id")]
		[StringLength(19)]
		public string Id { get; set; } = null!;

		[Column("notifications.youtube.upload.channel", TypeName = "character varying(19)")]
		public string? YoutubeUploadNotificationChannel { get; set; }

		[Column("notifications.youtube.upload.message")]
		public string? YoutubeUploadNotificationMessage { get; set; }

		[Column("notifications.youtube.live.channel", TypeName = "character varying(19)")]
		public string? YoutubeUploadLiveChannel { get; set; }

		[Column("notifications.youtube.live.message")]
		public string? YoutubeUploadLiveMessage { get; set; }
	}
}
