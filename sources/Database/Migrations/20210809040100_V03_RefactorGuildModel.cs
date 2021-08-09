using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Database.Migrations
{
    public partial class V03_RefactorGuildModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "afk.prefix",
                table: "guilds");

            migrationBuilder.DropColumn(
                name: "afk.prefix-force",
                table: "guilds");

            migrationBuilder.DropColumn(
                name: "afk.role",
                table: "guilds");

            migrationBuilder.DropColumn(
                name: "birthday.channel",
                table: "guilds");

            migrationBuilder.DropColumn(
                name: "birthday.message",
                table: "guilds");

            migrationBuilder.DropColumn(
                name: "birthday.role",
                table: "guilds");

            migrationBuilder.DropColumn(
                name: "channels.announcements",
                table: "guilds");

            migrationBuilder.DropColumn(
                name: "channels.farewell",
                table: "guilds");

            migrationBuilder.DropColumn(
                name: "channels.greeting",
                table: "guilds");

            migrationBuilder.DropColumn(
                name: "channels.ignore.all",
                table: "guilds");

            migrationBuilder.DropColumn(
                name: "channels.ignore.message-delete",
                table: "guilds");

            migrationBuilder.DropColumn(
                name: "channels.ignore.message-edit",
                table: "guilds");

            migrationBuilder.DropColumn(
                name: "channels.ignore.reaction-add",
                table: "guilds");

            migrationBuilder.DropColumn(
                name: "channels.logs.channel-create",
                table: "guilds");

            migrationBuilder.DropColumn(
                name: "channels.logs.channel-delete",
                table: "guilds");

            migrationBuilder.DropColumn(
                name: "channels.logs.channel-update",
                table: "guilds");

            migrationBuilder.DropColumn(
                name: "channels.logs.emoji-create",
                table: "guilds");

            migrationBuilder.DropColumn(
                name: "channels.logs.emoji-delete",
                table: "guilds");

            migrationBuilder.DropColumn(
                name: "channels.logs.emoji-update",
                table: "guilds");

            migrationBuilder.DropColumn(
                name: "channels.logs.image",
                table: "guilds");

            migrationBuilder.DropColumn(
                name: "channels.logs.member-add",
                table: "guilds");

            migrationBuilder.DropColumn(
                name: "channels.logs.member-nickname-update",
                table: "guilds");

            migrationBuilder.DropColumn(
                name: "channels.logs.member-remove",
                table: "guilds");

            migrationBuilder.DropColumn(
                name: "channels.logs.member-roles-update",
                table: "guilds");

            migrationBuilder.DropColumn(
                name: "channels.logs.member-username-update",
                table: "guilds");

            migrationBuilder.DropColumn(
                name: "channels.logs.message-delete",
                table: "guilds");

            migrationBuilder.DropColumn(
                name: "channels.logs.message-delete-nsfw",
                table: "guilds");

            migrationBuilder.DropColumn(
                name: "channels.logs.message-update",
                table: "guilds");

            migrationBuilder.DropColumn(
                name: "channels.logs.message-update-nsfw",
                table: "guilds");

            migrationBuilder.DropColumn(
                name: "channels.logs.moderation",
                table: "guilds");

            migrationBuilder.DropColumn(
                name: "channels.logs.prune",
                table: "guilds");

            migrationBuilder.DropColumn(
                name: "channels.logs.reaction",
                table: "guilds");

            migrationBuilder.DropColumn(
                name: "channels.logs.role-create",
                table: "guilds");

            migrationBuilder.DropColumn(
                name: "channels.logs.role-delete",
                table: "guilds");

            migrationBuilder.DropColumn(
                name: "channels.logs.role-update",
                table: "guilds");

            migrationBuilder.DropColumn(
                name: "channels.logs.server-update",
                table: "guilds");

            migrationBuilder.DropColumn(
                name: "channels.spam",
                table: "guilds");

            migrationBuilder.DropColumn(
                name: "command-auto-delete",
                table: "guilds");

            migrationBuilder.DropColumn(
                name: "custom-commands",
                table: "guilds");

            migrationBuilder.DropColumn(
                name: "disable-natural-prefix",
                table: "guilds");

            migrationBuilder.DropColumn(
                name: "disabled-channels",
                table: "guilds");

            migrationBuilder.DropColumn(
                name: "disabled-commands",
                table: "guilds");

            migrationBuilder.DropColumn(
                name: "disabled-commands-channels",
                table: "guilds");

            migrationBuilder.DropColumn(
                name: "events.ban-add",
                table: "guilds");

            migrationBuilder.DropColumn(
                name: "events.ban-remove",
                table: "guilds");

            migrationBuilder.DropColumn(
                name: "events.member-username-update",
                table: "guilds");

            migrationBuilder.DropColumn(
                name: "events.twemoji-reactions",
                table: "guilds");

            migrationBuilder.DropColumn(
                name: "language",
                table: "guilds");

            migrationBuilder.DropColumn(
                name: "messages.announcement-embed",
                table: "guilds");

            migrationBuilder.DropColumn(
                name: "messages.auto-delete.ignored-all",
                table: "guilds");

            migrationBuilder.DropColumn(
                name: "messages.auto-delete.ignored-channels",
                table: "guilds");

            migrationBuilder.DropColumn(
                name: "messages.auto-delete.ignored-commands",
                table: "guilds");

            migrationBuilder.DropColumn(
                name: "messages.auto-delete.ignored-roles",
                table: "guilds");

            migrationBuilder.DropColumn(
                name: "messages.farewell",
                table: "guilds");

            migrationBuilder.DropColumn(
                name: "messages.farewell-auto-delete",
                table: "guilds");

            migrationBuilder.DropColumn(
                name: "messages.greeting",
                table: "guilds");

            migrationBuilder.DropColumn(
                name: "messages.greeting-auto-delete",
                table: "guilds");

            migrationBuilder.DropColumn(
                name: "messages.ignore-channels",
                table: "guilds");

            migrationBuilder.DropColumn(
                name: "messages.join-dm",
                table: "guilds");

            migrationBuilder.DropColumn(
                name: "messages.moderation-auto-delete",
                table: "guilds");

            migrationBuilder.DropColumn(
                name: "messages.moderation-dm",
                table: "guilds");

            migrationBuilder.DropColumn(
                name: "messages.moderation-message-display",
                table: "guilds");

            migrationBuilder.DropColumn(
                name: "messages.moderation-reason-display",
                table: "guilds");

            migrationBuilder.DropColumn(
                name: "messages.moderator-name-display",
                table: "guilds");

            migrationBuilder.DropColumn(
                name: "music.allow-streams",
                table: "guilds");

            migrationBuilder.DropColumn(
                name: "music.allowed-roles",
                table: "guilds");

            migrationBuilder.DropColumn(
                name: "music.allowed-voice-channels",
                table: "guilds");

            migrationBuilder.DropColumn(
                name: "music.default-volume",
                table: "guilds");

            migrationBuilder.DropColumn(
                name: "music.maximum-duration",
                table: "guilds");

            migrationBuilder.DropColumn(
                name: "music.maximum-entries-per-user",
                table: "guilds");

            migrationBuilder.DropColumn(
                name: "no-mention-spam.alerts",
                table: "guilds");

            migrationBuilder.DropColumn(
                name: "no-mention-spam.enabled",
                table: "guilds");

            migrationBuilder.DropColumn(
                name: "no-mention-spam.mentions-allowed",
                table: "guilds");

            migrationBuilder.DropColumn(
                name: "no-mention-spam.time-period",
                table: "guilds");

            migrationBuilder.DropColumn(
                name: "notifications.streams.twitch.streamers",
                table: "guilds");

            migrationBuilder.DropColumn(
                name: "permissions.roles",
                table: "guilds");

            migrationBuilder.DropColumn(
                name: "permissions.users",
                table: "guilds");

            migrationBuilder.DropColumn(
                name: "prefix",
                table: "guilds");

            migrationBuilder.DropColumn(
                name: "reaction-roles",
                table: "guilds");

            migrationBuilder.DropColumn(
                name: "roles.admin",
                table: "guilds");

            migrationBuilder.DropColumn(
                name: "roles.auto",
                table: "guilds");

            migrationBuilder.DropColumn(
                name: "roles.dj",
                table: "guilds");

            migrationBuilder.DropColumn(
                name: "roles.initial",
                table: "guilds");

            migrationBuilder.DropColumn(
                name: "roles.initial-bots",
                table: "guilds");

            migrationBuilder.DropColumn(
                name: "roles.initial-humans",
                table: "guilds");

            migrationBuilder.DropColumn(
                name: "roles.moderator",
                table: "guilds");

            migrationBuilder.DropColumn(
                name: "roles.muted",
                table: "guilds");

            migrationBuilder.DropColumn(
                name: "roles.public",
                table: "guilds");

            migrationBuilder.DropColumn(
                name: "roles.remove-initial",
                table: "guilds");

            migrationBuilder.DropColumn(
                name: "roles.restricted-attachment",
                table: "guilds");

            migrationBuilder.DropColumn(
                name: "roles.restricted-embed",
                table: "guilds");

            migrationBuilder.DropColumn(
                name: "roles.restricted-emoji",
                table: "guilds");

            migrationBuilder.DropColumn(
                name: "roles.restricted-reaction",
                table: "guilds");

            migrationBuilder.DropColumn(
                name: "roles.restricted-voice",
                table: "guilds");

            migrationBuilder.DropColumn(
                name: "roles.subscriber",
                table: "guilds");

            migrationBuilder.DropColumn(
                name: "roles.unique-role-sets",
                table: "guilds");

            migrationBuilder.DropColumn(
                name: "selfmod.attachments.enabled",
                table: "guilds");

            migrationBuilder.DropColumn(
                name: "selfmod.attachments.hard-action",
                table: "guilds");

            migrationBuilder.DropColumn(
                name: "selfmod.attachments.hard-action-duration",
                table: "guilds");

            migrationBuilder.DropColumn(
                name: "selfmod.attachments.ignored-channels",
                table: "guilds");

            migrationBuilder.DropColumn(
                name: "selfmod.attachments.ignored-roles",
                table: "guilds");

            migrationBuilder.DropColumn(
                name: "selfmod.attachments.soft-action",
                table: "guilds");

            migrationBuilder.DropColumn(
                name: "selfmod.attachments.threshold-duration",
                table: "guilds");

            migrationBuilder.DropColumn(
                name: "selfmod.attachments.threshold-maximum",
                table: "guilds");

            migrationBuilder.DropColumn(
                name: "selfmod.capitals.enabled",
                table: "guilds");

            migrationBuilder.DropColumn(
                name: "selfmod.capitals.hard-action",
                table: "guilds");

            migrationBuilder.DropColumn(
                name: "selfmod.capitals.hard-action-duration",
                table: "guilds");

            migrationBuilder.DropColumn(
                name: "selfmod.capitals.ignored-channels",
                table: "guilds");

            migrationBuilder.DropColumn(
                name: "selfmod.capitals.ignored-roles",
                table: "guilds");

            migrationBuilder.DropColumn(
                name: "selfmod.capitals.maximum",
                table: "guilds");

            migrationBuilder.DropColumn(
                name: "selfmod.capitals.minimum",
                table: "guilds");

            migrationBuilder.DropColumn(
                name: "selfmod.capitals.soft-action",
                table: "guilds");

            migrationBuilder.DropColumn(
                name: "selfmod.capitals.threshold-duration",
                table: "guilds");

            migrationBuilder.DropColumn(
                name: "selfmod.capitals.threshold-maximum",
                table: "guilds");

            migrationBuilder.DropColumn(
                name: "selfmod.filter.enabled",
                table: "guilds");

            migrationBuilder.DropColumn(
                name: "selfmod.filter.hard-action",
                table: "guilds");

            migrationBuilder.DropColumn(
                name: "selfmod.filter.hard-action-duration",
                table: "guilds");

            migrationBuilder.DropColumn(
                name: "selfmod.filter.ignored-channels",
                table: "guilds");

            migrationBuilder.DropColumn(
                name: "selfmod.filter.ignored-roles",
                table: "guilds");

            migrationBuilder.DropColumn(
                name: "selfmod.filter.raw",
                table: "guilds");

            migrationBuilder.DropColumn(
                name: "selfmod.filter.soft-action",
                table: "guilds");

            migrationBuilder.DropColumn(
                name: "selfmod.filter.threshold-duration",
                table: "guilds");

            migrationBuilder.DropColumn(
                name: "selfmod.filter.threshold-maximum",
                table: "guilds");

            migrationBuilder.DropColumn(
                name: "selfmod.ignored-channels",
                table: "guilds");

            migrationBuilder.DropColumn(
                name: "selfmod.invites.enabled",
                table: "guilds");

            migrationBuilder.DropColumn(
                name: "selfmod.invites.hard-action",
                table: "guilds");

            migrationBuilder.DropColumn(
                name: "selfmod.invites.hard-action-duration",
                table: "guilds");

            migrationBuilder.DropColumn(
                name: "selfmod.invites.ignored-channels",
                table: "guilds");

            migrationBuilder.DropColumn(
                name: "selfmod.invites.ignored-codes",
                table: "guilds");

            migrationBuilder.DropColumn(
                name: "selfmod.invites.ignored-guilds",
                table: "guilds");

            migrationBuilder.DropColumn(
                name: "selfmod.invites.ignored-roles",
                table: "guilds");

            migrationBuilder.DropColumn(
                name: "selfmod.invites.soft-action",
                table: "guilds");

            migrationBuilder.DropColumn(
                name: "selfmod.invites.threshold-duration",
                table: "guilds");

            migrationBuilder.DropColumn(
                name: "selfmod.invites.threshold-maximum",
                table: "guilds");

            migrationBuilder.DropColumn(
                name: "selfmod.links.allowed",
                table: "guilds");

            migrationBuilder.DropColumn(
                name: "selfmod.links.enabled",
                table: "guilds");

            migrationBuilder.DropColumn(
                name: "selfmod.links.hard-action",
                table: "guilds");

            migrationBuilder.DropColumn(
                name: "selfmod.links.hard-action-duration",
                table: "guilds");

            migrationBuilder.DropColumn(
                name: "selfmod.links.ignored-channels",
                table: "guilds");

            migrationBuilder.DropColumn(
                name: "selfmod.links.ignored-roles",
                table: "guilds");

            migrationBuilder.DropColumn(
                name: "selfmod.links.soft-action",
                table: "guilds");

            migrationBuilder.DropColumn(
                name: "selfmod.links.threshold-duration",
                table: "guilds");

            migrationBuilder.DropColumn(
                name: "selfmod.links.threshold-maximum",
                table: "guilds");

            migrationBuilder.DropColumn(
                name: "selfmod.messages.enabled",
                table: "guilds");

            migrationBuilder.DropColumn(
                name: "selfmod.messages.hard-action",
                table: "guilds");

            migrationBuilder.DropColumn(
                name: "selfmod.messages.hard-action-duration",
                table: "guilds");

            migrationBuilder.DropColumn(
                name: "selfmod.messages.ignored-channels",
                table: "guilds");

            migrationBuilder.DropColumn(
                name: "selfmod.messages.ignored-roles",
                table: "guilds");

            migrationBuilder.DropColumn(
                name: "selfmod.messages.maximum",
                table: "guilds");

            migrationBuilder.DropColumn(
                name: "selfmod.messages.queue-size",
                table: "guilds");

            migrationBuilder.DropColumn(
                name: "selfmod.messages.soft-action",
                table: "guilds");

            migrationBuilder.DropColumn(
                name: "selfmod.messages.threshold-duration",
                table: "guilds");

            migrationBuilder.DropColumn(
                name: "selfmod.messages.threshold-maximum",
                table: "guilds");

            migrationBuilder.DropColumn(
                name: "selfmod.newlines.enabled",
                table: "guilds");

            migrationBuilder.DropColumn(
                name: "selfmod.newlines.hard-action",
                table: "guilds");

            migrationBuilder.DropColumn(
                name: "selfmod.newlines.hard-action-duration",
                table: "guilds");

            migrationBuilder.DropColumn(
                name: "selfmod.newlines.ignored-channels",
                table: "guilds");

            migrationBuilder.DropColumn(
                name: "selfmod.newlines.ignored-roles",
                table: "guilds");

            migrationBuilder.DropColumn(
                name: "selfmod.newlines.maximum",
                table: "guilds");

            migrationBuilder.DropColumn(
                name: "selfmod.newlines.soft-action",
                table: "guilds");

            migrationBuilder.DropColumn(
                name: "selfmod.newlines.threshold-duration",
                table: "guilds");

            migrationBuilder.DropColumn(
                name: "selfmod.newlines.threshold-maximum",
                table: "guilds");

            migrationBuilder.DropColumn(
                name: "selfmod.reactions.allowed",
                table: "guilds");

            migrationBuilder.DropColumn(
                name: "selfmod.reactions.blocked",
                table: "guilds");

            migrationBuilder.DropColumn(
                name: "selfmod.reactions.enabled",
                table: "guilds");

            migrationBuilder.DropColumn(
                name: "selfmod.reactions.hard-action",
                table: "guilds");

            migrationBuilder.DropColumn(
                name: "selfmod.reactions.hard-action-duration",
                table: "guilds");

            migrationBuilder.DropColumn(
                name: "selfmod.reactions.ignored-channels",
                table: "guilds");

            migrationBuilder.DropColumn(
                name: "selfmod.reactions.ignored-roles",
                table: "guilds");

            migrationBuilder.DropColumn(
                name: "selfmod.reactions.maximum",
                table: "guilds");

            migrationBuilder.DropColumn(
                name: "selfmod.reactions.soft-action",
                table: "guilds");

            migrationBuilder.DropColumn(
                name: "selfmod.reactions.threshold-duration",
                table: "guilds");

            migrationBuilder.DropColumn(
                name: "selfmod.reactions.threshold-maximum",
                table: "guilds");

            migrationBuilder.DropColumn(
                name: "social.achieve-channel",
                table: "guilds");

            migrationBuilder.DropColumn(
                name: "social.achieve-level",
                table: "guilds");

            migrationBuilder.DropColumn(
                name: "social.achieve-multiple",
                table: "guilds");

            migrationBuilder.DropColumn(
                name: "social.achieve-role",
                table: "guilds");

            migrationBuilder.DropColumn(
                name: "social.enabled",
                table: "guilds");

            migrationBuilder.DropColumn(
                name: "social.ignored-channels",
                table: "guilds");

            migrationBuilder.DropColumn(
                name: "social.ignored-roles",
                table: "guilds");

            migrationBuilder.DropColumn(
                name: "social.multiplier",
                table: "guilds");

            migrationBuilder.DropColumn(
                name: "starboard.channel",
                table: "guilds");

            migrationBuilder.DropColumn(
                name: "starboard.emoji",
                table: "guilds");

            migrationBuilder.DropColumn(
                name: "starboard.ignored-channels",
                table: "guilds");

            migrationBuilder.DropColumn(
                name: "starboard.maximum-age",
                table: "guilds");

            migrationBuilder.DropColumn(
                name: "starboard.minimum",
                table: "guilds");

            migrationBuilder.DropColumn(
                name: "starboard.self-star",
                table: "guilds");

            migrationBuilder.DropColumn(
                name: "sticky-roles",
                table: "guilds");

            migrationBuilder.DropColumn(
                name: "suggestions.channel",
                table: "guilds");

            migrationBuilder.DropColumn(
                name: "suggestions.emojis.downvote",
                table: "guilds");

            migrationBuilder.DropColumn(
                name: "suggestions.emojis.upvote",
                table: "guilds");

            migrationBuilder.DropColumn(
                name: "suggestions.on-action.dm",
                table: "guilds");

            migrationBuilder.DropColumn(
                name: "suggestions.on-action.hide-author",
                table: "guilds");

            migrationBuilder.DropColumn(
                name: "suggestions.on-action.repost",
                table: "guilds");

            migrationBuilder.DropColumn(
                name: "trigger.alias",
                table: "guilds");

            migrationBuilder.DropColumn(
                name: "trigger.includes",
                table: "guilds");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "afk.prefix",
                table: "guilds",
                type: "character varying(32)",
                maxLength: 32,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "afk.prefix-force",
                table: "guilds",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "afk.role",
                table: "guilds",
                type: "character varying(19)",
                maxLength: 19,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "birthday.channel",
                table: "guilds",
                type: "character varying(19)",
                maxLength: 19,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "birthday.message",
                table: "guilds",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "birthday.role",
                table: "guilds",
                type: "character varying(19)",
                maxLength: 19,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "channels.announcements",
                table: "guilds",
                type: "character varying(19)",
                maxLength: 19,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "channels.farewell",
                table: "guilds",
                type: "character varying(19)",
                maxLength: 19,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "channels.greeting",
                table: "guilds",
                type: "character varying(19)",
                maxLength: 19,
                nullable: true);

            migrationBuilder.AddColumn<string[]>(
                name: "channels.ignore.all",
                table: "guilds",
                type: "character varying(19)[]",
                nullable: false,
                defaultValue: new string[0]);

            migrationBuilder.AddColumn<string[]>(
                name: "channels.ignore.message-delete",
                table: "guilds",
                type: "character varying(19)[]",
                nullable: false,
                defaultValue: new string[0]);

            migrationBuilder.AddColumn<string[]>(
                name: "channels.ignore.message-edit",
                table: "guilds",
                type: "character varying(19)[]",
                nullable: false,
                defaultValue: new string[0]);

            migrationBuilder.AddColumn<string[]>(
                name: "channels.ignore.reaction-add",
                table: "guilds",
                type: "character varying(19)[]",
                nullable: false,
                defaultValue: new string[0]);

            migrationBuilder.AddColumn<string>(
                name: "channels.logs.channel-create",
                table: "guilds",
                type: "character varying(19)",
                maxLength: 19,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "channels.logs.channel-delete",
                table: "guilds",
                type: "character varying(19)",
                maxLength: 19,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "channels.logs.channel-update",
                table: "guilds",
                type: "character varying(19)",
                maxLength: 19,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "channels.logs.emoji-create",
                table: "guilds",
                type: "character varying(19)",
                maxLength: 19,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "channels.logs.emoji-delete",
                table: "guilds",
                type: "character varying(19)",
                maxLength: 19,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "channels.logs.emoji-update",
                table: "guilds",
                type: "character varying(19)",
                maxLength: 19,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "channels.logs.image",
                table: "guilds",
                type: "character varying(19)",
                maxLength: 19,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "channels.logs.member-add",
                table: "guilds",
                type: "character varying(19)",
                maxLength: 19,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "channels.logs.member-nickname-update",
                table: "guilds",
                type: "character varying(19)",
                maxLength: 19,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "channels.logs.member-remove",
                table: "guilds",
                type: "character varying(19)",
                maxLength: 19,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "channels.logs.member-roles-update",
                table: "guilds",
                type: "character varying(19)",
                maxLength: 19,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "channels.logs.member-username-update",
                table: "guilds",
                type: "character varying(19)",
                maxLength: 19,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "channels.logs.message-delete",
                table: "guilds",
                type: "character varying(19)",
                maxLength: 19,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "channels.logs.message-delete-nsfw",
                table: "guilds",
                type: "character varying(19)",
                maxLength: 19,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "channels.logs.message-update",
                table: "guilds",
                type: "character varying(19)",
                maxLength: 19,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "channels.logs.message-update-nsfw",
                table: "guilds",
                type: "character varying(19)",
                maxLength: 19,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "channels.logs.moderation",
                table: "guilds",
                type: "character varying(19)",
                maxLength: 19,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "channels.logs.prune",
                table: "guilds",
                type: "character varying(19)",
                maxLength: 19,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "channels.logs.reaction",
                table: "guilds",
                type: "character varying(19)",
                maxLength: 19,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "channels.logs.role-create",
                table: "guilds",
                type: "character varying(19)",
                maxLength: 19,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "channels.logs.role-delete",
                table: "guilds",
                type: "character varying(19)",
                maxLength: 19,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "channels.logs.role-update",
                table: "guilds",
                type: "character varying(19)",
                maxLength: 19,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "channels.logs.server-update",
                table: "guilds",
                type: "character varying(19)",
                maxLength: 19,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "channels.spam",
                table: "guilds",
                type: "character varying(19)",
                maxLength: 19,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "command-auto-delete",
                table: "guilds",
                type: "jsonb",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "custom-commands",
                table: "guilds",
                type: "jsonb",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "disable-natural-prefix",
                table: "guilds",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string[]>(
                name: "disabled-channels",
                table: "guilds",
                type: "character varying(19)[]",
                nullable: false,
                defaultValue: new string[0]);

            migrationBuilder.AddColumn<string[]>(
                name: "disabled-commands",
                table: "guilds",
                type: "character varying(32)[]",
                nullable: false,
                defaultValue: new string[0]);

            migrationBuilder.AddColumn<string>(
                name: "disabled-commands-channels",
                table: "guilds",
                type: "jsonb",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "events.ban-add",
                table: "guilds",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "events.ban-remove",
                table: "guilds",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "events.member-username-update",
                table: "guilds",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "events.twemoji-reactions",
                table: "guilds",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "language",
                table: "guilds",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "messages.announcement-embed",
                table: "guilds",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "messages.auto-delete.ignored-all",
                table: "guilds",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string[]>(
                name: "messages.auto-delete.ignored-channels",
                table: "guilds",
                type: "character varying(19)[]",
                nullable: false,
                defaultValue: new string[0]);

            migrationBuilder.AddColumn<string[]>(
                name: "messages.auto-delete.ignored-commands",
                table: "guilds",
                type: "character varying(32)[]",
                nullable: false,
                defaultValue: new string[0]);

            migrationBuilder.AddColumn<string[]>(
                name: "messages.auto-delete.ignored-roles",
                table: "guilds",
                type: "character varying(19)[]",
                nullable: false,
                defaultValue: new string[0]);

            migrationBuilder.AddColumn<string>(
                name: "messages.farewell",
                table: "guilds",
                type: "character varying(2000)",
                maxLength: 2000,
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "messages.farewell-auto-delete",
                table: "guilds",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "messages.greeting",
                table: "guilds",
                type: "character varying(2000)",
                maxLength: 2000,
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "messages.greeting-auto-delete",
                table: "guilds",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<string[]>(
                name: "messages.ignore-channels",
                table: "guilds",
                type: "character varying(19)[]",
                nullable: false,
                defaultValue: new string[0]);

            migrationBuilder.AddColumn<string>(
                name: "messages.join-dm",
                table: "guilds",
                type: "character varying(1500)",
                maxLength: 1500,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "messages.moderation-auto-delete",
                table: "guilds",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "messages.moderation-dm",
                table: "guilds",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "messages.moderation-message-display",
                table: "guilds",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "messages.moderation-reason-display",
                table: "guilds",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "messages.moderator-name-display",
                table: "guilds",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "music.allow-streams",
                table: "guilds",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string[]>(
                name: "music.allowed-roles",
                table: "guilds",
                type: "character varying(19)[]",
                nullable: false,
                defaultValue: new string[0]);

            migrationBuilder.AddColumn<string[]>(
                name: "music.allowed-voice-channels",
                table: "guilds",
                type: "character varying(19)[]",
                nullable: false,
                defaultValue: new string[0]);

            migrationBuilder.AddColumn<short>(
                name: "music.default-volume",
                table: "guilds",
                type: "smallint",
                nullable: false,
                defaultValue: (short)0);

            migrationBuilder.AddColumn<int>(
                name: "music.maximum-duration",
                table: "guilds",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<short>(
                name: "music.maximum-entries-per-user",
                table: "guilds",
                type: "smallint",
                nullable: false,
                defaultValue: (short)0);

            migrationBuilder.AddColumn<bool>(
                name: "no-mention-spam.alerts",
                table: "guilds",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "no-mention-spam.enabled",
                table: "guilds",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<short>(
                name: "no-mention-spam.mentions-allowed",
                table: "guilds",
                type: "smallint",
                nullable: false,
                defaultValue: (short)0);

            migrationBuilder.AddColumn<int>(
                name: "no-mention-spam.time-period",
                table: "guilds",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "notifications.streams.twitch.streamers",
                table: "guilds",
                type: "jsonb",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "permissions.roles",
                table: "guilds",
                type: "jsonb",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "permissions.users",
                table: "guilds",
                type: "jsonb",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "prefix",
                table: "guilds",
                type: "character varying(10)",
                maxLength: 10,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "reaction-roles",
                table: "guilds",
                type: "jsonb",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string[]>(
                name: "roles.admin",
                table: "guilds",
                type: "character varying(19)[]",
                nullable: false,
                defaultValue: new string[0]);

            migrationBuilder.AddColumn<string>(
                name: "roles.auto",
                table: "guilds",
                type: "jsonb",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string[]>(
                name: "roles.dj",
                table: "guilds",
                type: "character varying(19)[]",
                nullable: false,
                defaultValue: new string[0]);

            migrationBuilder.AddColumn<string>(
                name: "roles.initial",
                table: "guilds",
                type: "character varying(19)",
                maxLength: 19,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "roles.initial-bots",
                table: "guilds",
                type: "character varying(19)",
                maxLength: 19,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "roles.initial-humans",
                table: "guilds",
                type: "character varying(19)",
                maxLength: 19,
                nullable: true);

            migrationBuilder.AddColumn<string[]>(
                name: "roles.moderator",
                table: "guilds",
                type: "character varying(19)[]",
                nullable: false,
                defaultValue: new string[0]);

            migrationBuilder.AddColumn<string>(
                name: "roles.muted",
                table: "guilds",
                type: "character varying(19)",
                maxLength: 19,
                nullable: true);

            migrationBuilder.AddColumn<string[]>(
                name: "roles.public",
                table: "guilds",
                type: "character varying(19)[]",
                nullable: false,
                defaultValue: new string[0]);

            migrationBuilder.AddColumn<bool>(
                name: "roles.remove-initial",
                table: "guilds",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "roles.restricted-attachment",
                table: "guilds",
                type: "character varying(19)",
                maxLength: 19,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "roles.restricted-embed",
                table: "guilds",
                type: "character varying(19)",
                maxLength: 19,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "roles.restricted-emoji",
                table: "guilds",
                type: "character varying(19)",
                maxLength: 19,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "roles.restricted-reaction",
                table: "guilds",
                type: "character varying(19)",
                maxLength: 19,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "roles.restricted-voice",
                table: "guilds",
                type: "character varying(19)",
                maxLength: 19,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "roles.subscriber",
                table: "guilds",
                type: "character varying(19)",
                maxLength: 19,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "roles.unique-role-sets",
                table: "guilds",
                type: "jsonb",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "selfmod.attachments.enabled",
                table: "guilds",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<short>(
                name: "selfmod.attachments.hard-action",
                table: "guilds",
                type: "smallint",
                nullable: false,
                defaultValue: (short)0);

            migrationBuilder.AddColumn<long>(
                name: "selfmod.attachments.hard-action-duration",
                table: "guilds",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<string[]>(
                name: "selfmod.attachments.ignored-channels",
                table: "guilds",
                type: "character varying(19)[]",
                nullable: false,
                defaultValue: new string[0]);

            migrationBuilder.AddColumn<string[]>(
                name: "selfmod.attachments.ignored-roles",
                table: "guilds",
                type: "character varying(19)[]",
                nullable: false,
                defaultValue: new string[0]);

            migrationBuilder.AddColumn<short>(
                name: "selfmod.attachments.soft-action",
                table: "guilds",
                type: "smallint",
                nullable: false,
                defaultValue: (short)0);

            migrationBuilder.AddColumn<int>(
                name: "selfmod.attachments.threshold-duration",
                table: "guilds",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<short>(
                name: "selfmod.attachments.threshold-maximum",
                table: "guilds",
                type: "smallint",
                nullable: false,
                defaultValue: (short)0);

            migrationBuilder.AddColumn<bool>(
                name: "selfmod.capitals.enabled",
                table: "guilds",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<short>(
                name: "selfmod.capitals.hard-action",
                table: "guilds",
                type: "smallint",
                nullable: false,
                defaultValue: (short)0);

            migrationBuilder.AddColumn<long>(
                name: "selfmod.capitals.hard-action-duration",
                table: "guilds",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<string[]>(
                name: "selfmod.capitals.ignored-channels",
                table: "guilds",
                type: "character varying(19)[]",
                nullable: false,
                defaultValue: new string[0]);

            migrationBuilder.AddColumn<string[]>(
                name: "selfmod.capitals.ignored-roles",
                table: "guilds",
                type: "character varying(19)[]",
                nullable: false,
                defaultValue: new string[0]);

            migrationBuilder.AddColumn<short>(
                name: "selfmod.capitals.maximum",
                table: "guilds",
                type: "smallint",
                nullable: false,
                defaultValue: (short)0);

            migrationBuilder.AddColumn<short>(
                name: "selfmod.capitals.minimum",
                table: "guilds",
                type: "smallint",
                nullable: false,
                defaultValue: (short)0);

            migrationBuilder.AddColumn<short>(
                name: "selfmod.capitals.soft-action",
                table: "guilds",
                type: "smallint",
                nullable: false,
                defaultValue: (short)0);

            migrationBuilder.AddColumn<int>(
                name: "selfmod.capitals.threshold-duration",
                table: "guilds",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<short>(
                name: "selfmod.capitals.threshold-maximum",
                table: "guilds",
                type: "smallint",
                nullable: false,
                defaultValue: (short)0);

            migrationBuilder.AddColumn<bool>(
                name: "selfmod.filter.enabled",
                table: "guilds",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<short>(
                name: "selfmod.filter.hard-action",
                table: "guilds",
                type: "smallint",
                nullable: false,
                defaultValue: (short)0);

            migrationBuilder.AddColumn<long>(
                name: "selfmod.filter.hard-action-duration",
                table: "guilds",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<string[]>(
                name: "selfmod.filter.ignored-channels",
                table: "guilds",
                type: "character varying(19)[]",
                nullable: false,
                defaultValue: new string[0]);

            migrationBuilder.AddColumn<string[]>(
                name: "selfmod.filter.ignored-roles",
                table: "guilds",
                type: "character varying(19)[]",
                nullable: false,
                defaultValue: new string[0]);

            migrationBuilder.AddColumn<string[]>(
                name: "selfmod.filter.raw",
                table: "guilds",
                type: "character varying(32)[]",
                nullable: false,
                defaultValue: new string[0]);

            migrationBuilder.AddColumn<short>(
                name: "selfmod.filter.soft-action",
                table: "guilds",
                type: "smallint",
                nullable: false,
                defaultValue: (short)0);

            migrationBuilder.AddColumn<int>(
                name: "selfmod.filter.threshold-duration",
                table: "guilds",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<short>(
                name: "selfmod.filter.threshold-maximum",
                table: "guilds",
                type: "smallint",
                nullable: false,
                defaultValue: (short)0);

            migrationBuilder.AddColumn<string[]>(
                name: "selfmod.ignored-channels",
                table: "guilds",
                type: "character varying(19)[]",
                nullable: false,
                defaultValue: new string[0]);

            migrationBuilder.AddColumn<bool>(
                name: "selfmod.invites.enabled",
                table: "guilds",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<short>(
                name: "selfmod.invites.hard-action",
                table: "guilds",
                type: "smallint",
                nullable: false,
                defaultValue: (short)0);

            migrationBuilder.AddColumn<long>(
                name: "selfmod.invites.hard-action-duration",
                table: "guilds",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<string[]>(
                name: "selfmod.invites.ignored-channels",
                table: "guilds",
                type: "character varying(19)[]",
                nullable: false,
                defaultValue: new string[0]);

            migrationBuilder.AddColumn<string[]>(
                name: "selfmod.invites.ignored-codes",
                table: "guilds",
                type: "character varying[]",
                nullable: false,
                defaultValue: new string[0]);

            migrationBuilder.AddColumn<string[]>(
                name: "selfmod.invites.ignored-guilds",
                table: "guilds",
                type: "character varying(19)[]",
                nullable: false,
                defaultValue: new string[0]);

            migrationBuilder.AddColumn<string[]>(
                name: "selfmod.invites.ignored-roles",
                table: "guilds",
                type: "character varying(19)[]",
                nullable: false,
                defaultValue: new string[0]);

            migrationBuilder.AddColumn<short>(
                name: "selfmod.invites.soft-action",
                table: "guilds",
                type: "smallint",
                nullable: false,
                defaultValue: (short)0);

            migrationBuilder.AddColumn<int>(
                name: "selfmod.invites.threshold-duration",
                table: "guilds",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<short>(
                name: "selfmod.invites.threshold-maximum",
                table: "guilds",
                type: "smallint",
                nullable: false,
                defaultValue: (short)0);

            migrationBuilder.AddColumn<string[]>(
                name: "selfmod.links.allowed",
                table: "guilds",
                type: "character varying(128)[]",
                nullable: false,
                defaultValue: new string[0]);

            migrationBuilder.AddColumn<bool>(
                name: "selfmod.links.enabled",
                table: "guilds",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<short>(
                name: "selfmod.links.hard-action",
                table: "guilds",
                type: "smallint",
                nullable: false,
                defaultValue: (short)0);

            migrationBuilder.AddColumn<long>(
                name: "selfmod.links.hard-action-duration",
                table: "guilds",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<string[]>(
                name: "selfmod.links.ignored-channels",
                table: "guilds",
                type: "character varying(19)[]",
                nullable: false,
                defaultValue: new string[0]);

            migrationBuilder.AddColumn<string[]>(
                name: "selfmod.links.ignored-roles",
                table: "guilds",
                type: "character varying(19)[]",
                nullable: false,
                defaultValue: new string[0]);

            migrationBuilder.AddColumn<short>(
                name: "selfmod.links.soft-action",
                table: "guilds",
                type: "smallint",
                nullable: false,
                defaultValue: (short)0);

            migrationBuilder.AddColumn<int>(
                name: "selfmod.links.threshold-duration",
                table: "guilds",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<short>(
                name: "selfmod.links.threshold-maximum",
                table: "guilds",
                type: "smallint",
                nullable: false,
                defaultValue: (short)0);

            migrationBuilder.AddColumn<bool>(
                name: "selfmod.messages.enabled",
                table: "guilds",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<short>(
                name: "selfmod.messages.hard-action",
                table: "guilds",
                type: "smallint",
                nullable: false,
                defaultValue: (short)0);

            migrationBuilder.AddColumn<long>(
                name: "selfmod.messages.hard-action-duration",
                table: "guilds",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<string[]>(
                name: "selfmod.messages.ignored-channels",
                table: "guilds",
                type: "character varying(19)[]",
                nullable: false,
                defaultValue: new string[0]);

            migrationBuilder.AddColumn<string[]>(
                name: "selfmod.messages.ignored-roles",
                table: "guilds",
                type: "character varying(19)[]",
                nullable: false,
                defaultValue: new string[0]);

            migrationBuilder.AddColumn<short>(
                name: "selfmod.messages.maximum",
                table: "guilds",
                type: "smallint",
                nullable: false,
                defaultValue: (short)0);

            migrationBuilder.AddColumn<short>(
                name: "selfmod.messages.queue-size",
                table: "guilds",
                type: "smallint",
                nullable: false,
                defaultValue: (short)0);

            migrationBuilder.AddColumn<short>(
                name: "selfmod.messages.soft-action",
                table: "guilds",
                type: "smallint",
                nullable: false,
                defaultValue: (short)0);

            migrationBuilder.AddColumn<int>(
                name: "selfmod.messages.threshold-duration",
                table: "guilds",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<short>(
                name: "selfmod.messages.threshold-maximum",
                table: "guilds",
                type: "smallint",
                nullable: false,
                defaultValue: (short)0);

            migrationBuilder.AddColumn<bool>(
                name: "selfmod.newlines.enabled",
                table: "guilds",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<short>(
                name: "selfmod.newlines.hard-action",
                table: "guilds",
                type: "smallint",
                nullable: false,
                defaultValue: (short)0);

            migrationBuilder.AddColumn<long>(
                name: "selfmod.newlines.hard-action-duration",
                table: "guilds",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<string[]>(
                name: "selfmod.newlines.ignored-channels",
                table: "guilds",
                type: "character varying(19)[]",
                nullable: false,
                defaultValue: new string[0]);

            migrationBuilder.AddColumn<string[]>(
                name: "selfmod.newlines.ignored-roles",
                table: "guilds",
                type: "character varying(19)[]",
                nullable: false,
                defaultValue: new string[0]);

            migrationBuilder.AddColumn<short>(
                name: "selfmod.newlines.maximum",
                table: "guilds",
                type: "smallint",
                nullable: false,
                defaultValue: (short)0);

            migrationBuilder.AddColumn<short>(
                name: "selfmod.newlines.soft-action",
                table: "guilds",
                type: "smallint",
                nullable: false,
                defaultValue: (short)0);

            migrationBuilder.AddColumn<int>(
                name: "selfmod.newlines.threshold-duration",
                table: "guilds",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<short>(
                name: "selfmod.newlines.threshold-maximum",
                table: "guilds",
                type: "smallint",
                nullable: false,
                defaultValue: (short)0);

            migrationBuilder.AddColumn<string[]>(
                name: "selfmod.reactions.allowed",
                table: "guilds",
                type: "character varying(128)[]",
                nullable: false,
                defaultValue: new string[0]);

            migrationBuilder.AddColumn<string[]>(
                name: "selfmod.reactions.blocked",
                table: "guilds",
                type: "character varying(128)[]",
                nullable: false,
                defaultValue: new string[0]);

            migrationBuilder.AddColumn<bool>(
                name: "selfmod.reactions.enabled",
                table: "guilds",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<short>(
                name: "selfmod.reactions.hard-action",
                table: "guilds",
                type: "smallint",
                nullable: false,
                defaultValue: (short)0);

            migrationBuilder.AddColumn<long>(
                name: "selfmod.reactions.hard-action-duration",
                table: "guilds",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<string[]>(
                name: "selfmod.reactions.ignored-channels",
                table: "guilds",
                type: "character varying(19)[]",
                nullable: false,
                defaultValue: new string[0]);

            migrationBuilder.AddColumn<string[]>(
                name: "selfmod.reactions.ignored-roles",
                table: "guilds",
                type: "character varying(19)[]",
                nullable: false,
                defaultValue: new string[0]);

            migrationBuilder.AddColumn<short>(
                name: "selfmod.reactions.maximum",
                table: "guilds",
                type: "smallint",
                nullable: false,
                defaultValue: (short)0);

            migrationBuilder.AddColumn<short>(
                name: "selfmod.reactions.soft-action",
                table: "guilds",
                type: "smallint",
                nullable: false,
                defaultValue: (short)0);

            migrationBuilder.AddColumn<int>(
                name: "selfmod.reactions.threshold-duration",
                table: "guilds",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<short>(
                name: "selfmod.reactions.threshold-maximum",
                table: "guilds",
                type: "smallint",
                nullable: false,
                defaultValue: (short)0);

            migrationBuilder.AddColumn<string>(
                name: "social.achieve-channel",
                table: "guilds",
                type: "character varying(19)",
                maxLength: 19,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "social.achieve-level",
                table: "guilds",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<short>(
                name: "social.achieve-multiple",
                table: "guilds",
                type: "smallint",
                nullable: false,
                defaultValue: (short)0);

            migrationBuilder.AddColumn<string>(
                name: "social.achieve-role",
                table: "guilds",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "social.enabled",
                table: "guilds",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string[]>(
                name: "social.ignored-channels",
                table: "guilds",
                type: "character varying(19)[]",
                nullable: false,
                defaultValue: new string[0]);

            migrationBuilder.AddColumn<string[]>(
                name: "social.ignored-roles",
                table: "guilds",
                type: "character varying(19)[]",
                nullable: false,
                defaultValue: new string[0]);

            migrationBuilder.AddColumn<decimal>(
                name: "social.multiplier",
                table: "guilds",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "starboard.channel",
                table: "guilds",
                type: "character varying(19)",
                maxLength: 19,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "starboard.emoji",
                table: "guilds",
                type: "character varying(75)",
                maxLength: 75,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string[]>(
                name: "starboard.ignored-channels",
                table: "guilds",
                type: "character varying(19)[]",
                nullable: false,
                defaultValue: new string[0]);

            migrationBuilder.AddColumn<long>(
                name: "starboard.maximum-age",
                table: "guilds",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<short>(
                name: "starboard.minimum",
                table: "guilds",
                type: "smallint",
                nullable: false,
                defaultValue: (short)0);

            migrationBuilder.AddColumn<bool>(
                name: "starboard.self-star",
                table: "guilds",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "sticky-roles",
                table: "guilds",
                type: "jsonb",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "suggestions.channel",
                table: "guilds",
                type: "character varying(19)",
                maxLength: 19,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "suggestions.emojis.downvote",
                table: "guilds",
                type: "character varying(128)",
                maxLength: 128,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "suggestions.emojis.upvote",
                table: "guilds",
                type: "character varying(128)",
                maxLength: 128,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "suggestions.on-action.dm",
                table: "guilds",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "suggestions.on-action.hide-author",
                table: "guilds",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "suggestions.on-action.repost",
                table: "guilds",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "trigger.alias",
                table: "guilds",
                type: "jsonb",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "trigger.includes",
                table: "guilds",
                type: "jsonb",
                nullable: false,
                defaultValue: "");
        }
    }
}
