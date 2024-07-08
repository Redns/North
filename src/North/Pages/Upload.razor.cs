using Masuit.Tools;
using Microsoft.AspNetCore.Components.Forms;
using North.Core.Helpers;
using North.Core.Models;
using North.Core.Repository;

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
            if ((_accessor.HttpContext is null) || (await _authService.AuthAsync(_accessor.HttpContext) is false))
            {
                _snackbar.Add("授权信息无效，请重新登录", MudBlazor.Severity.Error);
                _nav.NavigateTo($"/login?redirect={relativeUrl}", true);
                return;
            }

            // 解析输入图片
            await Parallel.ForEachAsync(args.GetMultipleFiles(int.MaxValue), async (image, token) =>
            {
                var imageReadStream = image.OpenReadStream(long.MaxValue, token);
                var imagePreviewUrl = await JS.UploadToBlobAsync(imageReadStream, image.ContentType);
                Images.Add(new ImageUploadModel()
                {
                    Name = image.Name,
                    ContentType = image.ContentType,
                    PreviewUrl = imagePreviewUrl,
                    Stream = imageReadStream,
                    Message = "等待上传..."
                });
            });
        }


        /// <summary>
        /// 上传图片
        /// </summary>
        /// <param name="image">待上传的图片</param>
        private async Task UploadImageAsync(ImageUploadModel image)
        {
            // 更新图片上传状态
            image.Message = "图片上传中...";
            image.State = ImageUploadState.Uploading;
            
            
            // 解析图片信息（宽度、高度）
            



            // TODO 模拟上传
            Thread.Sleep(1000);
            image.State = new Random().Next() % 2 == 0 ? ImageUploadState.Success : ImageUploadState.Failed;
            image.Progress = image.State == ImageUploadState.Success ? 100 : 0;
            image.Message = image.State == ImageUploadState.Success ? "上传成功" : "上传失败";

            // TODO 上传图片
            if(image.State is ImageUploadState.Success && image.Stream is not null)
            {
                await image.Stream.DisposeAsync();
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
            if(image.State is not ImageUploadState.Success && image.Stream is not null)
            {
                await image.Stream.DisposeAsync();
            }

            // 从列表中移除该图片
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
        private void UploadAllImages()
        {
            // 检索待上传图片（未开始/上传失败）
            // 此处若使用 Where 需要注意，每一次调用 images 均会重新进行筛选
            var images = Images.FindAll(i => i.State is ImageUploadState.UnStart or ImageUploadState.Failed);
            images.ForEach(image =>
            {
                // 重置图片上传状态
                image.State = ImageUploadState.Uploading;
                image.Progress = 0;
                image.Message = "上传中...";
            });

            // TODO 上传图片
            // await _pluginsContext.OnImageUpload(images, null);

            // TODO 此处模拟上传流程
            var random = new Random();
            images.ForEach(image =>
            {
                if (random.Next() % 2 == 0)
                {
                    image.State = ImageUploadState.Success;
                    image.Progress = 100;
                    image.Message = "上传完成";
                }
                else
                {
                    image.State = ImageUploadState.Failed;
                    image.Progress = 0;
                    image.Message = "上传失败";
                }
                Thread.Sleep(200);
            });

            // 释放已完成上传的数据流
            // 释放必须在此处完成，用户未必会清空列表
            images.Where(i => (i.State == ImageUploadState.Success)).ForEach(async image =>
            {
                if (image.Stream is not null)
                {
                    await image.Stream.DisposeAsync();
                }
            });
        }


        /// <summary>
        /// 清空图片列表
        /// </summary>
        /// <returns></returns>
        private void ClearAllImages()
        {
            // 筛选待清除的图片（未开始/上传成功/上传失败）
            Images.Where(i => i.State is not ImageUploadState.Uploading).ForEach(async image =>
            {
                // 清空图片预览 blob
                await JS.DestroyBlobAsync(image.PreviewUrl);

                // 释放图片数据流
                // Success 的图片数据流在上传完成后就被释放，此处仅需要对 Unstart 和 Failed 图片进行释放
                if((image.State is not ImageUploadState.Success) && (image.Stream is not null))
                {
                    await image.Stream.DisposeAsync();
                }
            });

            // 更新图片列表
            Images = Images.FindAll(image => image.State is ImageUploadState.Uploading);
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
