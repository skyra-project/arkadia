using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Database.Models.Entities;

[Table("cdn-entry")]
public class CdnEntry
{
	[Column]
	public long Id { get; set; }

	[Column]
	[MaxLength(50)]
	public string Name { get; set; } = null!;

	// RFC 4288 allows Content-Type headers to be a maximum of 127/127 characters, making it 255 total
	[Column]
	[MaxLength(255)]
	public string ContentType { get; set; } = null!;

	[Column]
	public DateTime LastModifiedAt { get; set; }

	// MD5 checksums are 128 bytes, so 32 characters in hex
	[Column]
	[MaxLength(32)]
	public string ETag { get; set; } = null!;
}
