using System.Security.Claims;
using System.Text.Json;
using Accord.MachineLearning;
using AIToolbox.Models;
using AIToolbox.Utils;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.AI;
using UglyToad.PdfPig;
using WebApplication2.IServices;

namespace AIToolbox.Services;

public class FileService :  IFileService
{
    AppDbContext _context;
    HttpContext _httpContext;
    IEmbeddingGenerator<string, Embedding<float>> _embeddingGenerator;
    public FileService(AppDbContext context, IHttpContextAccessor httpContextAccessor, IEmbeddingGenerator<string, Embedding<float>> embeddingGenerator)
    {
        _context = context;
        _httpContext = httpContextAccessor.HttpContext;
        _embeddingGenerator = embeddingGenerator;
    }

    public async Task<IActionResult> UploadFile(IFormFile file)
    {
        byte[] buffer = new byte[file.Length];
        file.OpenReadStream().ReadAsync(buffer, 0, buffer.Length).Wait();
        
        string fileName = Path.GetFileName(file.FileName);
        string fileContent = Convert.ToBase64String(buffer);
        
        string userId = _httpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
        
        List<StoredFile> chunks = new List<StoredFile>();
        StoredFile storedFile = new StoredFile{FileName = fileName, FileContent = fileContent,  UserId = userId, Chunks = new List<StoredChunk>(), uploadTime = DateTime.UtcNow};

        if (file.FileName.EndsWith(".pdf"))
        {
            Stream stream = new MemoryStream(buffer);
        
            PdfDocument pdf = PdfDocument.Open(stream);
            string finalText = "";
            foreach (var page in pdf.GetPages())
            {
                var embedding = await _embeddingGenerator.GenerateAsync(page.Text, cancellationToken: default);
                ReadOnlyMemory<float> embeddingMemory = embedding.Vector;
                Pgvector.Vector vector = new Pgvector.Vector(embeddingMemory);

                storedFile.Chunks.Add(new StoredChunk{ChunkContent = page.Text, Embeddings = vector});

            } 
        } else if (file.FileName.EndsWith(".json"))
        {
            UploadedJson uploadedJson = JsonSerializer.Deserialize<UploadedJson>(Convert.FromBase64String(fileContent));

            foreach (Rule rule in uploadedJson.Rules)
            {
                var embedding = await _embeddingGenerator.GenerateAsync(JsonSerializer.Serialize(rule), cancellationToken: default);
                ReadOnlyMemory<float> embeddingMemory = embedding.Vector;
                Pgvector.Vector vector = new Pgvector.Vector(embeddingMemory);
                
                storedFile.Chunks.Add(new StoredChunk{ChunkContent = JsonSerializer.Serialize(rule), Embeddings = vector});
                
            }
            
        } else if (file.FileName.EndsWith(".jsonl"))
        {
            Stream stream = file.OpenReadStream();
            StreamReader reader = new StreamReader(stream);
            string? line;
            List<string> lineBatch =  new List<string>();
            while ((line = reader.ReadLine()) != null)
            {
                //var embedding = await _embeddingGenerator.GenerateAsync(line, cancellationToken: default);
                string messageLine = JsonExtractor.GetMessageFromJson(line);
                lineBatch.Add(messageLine);
                if (lineBatch.Count == 200)
                {
                    var embeddings = await _embeddingGenerator.GenerateAndZipAsync(lineBatch);

                    foreach  (var embedding in embeddings)
                    {
                        ReadOnlyMemory<float> embeddingMemory = embedding.Embedding.Vector;
                        Pgvector.Vector vector = new Pgvector.Vector(embeddingMemory);
                        storedFile.Chunks.Add(new StoredChunk{ChunkContent = embedding.Value, Embeddings = vector}); 
                    }
                    
                    lineBatch.Clear();
                }
                
                
            }
            var remainingEmbeddings = await _embeddingGenerator.GenerateAndZipAsync(lineBatch);

            foreach  (var embedding in remainingEmbeddings)
            {
                ReadOnlyMemory<float> embeddingMemory = embedding.Embedding.Vector;
                Pgvector.Vector vector = new Pgvector.Vector(embeddingMemory);
                storedFile.Chunks.Add(new StoredChunk{ChunkContent = embedding.Value, Embeddings = vector}); 
            }          
           
            //KMeansClustering(storedFile);
        }
        else
        {
            return new BadRequestResult();
        }

        _context.StoredFiles.Add(storedFile);
        
        if(_context.SaveChanges() > 0)
        {
            return new OkResult();
        }


        return new BadRequestResult();

    }
    
    public void KMeansClustering(StoredFile uploadedFile)
    {
        var chunksList = uploadedFile.Chunks.ToList();
 
        double[][] embeddingsForClustering = chunksList
            .Select(c => c.Embeddings.ToArray().Select(f => (double)f).ToArray())
            .ToArray();
 
        int numberOfClusters = Math.Min(100, chunksList.Count);
 
        // init k-means
        var kmeans = new KMeans(k: numberOfClusters);
        kmeans.Distance = new Accord.Math.Distances.Euclidean();
        kmeans.MaxIterations = 100;
 
        // compute clusters
        KMeansClusterCollection clusters = kmeans.Learn(embeddingsForClustering);
 
        // get the cluster assignment for each chunk
        int[] assignments = clusters.Decide(embeddingsForClustering);
 
        // update chunk
        for (int i = 0; i < chunksList.Count; i++)
        {
            chunksList[i].ClusterNumber = assignments[i];
            chunksList[i].ClusterMethod = "K-Means (k=" + numberOfClusters + ")";
        }
    } 

    public async Task<IActionResult> DownloadFile(string filename)
    {
        string basefile = _context.StoredFiles.First(x => x.FileName == filename).FileContent;
        
        byte[] fileBytes = Convert.FromBase64String(basefile);
        
        return new FileContentResult(fileBytes, "application/octet-stream");
    }

    public async Task<IActionResult> RemakeEmbeddings()
    {
        foreach (StoredChunk chunk in _context.StoredChunks)
        {
            var embedding = await _embeddingGenerator.GenerateAsync(chunk.ChunkContent, cancellationToken: default);
            ReadOnlyMemory<float> embeddingMemory = embedding.Vector;
            Pgvector.Vector vector = new Pgvector.Vector(embeddingMemory);
            chunk.Embeddings = vector;
        }

        if (_context.SaveChanges() > 0)
        {
            return new OkResult();
        }
        
        return new NotFoundResult();

    }
}

public class UploadedJson
{
    public Rule[] Rules;
}