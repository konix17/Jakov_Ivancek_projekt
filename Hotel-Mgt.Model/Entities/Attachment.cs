using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HotelMgt.Model.Entities;

public class Attachment
{
    [Key]
    public int Id { get; set; }

    [ForeignKey("Hotel")]
    public int HotelId { get; set; }
    public virtual Hotel Hotel { get; set; } = null!;

    public string FileName { get; set; } = null!;
    public string FilePath { get; set; } = null!;
    public string ContentType { get; set; } = null!;
    public long FileSize { get; set; }
    public DateTime CreatedAt { get; set; }
}
