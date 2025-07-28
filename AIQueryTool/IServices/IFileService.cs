using Microsoft.AspNetCore.Mvc;

namespace WebApplication2.IServices;

public interface IFileService
{
    public Task<IActionResult> UploadFile(IFormFile file);

    public Task<IActionResult> DownloadFile(string filename);

    public Task<IActionResult> RemakeEmbeddings();
}