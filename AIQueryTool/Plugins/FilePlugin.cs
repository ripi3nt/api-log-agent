using System.ComponentModel;
using System.Text;
using System.Text.Json;
using AIToolbox.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.AI;
using Microsoft.SemanticKernel;
using Pgvector.EntityFrameworkCore;
using Seq.Api;
using Seq.Api.Model.Data;
using UglyToad.PdfPig;

namespace AIToolbox.Plugins;

public class FilePlugin
{
    AppDbContext _context;
    IEmbeddingGenerator<string, Embedding<float>> _embeddingGenerator;
    ILogger<FilePlugin> _logger;
    public FilePlugin(AppDbContext context, IEmbeddingGenerator<string, Embedding<float>> embeddingGenerator, ILogger<FilePlugin> logger)
    {
        _context = context;
        _embeddingGenerator = embeddingGenerator;
        _logger = logger;
    }
    
    [KernelFunction("DownloadFile")]
    [Description("Gets the text content of a file from database")]
    public string GetFile()
    {
        StoredFile storedFile = _context.StoredFiles.Where(p => p.FileName == "todos.pdf").FirstOrDefault();
        
        string finalText = storedFile.FileContent;
       

        return finalText;
    }

    
    [KernelFunction("SearchRules")]
    [Description("Search bussines rules for writing transactions rules for a card processing company")]
    public async Task<List<StoredChunk>> FindInFile(string textSearchQuery)
    {
        var embedding = await _embeddingGenerator.GenerateAsync(textSearchQuery);
        ReadOnlyMemory<float> embeddingMemory = embedding.Vector;
        Pgvector.Vector vector = new Pgvector.Vector(embeddingMemory);
        
       StoredChunk matchingChunk = _context.StoredChunks.Where(c => c.ClusterNumber == null).OrderBy(p => p.Embeddings.CosineDistance(vector)).First();
       
       List<StoredChunk> matchingChunks = _context.StoredChunks.Where(c => c.ClusterNumber ==  matchingChunk.ClusterNumber).ToList();

       return matchingChunks;
    }
    
    [KernelFunction("SearchLogs")]
    [Description("Does a vector search on text in the database and searches for logs that are the closest. Use this in cases where the user asks for ex. Mastercard but since that is not the value you are searching for exactly use this vector search to figure out on which field a value like Mastercard appears and what is its exact value and then use that when you query elsewhere.")]
    public async Task<List<StoredChunk>> FindLogsWithVectorSearch(string textSearchQuery)
    {
        var embedding = await _embeddingGenerator.GenerateAsync(textSearchQuery);
        ReadOnlyMemory<float> embeddingMemory = embedding.Vector;
        Pgvector.Vector vector = new Pgvector.Vector(embeddingMemory);
        
        List<StoredChunk> matchingChunks = _context.StoredChunks.OrderBy(p => p.Embeddings.CosineDistance(vector)).Take(5).ToList();
       
        _logger.LogInformation("Vector search: {Query}", textSearchQuery);
        //List<StoredChunk> matchingChunks = _context.StoredChunks.Where(c => c.ClusterNumber ==  matchingChunk.ClusterNumber).ToList();

        return matchingChunks;
    }

    [KernelFunction("GetFileNameFromId")]
    public StoredFile GetFileNameFromId(int id)
    {
       return _context.StoredFiles.Where(p => p.Id == id).FirstOrDefault(); 
    }

    [KernelFunction]
    [Description("Checks whether transaction bussiness rule is valid or not")]
    public bool VerifyRule(string rule)
    {
        Console.WriteLine(rule);

        return true;

    }
    
}