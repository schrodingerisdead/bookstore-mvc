using BookStore.Interfaces;
using Microsoft.AspNetCore.Hosting;

public class BufferedFileUploadLocalService : IBufferedFileUploadService
{
    private readonly IWebHostEnvironment _webHostEnvironment;

    public BufferedFileUploadLocalService(IWebHostEnvironment webHostEnvironment)
    {
        _webHostEnvironment = webHostEnvironment;
    }
    public async Task<string> UploadFile(IFormFile file)
    {
        string path = "";
        try
        {
            if (file.Length > 0)
            {
                //path = Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, "images"));
                path = Path.Combine(_webHostEnvironment.WebRootPath, "uploadedfiles");
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                //var fileName = Path.GetFileName(file.FileName);
                //var filePath = Path.Combine(path, fileName);
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                var filePath = Path.Combine(path, fileName);
                using (var fileStream = new FileStream(Path.Combine(path, fileName), FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
                }
                //return true;
                //return filePath;
                var fileUrl = "/uploadedfiles/" + fileName;
                return fileUrl;

            }
            else
            {
                return null;
            }
        }
        catch (Exception ex)
        {
            throw new Exception("File Copy Failed", ex);
        }
    }
}
