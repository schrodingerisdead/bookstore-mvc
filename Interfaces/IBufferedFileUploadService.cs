namespace BookStore.Interfaces
{
    public interface IBufferedFileUploadService
    {

            Task<string> UploadFile(IFormFile file);

    }
}
