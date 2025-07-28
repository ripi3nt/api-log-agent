using System.ComponentModel.DataAnnotations.Schema;
using Pgvector;

namespace AIToolbox.Models;

public class StoredChunk
{
    public int Id {get; set;}
    public StoredFile File {get; set;}
    public string ChunkContent {get; set;}
    [Column(TypeName = "vector(768)")]
    public Vector Embeddings {get; set;}
    public int ClusterNumber {get; set;}
    public string? ClusterMethod {get; set;}
}