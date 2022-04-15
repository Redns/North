using AntDesign;
using ImageBed.Common;
using ImageBed.Data.Entity;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace ImageBed.Pages
{
    partial class Index : IDisposable
    {
        bool spining;                               // 页面加载标志

        int  imageTotalNum;                         // 总图片数量
        int  imageSuccessNum;                       // 上传成功的图片数量
        long imageTotalSize;                        // 待上传的图片总尺寸(Byte)
        long _imageUploadedSize;                    // 上传完成的图片尺寸(Byte)
        long ImageUploadedSize
        {
            get { return _imageUploadedSize; }
            set
            {
                _imageUploadedSize = value;
                if(imageTotalSize != 0)
                {
                    ProgressPercent = (int)(_imageUploadedSize * 100.0 / imageTotalSize);
                }
            }
        }

        int  progress_stroke_width;                 // 进度条宽度
        bool progress_showInfo;                     // 是否显示进度条信息

        int _ProgressPercent;                       // 进度条百分比            
        int ProgressPercent
        {
            get { return _ProgressPercent; }
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
                _ProgressPercent = value;
            }
        }

        int imageUploadSizeLimit;                   // 图片最大上传尺寸(MB)
        int imageUploadNumLimit;                    // 图片单次最大上传数量(张)

        Data.Entity.Image? imageConfig;             // 图片设置


        /// <summary>
        /// 初始化加载界面
        /// </summary>
        protected override async Task OnInitializedAsync()
        {
            spining = true;

            imageTotalNum = 0;
            imageSuccessNum = 0;
            imageTotalSize = 0;
            ImageUploadedSize = 0;

            progress_stroke_width = 0;
            progress_showInfo = false;

            ProgressPercent = 0;

            imageConfig = GlobalValues.appSetting.Data.Resources.Images;
            imageUploadSizeLimit = imageConfig.MaxSize;
            imageUploadNumLimit = imageConfig.MaxNum;

            _ = JS.InvokeVoidAsync("BindPasteEvent", imageUploadSizeLimit, imageUploadNumLimit);

            await base.OnInitializedAsync();
        }


        /// <summary>
        /// 渲染完成后调用
        /// </summary>
        /// <param name="firstRender">是否为第一次渲染</param>
        /// <returns></returns>
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                spining = false;
                await InvokeAsync(() => { StateHasChanged(); });
            }
            await base.OnAfterRenderAsync(firstRender);
        }


        /// <summary>
        /// 图片上传前调用(检查图片尺寸、数量是否符合规则)
        /// </summary>
        /// <param name="images">待上传的图片列表</param>
        bool CheckImages(List<UploadFileItem> images)
        {
            // 初始化上传变量
            imageTotalNum = images.Count;
            imageSuccessNum = 0;
            imageTotalSize = 0;
            ImageUploadedSize = 0;
            
            // 移除尺寸超出限制的图片
            if (imageUploadSizeLimit != 0)
            {
                images.RemoveAll(image => image.Size > imageUploadSizeLimit * UnitNameGenerator.FILESIZE_1MB);
            }

            // 移除多余的图片
            if ((imageUploadNumLimit != 0) && (images.Count > imageUploadNumLimit))
            {
                int indexStart = imageUploadNumLimit;
                int redunCount = images.Count - indexStart;
                images.RemoveRange(indexStart, redunCount);
            }

            if(images.Count > 0)
            {
                images.ForEach(image =>
                {
                    imageTotalSize += image.Size;
                });
                return true;
            }
            else
            {
                _message.Error("上传失败, 图片尺寸或数量超出限制 !");
                return false;
            }
        }


        /// <summary>
        /// 图片状态改变时调用(调整进度条)
        /// </summary>
        /// <param name="fileinfo">所有图片的上传信息</param>
        void ImageStateChanged(UploadInfo uploadInfo)
        {
            var image = uploadInfo.File;
            if(image.State != UploadState.Uploading)
            {
                ImageUploadedSize += image.Size;
            } 
        }


        /// <summary>
        /// 上传完成后调用(统计成功上传的图片数、复制链接)
        /// </summary>
        async void UploadFinished(UploadInfo uploadInfo)
        {
            // 隐藏进度条
            ProgressPercent = 0;

            // 获取所有成功上传的图片的链接
            var urls = string.Empty;
            uploadInfo.FileList.ForEach((image) =>
            {
                var url = image.GetResponse<ApiResult<List<string>>>()?.Res?.FirstOrDefault();
                if (!string.IsNullOrEmpty(url))
                {
                    urls += $"{UnitNameGenerator.UrlBuild(imageConfig.UrlFormat, $"{NavigationManager.BaseUri}{image.Url}")}\n";
                    imageSuccessNum++;
                }
            });
            urls = urls.Remove(urls.Length - 1);

            // 清空已上传的图片列表
            uploadInfo.FileList.Clear();

            // 复制链接到剪贴板
            await JS.InvokeVoidAsync("CopyToClip", urls);
            _ = _message.Success($"图片上传完成, {imageSuccessNum}个成功, {imageTotalNum - imageSuccessNum}个失败 !"); ;
        }


        /// <summary>
        /// 释放资源
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        public void Dispose()
        {
            imageConfig = null;

            GC.Collect();   
            GC.SuppressFinalize(this);
        }
    }
}
