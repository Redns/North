using Microsoft.AspNetCore.Components.Forms;
using North.Core.Helpers;
using North.Core.Models;
using North.Core.Services.AuthService;

namespace North.Pages
{
    partial class Upload
    {
        private static readonly string DefaultDragClass = "flex-none relative rounded-lg border-2 border-dashed ";
        private string DragClass = DefaultDragClass;
        private bool Clearing = false;
        private List<ImageUploadModel> Images { get; set; } = new();
        private object UrlType { get; set; } = ImageUrlType.Markdown;


        /// <summary>
        /// 文件选择回调函数
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        private async Task OnInputImagesChanged(InputFileChangeEventArgs args)
        {
            // 用户授权判定
            var relativeUrl = _nav.ToBaseRelativePath(_nav.Uri).Split('?').First().ToLower();
            if ((_accessor.HttpContext is null) || (await _authService.AuthAsync(_accessor.HttpContext) is not AuthState.Valid))
            {
                _nav.NavigateTo($"/login?redirect={relativeUrl}", true);
                return;
            }

            // 解析输入图片
            await Parallel.ForEachAsync(args.GetMultipleFiles(int.MaxValue), async (file, token) =>
            {
                // 拖拽上传可能包括文件夹，其 ContentType 为空
                if (string.IsNullOrEmpty(file.ContentType))
                {
                    return;
                }
                /* 添加图像至列表 */
                var imageReadStream = file.OpenReadStream(long.MaxValue, token);
                var imagePreviewUrl = await JS.UploadToBlobAsync(imageReadStream, file.ContentType);
                if (string.IsNullOrEmpty(imagePreviewUrl))
                {
                    _snackbar.Add("预览图生成失败", MudBlazor.Severity.Error);
                    return;
                }
                Images.Add(new ImageUploadModel(file, imageReadStream, imagePreviewUrl));
            });
        }


        /// <summary>
        /// 上传图片
        /// </summary>
        /// <param name="model">待上传的图片</param>
        private async Task UploadImageAsync(ImageUploadModel model)
        {
            // 更新图片上传状态
            model.Progress = 0;
            model.Message = "图片上传中...";
            model.State = ImageUploadState.Uploading;
            
            // 解析图片信息（宽度、高度）
            using var image = _imageService.Load(model.Stream);



            // TODO 模拟上传
            // Thread.Sleep(1000);
            await Task.Delay(1000);
            model.State = new Random().Next() % 2 == 0 ? ImageUploadState.Success : ImageUploadState.Failed;
            model.Progress = model.State == ImageUploadState.Success ? 100 : 0;
            model.Message = model.State == ImageUploadState.Success ? "上传成功" : "上传失败";

            // TODO 上传图片
            if(model.State is ImageUploadState.Success && model.Stream is not null)
            {
                await model.Stream.DisposeAsync();
            }
        }


        /// <summary>
        /// 清除指定图片
        /// </summary>
        /// <param name="image"></param>
        private async Task ClearImageAsync(ImageUploadModel image)
        {
            // 销毁图片预览图 blob 文件
            await JS.DestroyBlobAsync(image.PreviewUrl);
            // 释放数据流
            // 在 Success 状态下数据流经由 UploadImageAsync/UploadAllImages 释放
            if(image.State is not ImageUploadState.Success)
            {
                await image.Stream.DisposeAsync();
            }
            Images.Remove(image);
        }


        /// <summary>
        /// 拷贝图片链接
        /// </summary>
        /// <param name="image">待拷贝的图片</param>
        private async Task CopyImageUrlAsync(ImageUploadModel image)
        {
            if(await JS.CopyToClipboardAsync(image.SourceUrl) == image.SourceUrl)
            {
                _snackbar.Add("图片链接已拷贝至剪贴板", MudBlazor.Severity.Success);
            }
            else
            {
                _snackbar.Add("图片链接拷贝失败", MudBlazor.Severity.Error);
            }
        }


        /// <summary>
        /// 上传列表中的全部图片
        /// </summary>
        /// <returns></returns>
        private async Task UploadAllImagesAsync()
        {
            await Parallel.ForEachAsync(Images.FindAll(i => i.State is ImageUploadState.UnStart or ImageUploadState.Failed), async (image, token) =>
            {
                await UploadImageAsync(image);
            });
        }


        /// <summary>
        /// 清空图片列表
        /// </summary>
        /// <returns></returns>
        private async Task ClearAllImagesAsync()
        {
            // 筛选待清除图片
            await Parallel.ForEachAsync(Images.Where(i => i.State is not ImageUploadState.Uploading), async (image, token) =>
            {
                // 清空图片预览 blob
                await JS.DestroyBlobAsync(image.PreviewUrl);
                // 释放图片数据流
                // Success 的图片数据流在上传完成后就被释放，此处仅需要对 Unstart 和 Failed 图片进行释放
                await image.Stream.DisposeAsync();
            });
            Images.RemoveAll(i => i.State is not ImageUploadState.Uploading);
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


    /// <summary>
    /// 图片链接类型
    /// </summary>
    public enum ImageUrlType
    {
        Markdown = 0,   // Markdown 格式 => ![image_name](image_url)
        Url,            // 原始 url => image_url
        Html,           // html img 标签 => <img src="image_url"/>
        UBB             // 论坛 ubb 格式 => [img]image_url[/img]
    }
}
