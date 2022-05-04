using AntDesign;
using ImageBed.Common;
using ImageBed.Data.Access;
using ImageBed.Data.Entity;
using Microsoft.JSInterop;

namespace ImageBed.Pages
{
    public enum ViewFormat
    {
        List = 0,       // 列表视图
        Card            // 卡片视图
    }

    partial class Pics
    {
        bool spinning;
        bool searching;
        string? searchText;

        ITable? table;
        int pageSize;

        ImageEntity[] imagesAll = Array.Empty<ImageEntity>();
        ImageEntity[] imagesShow = Array.Empty<ImageEntity>();
        IEnumerable<ImageEntity>? imagesSelected;

        Data.Entity.Image? imageConfig;         // 图片相关设置
        Data.Entity.Pics?  picsConfig;          // 图库界面相关设置

        string imageFormatChangeButtonIcon;     // 视图切换按钮图标  

        ViewFormat _imageViewFormat;            // 图库界面视图
        ViewFormat ImageViewFormat
        {
            get { return _imageViewFormat; }
            set
            {
                _imageViewFormat = value;
                if(value == ViewFormat.List)
                {
                    imageFormatChangeButtonIcon = IconType.Outline.UnorderedList;
                }
                else
                {
                    imageFormatChangeButtonIcon = IconType.Outline.Appstore;
                }
            }
        }

        // 卡片视图时相关参数
        ListGridType grid = new()               
        {
            Gutter = 16,    // 栅格间距
            Xs = 1,         // < 576px 展示的列数
            Sm = 2,         // ≥ 576px 展示的列数
            Md = 2,         // ≥ 768px 展示的列数
            Lg = 3,         // ≥ 992px 展示的列数
            Xl = 4,         // ≥ 1200px 展示的列数
            Xxl = 5,        // ≥ 1600px 展示的列数 
        };


        /// <summary>
        /// 初始化界面
        /// </summary>
        /// <returns></returns>
        protected override async Task OnInitializedAsync()
        {
            spinning = true;

            searching = false;
            searchText = null;

            pageSize = 8;

            imageConfig = GlobalValues.appSetting.Data.Resources.Images;
            picsConfig  = GlobalValues.appSetting.Pics;

            ImageViewFormat = picsConfig.ViewFormat;

            using (var context = new OurDbContext())
            {
                imagesAll = await new SQLImageData(context).GetArrayAsync();
                imagesShow = (ImageEntity[])imagesAll.Clone();
            }
        }


        /// <summary>
        /// 页面渲染完成后调用
        /// </summary>
        /// <param name="firstRender"></param>
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                spinning = false;
                await InvokeAsync(() => { StateHasChanged(); });
            }
            await base.OnAfterRenderAsync(firstRender);
        }


        /// <summary>
        /// 切换视图
        /// </summary>
        void ChangeTableFormat()
        {
            if(ImageViewFormat == ViewFormat.List)
            {
                ImageViewFormat = ViewFormat.Card;
            }
            else
            {
                ImageViewFormat = ViewFormat.List;
            }
        }


        /// <summary>
        /// 检索图片
        /// </summary>
        public void OnSearch()
        {
            searching = true;

            if(searchText != null)
            {
                imagesShow = imagesAll.Where(i => i.Name.Contains(searchText) ||
                                                  i.Dpi.Contains(searchText) ||
                                                  i.Size.Contains(searchText) ||
                                                  i.UploadTime.Contains(searchText) ||
                                                  i.Owner.Contains(searchText)).ToArray();
            }
            else
            {
                imagesShow = (ImageEntity[])imagesAll.Clone();
            }
            
            searching = false;
        }


        /// <summary>
        /// 图片导入前调用
        /// </summary>
        /// <param name="images">待导入的图片</param>
        async Task<bool> StartImport(List<UploadFileItem> images)
        {
            await _notice.Open(new NotificationConfig()
            {
                Message = "图片开始导入",
                Description = "图片后台导入中，导入完成前请勿再次导入！"
            });
            return true;
        }


        /// <summary>
        /// 图片导入完成后调用
        /// </summary>
        async Task ImportFinished()
        {
            using (var context = new OurDbContext())
            {
                imagesAll = await new SQLImageData(context).GetArrayAsync();
                
                OnSearch();

                _ = _notice.Open(new NotificationConfig()
                {
                    Message = "图片导入完成",
                    Description = $"图片已成功导入至服务器, 现有 {imagesAll.Length} 张图片！"
                });
            }
        }


        /// <summary>
        /// 移除选中的图片
        /// </summary>
        /// <returns></returns>
        async Task RemoveSelectedImages()
        {
            using (var context = new OurDbContext())
            {
                if((imagesSelected != null) && (imagesSelected.Any()))
                {
                    await new SQLImageData(context).RemoveRangeAsync(imagesSelected);
                    foreach (var image in imagesSelected)
                    {
                        imagesAll = imagesAll.Remove(image);
                        imagesShow = imagesShow.Remove(image);
                    }
                }
                imagesSelected = null;

                _ = _message.Success("图片已删除! ");
            }
        }


        /// <summary>
        /// 导出选中的图片
        /// </summary>
        /// <returns></returns>
        async Task ExportSelectedImages()
        {
            List<string> imageFullpaths = new();

            if((imagesSelected != null) && imagesSelected.Any())
            {
                imagesSelected.ForEach(image =>
                {
                    imageFullpaths.Add($"{imageConfig.Path}/{image.Name}");
                });
            }

            // 打包压缩文件
            // 压缩文件路径为 {ImagesDir}/Imaged.zip
            string zipFullPath = $"{imageConfig.Path}/Images.zip";
            if (File.Exists(zipFullPath)){ File.Delete(zipFullPath); }
            await FileOperator.CompressMulti(imageFullpaths, $"{imageConfig.Path}/Images.zip");
            
            // 这里压缩完成后可以先弹窗提示, 然后子线程再去启动下载
            // 调用 JS 下载后不建议删除 Images.zip
            _ = _message.Success("导出成功 !");
            _ = JS.InvokeVoidAsync("downloadFileFromStream", "Images.zip", "api/image/Images.zip");
        }


        /// <summary>
        /// 复制图片链接到剪贴板
        /// </summary>
        /// <param name="content">图片链接</param>
        async Task CopyUrlToClip(string? content)
        {
            if (!string.IsNullOrEmpty(content))
            {
                await JS.InvokeVoidAsync("CopyToClip", content);
                _ = _message.Success("已拷贝链接到剪贴板 !");
            }
            else
            {
                _ = _message.Error("拷贝链接失败, 链接为空 !");
            }
        }


        /// <summary>
        /// 调用 JS 下载文件
        /// </summary>
        /// <param name="url">文件url</param>
        /// <param name="imagename">下载时的文件名称</param>
        async Task DownloadFile(string url, string? imagename)
        {
            if (string.IsNullOrEmpty(imagename))
            {
                await JS.InvokeVoidAsync("downloadFileFromStream", "default-imageName", url);
            }
            else
            {
                await JS.InvokeVoidAsync("downloadFileFromStream", imagename, url);
            }
        }


        /// <summary>
        /// 移除单个图片
        /// </summary>
        /// <param name="image">待移除的图片实体</param>
        async Task RemoveImage(ImageEntity image)
        {
            using(var context = new OurDbContext())
            {
                await new SQLImageData(context).RemoveAsync(image);
            }
            imagesAll = imagesAll.Remove(image);
            imagesShow = imagesShow.Remove(image);

            _ = _message.Success("图片已删除 !");
        }
    }
}
