using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Processing;

namespace UploadImageDemo.Services
{
    public class ImageService : IImageService
    {
        #region Property
        private readonly string _directory = @"/Images/";
        private readonly string _path;
        private readonly IUriService _uriService;
        #endregion

        #region Constructor
        public ImageService(IWebHostEnvironment env, IUriService uriService)
        {
            this._path = string.Concat(env.WebRootPath, _directory);
            this._uriService = uriService;
        }
        #endregion

        #region Method
        public async Task<(bool isSuccess, Uri uri)> UploadImageAsync(Stream imageStream)
        {
            try
            {
                string fileName = string.Empty;

                IImageFormat imageFormat;
                using (var rawImage = Image.Load(imageStream, out imageFormat))
                {
                    if (!ValidateImageFormat(imageFormat))
                        return (false, null);

                    var imageSize = GetThumbnailSize(rawImage);

                    rawImage.Mutate(x => x.AutoOrient().Resize(imageSize.Width, imageSize.Height));

                    string fileExtensions = imageFormat.FileExtensions.First();
                    fileName = $"{Guid.NewGuid()}.{fileExtensions}";

                    await rawImage.SaveAsync($"{_path}{fileName}");
                }

                return (true, _uriService.GetRouteUri($"{_directory}{fileName}"));
            }
            catch
            {
                return (false, null);
            }

        }

        private static bool ValidateImageFormat(IImageFormat imageFormat)
        {
            string nameFormat = imageFormat.Name.ToUpper();

            if (nameFormat.Equals("JPEG"))
                return true;

            if (nameFormat.Equals("PNG"))
                return true;

            if (nameFormat.Equals("GIF"))
                return true;

            if (nameFormat.Equals("BMP"))
                return true;

            return false;
        }

        private static Size GetThumbnailSize(Image rawImage, int size = 500)
        {
            var originalWidth = rawImage.Width;
            var originalHeight = rawImage.Height;

            double factor;
            if (originalWidth > originalHeight)
            {
                factor = (double)size / originalWidth;
            }
            else
            {
                factor = (double)size / originalHeight;
            }

            return new Size((int)(originalWidth * factor), (int)(originalHeight * factor));
        }
        #endregion
    }
}
