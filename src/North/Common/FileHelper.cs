namespace North.Common
{
    public class FileHelper
    {
        private readonly string[] Image_Extensions = new string[] { ".png", ".jpg", ".jpeg", ".bmp", ".svg", ".gif" };


        /// <summary>
        /// 判断文件是否为图片
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public bool IsImage(string filename)
        {
            return Image_Extensions.Contains(Path.GetExtension(filename));
        }
    }
}
