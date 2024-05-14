using Microsoft.AspNetCore.WebUtilities;

namespace BookStore.Interfaces
{
    public interface IStreamFileUploadService
    {

            Task<bool> UploadFile(MultipartReader reader, MultipartSection section);

    }
}
