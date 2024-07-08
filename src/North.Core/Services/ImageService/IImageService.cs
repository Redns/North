using NetVips;

namespace North.Core.Services.ImageService
{
    public interface IImageService
    {
        Image Load(Stream stream);

        Image Resize(Image image, int width, int height);
    }
}
