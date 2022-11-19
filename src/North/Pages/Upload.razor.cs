using Microsoft.AspNetCore.Components.Forms;
using North.Core.Models;

namespace North.Pages
{
    partial class Upload
    {
        private static readonly string DefaultDragClass = "flex-none relative rounded-lg border-2 border-dashed ";
        private string DragClass = DefaultDragClass;
        private bool Clearing = false;
        private List<ImageUploadModel> Images { get; set; } = new(64);
        private object UrlType { get; set; } = ImageUrlType.Markdown;

        private async Task OnInputImagesChanged(InputFileChangeEventArgs args)
        {
            
        }


        /// <summary>
        /// 上传图片
        /// </summary>
        /// <param name="image">待上传的图片</param>
        private void UploadImage(ImageUploadModel image)
        {
            
        }


        /// <summary>
        /// 清除图片
        /// </summary>
        /// <param name="image"></param>
        private void ClearImage(ImageUploadModel image)
        {
            
        }


        /// <summary>
        /// 拷贝图片链接
        /// </summary>
        /// <param name="image">待拷贝的图片</param>
        private async Task CopyImageUrl(ImageUploadModel image)
        {
            
        }


        private void UploadImages()
        {
            
        }


        /// <summary>
        /// 清空图片列表
        /// </summary>
        /// <returns></returns>
        private void ClearImages()
        {
            
        }


        private async void CopyImagesUrl()
        {
            
        }
        

        private void SetDragClass()
        {
        
        }

        private void ClearDragClass()
        {
        
        }
    }


    public enum ImageUrlType
    {
        Markdown = 0,
        Url,
        Html,
        UBB
    }
}
