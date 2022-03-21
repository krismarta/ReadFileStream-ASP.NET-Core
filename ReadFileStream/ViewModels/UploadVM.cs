using Microsoft.AspNetCore.Http;

namespace ReadFileStream.ViewModels
{
    public class UploadVM
    {
        public IFormFile filedata { get; set; }
    }
}
