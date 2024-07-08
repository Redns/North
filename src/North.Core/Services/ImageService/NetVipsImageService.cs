using NetVips;

namespace North.Core.Services.ImageService
{
    public class NetVipsImageService : IImageService
    {
        /// <summary>
        /// 加载图片
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public Image Load(Stream stream)
        {
            return Image.NewFromStream(stream);
        }

        /// <summary>
        /// 图像裁剪
        /// </summary>
        /// <param name="image">待裁剪图像</param>
        /// <param name="size">裁剪后尺寸</param>
        /// <returns></returns>
        public Image Resize(Image image, int width, int height)
        {
            if (width <= 0 || height <= 0)
            {
                throw new ArgumentException("Image resize failed, size must be greater than 0");
            }

            if (image.Width == width && image.Height == height)
            {
                return image;
            }

            /* 计算裁剪尺寸 */
            int imageResizeWidth, imageResizeHeight;
            var imageSrcRatio = image.Width * 1.0 / image.Height;
            var imageDstRatio = width * 1.0 / height;
            // 修改图片纵横比
            if (imageSrcRatio < imageDstRatio)
            {
                imageResizeWidth = image.Width;
                imageResizeHeight = (int)(imageResizeWidth / imageDstRatio);
            }
            else
            {
                imageResizeHeight = image.Height;
                imageResizeWidth = (int)(imageResizeHeight * imageDstRatio);
            }

            // 计算裁剪坐标
            var imageScale = width * 1.0 / imageResizeWidth;
            var imageResizeLeft = (image.Width - imageResizeWidth) / 2;
            var imageResizeTop = (image.Height - imageResizeHeight) / 2;

            /* 裁剪图片 */
            return image.Crop(imageResizeLeft, imageResizeTop, imageResizeWidth, imageResizeHeight)
                        .Resize(imageScale);
        }
    }
}
