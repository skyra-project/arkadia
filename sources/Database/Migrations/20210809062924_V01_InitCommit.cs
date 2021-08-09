using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Database.Migrations
{
    public partial class V01_InitCommit : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "cdn-entry",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    content_type = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    last_modified_at = table.Column<long>(type: "bigint", nullable: false),
                    e_tag = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_cdn_entry", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "guilds",
                columns: table => new
                {
                    id = table.Column<string>(type: "character varying(19)", maxLength: 19, nullable: false),
                    notificationsyoutubeuploadchannel = table.Column<string>(name: "notifications.youtube.upload.channel", type: "character varying(19)", nullable: true),
                    notificationsyoutubeuploadmessage = table.Column<string>(name: "notifications.youtube.upload.message", type: "text", nullable: true),
                    notificationsyoutubelivechannel = table.Column<string>(name: "notifications.youtube.live.channel", type: "character varying(19)", nullable: true),
                    notificationsyoutubelivemessage = table.Column<string>(name: "notifications.youtube.live.message", type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_guilds", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "youtube_subscription",
                columns: table => new
                {
                    id = table.Column<string>(type: "character varying(24)", maxLength: 24, nullable: false),
                    already_seen_ids = table.Column<string[]>(type: "character varying(11)[]", nullable: false),
                    expires_at = table.Column<long>(type: "bigint", nullable: false),
                    guild_ids = table.Column<string[]>(type: "character varying(19)[]", nullable: false),
                    channel_title = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_youtube_subscription", x => x.id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "cdn-entry");

            migrationBuilder.DropTable(
                name: "guilds");

            migrationBuilder.DropTable(
                name: "youtube_subscription");
        }
    }
}
