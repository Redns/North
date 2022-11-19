using North.Core.Models;

namespace North.Plugin
{
    /// <summary>
    /// 图片解析器
    /// </summary>
    public interface IParse
    {
        /// <summary>
        /// 解析图片信息
        /// </summary>
        /// <param name="image">待解析的图片</param>
        /// <returns></returns>
        Task ParseAsync(ImageUploadModel image);
    }
}
