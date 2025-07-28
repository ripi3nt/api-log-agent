using System.ComponentModel.DataAnnotations.Schema;
using Pgvector;

namespace AIToolbox.Models;

public class StoredFile
{
    public int Id { get; set; }
    public required string FileName { get; set; }
    public required string FileContent { get; set; }
    public required string UserId { get; set; }
    public DateTime uploadTime { get; set; }
    public required ICollection<StoredChunk> Chunks { get; set; }
}
