namespace UploadImageDemo.Services
{
    public interface IImageService
    {
        Task<(bool isSuccess, Uri uri)> UploadImageAsync(Stream imageStream);
    }
}
