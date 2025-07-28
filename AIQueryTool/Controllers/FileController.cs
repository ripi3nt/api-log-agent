using Microsoft.AspNetCore.Mvc;
using Seq.Api;
using Seq.Api.Client;
using WebApplication2.IServices;

namespace WebApplication2.Controllers;

[Route("file")]
[ApiController]
public class FileController : ControllerBase
{
    private readonly IFileService _fileService;
    public FileController(IFileService fileService)
    {
        _fileService = fileService;
    }

    [HttpPost("upload")]
    public async Task<IActionResult> Upload(IFormFile file)
    {
        return await _fileService.UploadFile(file);
    }
    [HttpGet("download/{filename}")]
    public async Task<IActionResult> get([FromRoute] string filename)
    {
        return await _fileService.DownloadFile(filename);
    }

}
