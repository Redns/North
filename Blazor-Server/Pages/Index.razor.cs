using AntDesign;
using ImageBed.Common;
using ImageBed.Data.Entity;
using Microsoft.JSInterop;

namespace ImageBed.Pages
{
    partial class Index
    {
        Dictionary<string, object> attrs = new()
        {
            {"accept", "image/*"},
            {"Action", "/api/image" },
            {"Name", "files" },
            {"Multiple", true }
        };

        int imageTotalNum = 0;                          // 用户选择的总图片数量
        int imageSuccessNum = 0;                        // 上传成功的图片数量(获取到链接)
        long imageTotalSize = 0;                        // 待上传的图片总尺寸(Byte)
        long imageUploadedSize = 0;                     // 上传完成的图片尺寸(Byte)
        int _progress_percent = 0;                      // 上传完成的图片的百分比
        int progress_percent
        {
            get
            {
                return _progress_percent;
            }
            set
            {
                if (value == 0)
                {
                    progress_stroke_width = 0;
                    progress_showInfo = false;
                }
                else
                {
                    progress_stroke_width = 10;
                    progress_showInfo = true;
                }
                _progress_percent = value;
            }
        }
        int progress_stroke_width = 0;                  // 进度条宽度
        bool progress_showInfo = false;                 // 是否显示进度条信息


        /// <summary>
        /// 更新进度条
        /// </summary>
        void UpdateProgress()
        {
            progress_percent = (int)(imageUploadedSize * 100.0 / imageTotalSize);
        }


        /// <summary>
        /// 图片上传前调用
        /// </summary>
        /// <param name="images">待上传的图片列表</param>
        bool CheckImages(List<UploadFileItem> images)
        {
            // 图片最大上传尺寸(MB)
            // 图片单次最大上传数量(张)
            int imageUploadSizeLimit = GlobalValues.appSetting.Data.Resources.Images.MaxSize;
            int imageUploadNumLimit = GlobalValues.appSetting.Data.Resources.Images.MaxNum;

            imageSuccessNum = 0;
            imageTotalNum = images.Count;

            if (imageUploadSizeLimit > 0)
            {
                images.RemoveAll(i => i.Size > imageUploadSizeLimit * 1024 * 1024);
            }

            if ((images.Count > imageUploadNumLimit) && (imageUploadNumLimit > 0))
            {
                int indexStart = imageUploadNumLimit;
                int redunCount = images.Count - indexStart;
                images.RemoveRange(indexStart, redunCount);
            }

            if(images.Count > 0)
            {
                imageTotalSize = 0;
                imageUploadedSize = 0;
                foreach (var image in images)
                {
                    imageTotalSize += image.Size;
                }

                UpdateProgress();

                return true;
            }
            else
            {
                _message.Error("上传失败, 图片尺寸或数量超出限制!");
                return false;
            }
        }


        /// <summary>
        /// 图片状态改变时调用
        /// </summary>
        /// <param name="fileinfo">所有图片的上传信息</param>
        void ImageStateChanged(UploadInfo uploadInfo)
        {
            var image = uploadInfo.File;
            if(image.State != UploadState.Uploading)
            {
                if(image.State == UploadState.Success)
                {
                    image.Url = image.GetResponse<ApiResult<List<string>>>()?.Res?.FirstOrDefault();
                }
                imageUploadedSize += image.Size;
                UpdateProgress();
            } 
        }


        /// <summary>
        /// 上传完成后调用
        /// </summary>
        async void UploadFinished(UploadInfo uploadInfo)
        {
            progress_percent = 0;

            string urls = string.Empty;

            var images = uploadInfo.FileList;
            foreach(var image in images)
            {
                if (!string.IsNullOrEmpty(image.Url))
                {
                    urls += $"{NavigationManager.BaseUri}{image.Url}\n";
                    imageSuccessNum++;
                }
            }
            urls = urls.Remove(urls.Length - 1);

            images.Clear();

            await JS.InvokeVoidAsync("CopyToClip", urls);
            _ = _message.Success($"图片上传完成, {imageSuccessNum}个成功, {imageTotalNum - imageSuccessNum}个失败!"); ;
        }


        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
             await JS.InvokeVoidAsync("BindPasteEvent");
        }
    }
}
