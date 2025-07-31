using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using AIToolbox.Models;
using AIToolbox.Plugins;
using AIToolbox.Services;
using Microsoft.Extensions.AI;
using ModelContextProtocol.Client;
using WebApplication2.IServices;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddLogging(b => b.AddConsole());
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "Your API Name", Version = "v1" });
});
builder.Services.AddCors(options =>
{
    options.AddPolicy("ReactApp", policy => policy.WithOrigins("http://localhost:5173").AllowAnyMethod().AllowAnyHeader().AllowCredentials());
});

builder.Services.AddDbContext<AppDbContext>(opt => opt.UseNpgsql(builder.Configuration.GetConnectionString("Db"), o => o.UseVector()));
builder.Services.AddScoped<TodoService>();
builder.Services.AddScoped<IAgentService, OpenAIService>();
builder.Services.AddScoped<IFileService , FileService>();
builder.Services.AddScoped<ILogService, SeqService>();
builder.Services.AddScoped<IVCService, GitService>();
builder.Services.AddScoped<DataAnalysisService>();
builder.Services.AddHttpContextAccessor();

builder.Services.AddScoped<TodoPlugin>();
builder.Services.AddScoped<FilePlugin>();
builder.Services.AddScoped<SeqPlugin>();
builder.Services.AddScoped<VCSPlugin>();

IMcpClient mcpClient = await McpClientFactory.CreateAsync(new StdioClientTransport(new()
{
    Command = "docker", Arguments =
    [
        "run",
        "-i",
        "--rm",
        "-e",
        "POSTGRES_URL",
        "mcp/postgres",
        "postgres://postgres:admin@localhost:32774/postgres"
    ],
    Name = "Postgres MCP server"
    }));

IList<McpClientTool> tools = await mcpClient.ListToolsAsync();

IMcpClient gitlabMcp = await McpClientFactory.CreateAsync(new StdioClientTransport(new()
{
    Command = "docker",
    Arguments =
    [
        "run",
        "-i",
        "--rm",
        "-e",
        "GITLAB_PERSONAL_ACCESS_TOKEN",
        "-e",
        "GITLAB_API_URL",
        "-e",
        "GITLAB_READ_ONLY_MODE",
        "-e",
        "USE_GITLAB_WIKI",
        "-e",
        "USE_MILESTONE",
        "-e",
        "USE_PIPELINE",
        "iwakitakuma/gitlab-mcp"
    ],
    EnvironmentVariables = new Dictionary<string, string?>()
    {
        ["GITLAB_PERSONAL_ACCESS_TOKEN"] = "glpat-pmuWI0Aw8Ddw60PUoae2nm86MQp1OmZyYm16Cw.01.120pjr34p",
        ["USE_READ_ONLY_MODE"] = "true",
        ["USE_GITLAB_WIKI"] = "true",
        ["USE_MILESTONE"] = "true",
        ["USE_PIPELINE"] = "true",
    },
    Name = "Gitlab-tools"
}));

IList<McpClientTool> gitlabTools = await gitlabMcp.ListToolsAsync();

gitlabTools = gitlabTools.Where(t => t.Name == "get_merge_request_diffs" || t.Name == "list_merge_requests" || t.Name == "create_note" || t.Name == "list_projects").ToList();


builder.Services.AddScoped(sp =>
{
    //Kernel kernel = Kernel.CreateBuilder() .AddGoogleAIGeminiChatCompletion(modelId: "gemini-2.5-flash", apiKey: builder.Configuration["Gemini:ApiKey"]).Build();
    Kernel kernel = Kernel.CreateBuilder().AddOpenAIChatCompletion(modelId: "o4-mini", apiKey: builder.Configuration["OpenAI:ApiKey"]).Build();

    var todoPlugin = sp.GetRequiredService<TodoPlugin>();
    var filePlugin = sp.GetRequiredService<FilePlugin>();
    var  seqPlugin = sp.GetRequiredService<SeqPlugin>();
    var vcsplugin = sp.GetRequiredService<VCSPlugin>();
    
    
    kernel.Plugins.AddFromObject(todoPlugin);
    kernel.Plugins.AddFromObject(filePlugin);
    kernel.Plugins.AddFromObject(seqPlugin);
    kernel.Plugins.AddFromObject(vcsplugin);
#pragma warning disable SKEXP0001
    kernel.Plugins.AddFromFunctions("PostgresPlugin", tools.Select(tool => tool.AsKernelFunction()));
#pragma warning restore SKEXP0001
    
#pragma warning disable SKEXP0001
    kernel.Plugins.AddFromFunctions("GitlabPlugin", gitlabTools.Select(tool => tool.AsKernelFunction()));
#pragma warning restore SKEXP0001
    
    return kernel;
});
//builder.Services.AddGoogleAIEmbeddingGenerator(modelId:  "text-embedding-004", apiKey: builder.Configuration["Gemini:ApiKey"] );
#pragma warning disable SKEXP0010
builder.Services.AddOpenAIEmbeddingGenerator(modelId:  "text-embedding-3-small", apiKey: builder.Configuration["OpenAI:ApiKey"], dimensions: 768);
#pragma warning restore SKEXP0010


builder.Services.AddAuthentication().AddCookie(IdentityConstants.ApplicationScheme);
builder.Services.AddAuthorizationBuilder();
builder.Services.AddIdentityCore<User>().AddEntityFrameworkStores<AppDbContext>().AddApiEndpoints();



var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();
app.UseAuthentication();

app.MapIdentityApi<User>();
app.MapControllers();

app.UseCors("ReactApp");

app.Run();