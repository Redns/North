using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;
using North.Core.Entities;
using North.Core.Helpers;
using North.Core.Models;
using System.Text;

namespace North.Pages
{
    partial class Upload
    {
        private static readonly string DefaultDragClass = "flex-none relative rounded-lg border-2 border-dashed ";
        private string DragClass = DefaultDragClass;
        private bool Clearing = false;
        private List<ImageUploadModel> Images { get; set; } = new(64);

        private async Task OnInputImagesChanged(InputFileChangeEventArgs e)
        {
            ClearDragClass();
            foreach(var file in e.GetMultipleFiles())
            {
                using var stream = file.OpenReadStream(51200000);
                var previewUrl = await JS.UploadToBlob(stream, file.ContentType);
                Images.Add(new ImageUploadModel(file.Name, file.ContentType, previewUrl, stream));
                await InvokeAsync(StateHasChanged);
            }
        }


        /// <summary>
        /// 上传图片
        /// </summary>
        /// <param name="image">待上传的图片</param>
        private void UploadImage(ImageUploadModel image)
        {
            image.State = ImageUploadState.Success;
            image.Progress = 100;
            image.Message = "上传完成";
            image.Url = new ImageUrl(image.Name, image.Name);
        }


        /// <summary>
        /// 清除图片
        /// </summary>
        /// <param name="image"></param>
        private void ClearImage(ImageUploadModel image)
        {
            Images.Remove(image);
            _ = JS.DestroyBlob(image.PreviewUrl);
        }


        /// <summary>
        /// 拷贝图片链接
        /// </summary>
        /// <param name="image">待拷贝的图片</param>
        private async Task CopyImageUrl(ImageUploadModel image)
        {
            var imageAbsoluteUrl = new Uri(new Uri(_nav.BaseUri), image.Url?.Source);
            await JS.CopyToClipboard(imageAbsoluteUrl.AbsoluteUri);
            _snackbar.Add("已拷贝图片链接", Severity.Success);
        }


        private void UploadImages()
        {
            Images.FindAll(image => image.State is ImageUploadState.UnStart or ImageUploadState.Failed)
                  .ForEach(image => UploadImage(image));
        }


        /// <summary>
        /// 清空图片列表
        /// </summary>
        /// <returns></returns>
        private void ClearImages()
        {
            Images.ForEach(image =>
            {
                _ = JS.DestroyBlob(image.PreviewUrl);
            });
            Images.Clear();
        }


        private async void CopyImagesUrl()
        {
            var imageUrls = new StringBuilder();
            Images.FindAll(image => image.State is ImageUploadState.Success)
                  .ForEach(image =>
                  {
                      imageUrls.Append(new Uri(new Uri(_nav.BaseUri), image.Url?.Source).AbsoluteUri);
                  });
            await JS.CopyToClipboard(imageUrls.ToString());
            _snackbar.Add("已拷贝图片链接", Severity.Success);
        }
        

        private void SetDragClass()
        {
        DragClass = $"{DefaultDragClass} mud-border-primary";
        }

        private void ClearDragClass()
        {
        DragClass = DefaultDragClass;
        }
    }
}
