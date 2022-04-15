using AntDesign;
using ImageBed.Common;
using ImageBed.Data.Access;
using ImageBed.Data.Entity;
using Microsoft.JSInterop;

namespace ImageBed.Pages
{
    partial class Pics
    {
        // 视图切换按钮图标(列表/卡片)
        string imageFormatChangeButtonIcon = GlobalValues.appSetting.Pics.ViewList ? IconType.Outline.UnorderedList : IconType.Outline.Appstore;
        void ChangeTableFormat()
        {
            if(imageFormatChangeButtonIcon == IconType.Outline.UnorderedList)
            {
                imageFormatChangeButtonIcon = IconType.Outline.Appstore;
            }
            else
            {

                imageFormatChangeButtonIcon = IconType.Outline.UnorderedList;
            }
        }


        // 图片卡片
        ListGridType grid = new()
        {
            Gutter = 16,
            Xs = 1,
            Sm = 2,
            Md = 4,
            Lg = 4,
            Xl = 6,
            Xxl = 6,
        };


        /// <summary>
        /// 搜索框部分
        /// </summary>
        string? searchText;
        bool searchRunning = false;


        /// <summary>
        /// 根据搜索框内容检索图片
        /// </summary>
        public void OnSearch()
        {
            searchRunning = true;

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
                using(var context = new OurDbContext())
                {
                    imagesShow = (ImageEntity[])imagesAll.Clone();
                }
            }
            
            searchRunning = false;
        }


        /// <summary>
        /// 表格部分
        /// </summary>
        string imageFormatFilter = "";
        Dictionary<string, object> attrs = new();


        /// <summary>
        /// 图片导入前调用
        /// </summary>
        /// <param name="imageInfo">待导入的图片信息</param>
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
                imagesShow = (ImageEntity[])imagesAll.Clone();

                _ = _notice.Open(new NotificationConfig()
                {
                    Message = "图片导入完成",
                    Description = "图片已成功导入至服务器！"
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
                var sqlImageData = new SQLImageData(context);
                var imagesSelectedList = imagesSelected?.ToList();
                if((imagesSelectedList != null) && (imagesSelectedList.Count > 0))
                {
                    await sqlImageData.RemoveRangeAsync(imagesSelectedList);
                    foreach (var image in imagesSelected)
                    {
                        imagesAll = imagesAll.Remove(image);
                        imagesShow = imagesShow.Remove(image);
                    }
                }
                imagesSelected = null;
                _ = _message.Success("图片已删除!");
            }
        }


        /// <summary>
        /// 导出选中的图片
        /// </summary>
        /// <returns></returns>
        async Task ExportSelectedImages()
        {
            List<string> imageFullpaths = new();
            string imageDir = $"{GlobalValues.appSetting?.Data?.Resources?.Images?.Path}";
            if (GlobalValues.appSetting != null)
            {
                foreach (var image in imagesSelected)
                {
                    imageFullpaths.Add($"{imageDir}/{image.Name}");
                }
            }

            // 打包压缩文件
            // 压缩文件路径为 {ImagesDir}/Imaged.zip
            string zipFullPath = $"{imageDir}/Images.zip";
            if (File.Exists(zipFullPath))
            {
                File.Delete(zipFullPath);
            }
            await FileOperator.CompressMulti(imageFullpaths, $"{GlobalValues.appSetting?.Data?.Resources?.Images?.Path}/Images.zip");
            
            // 这里压缩完成后可以先弹窗提示, 然后子线程再去启动下载
            // 调用 JS 下载后不建议删除 Images.zip，因为这时并不一定下载完成
            _ = _message.Success("导出成功!");
            await JS.InvokeVoidAsync("downloadFileFromStream", "Images.zip", "api/image/Images.zip");
        }


        ITable? table;
        int _pageSize = 8;
        bool loading = true;

        ImageEntity[] imagesAll = Array.Empty<ImageEntity>();
        ImageEntity[] imagesShow = Array.Empty<ImageEntity>();

        IEnumerable<ImageEntity>? imagesSelected;



        /// <summary>
        /// 初始化界面
        /// </summary>
        /// <returns></returns>
        protected override async Task OnInitializedAsync()
        {
            loading = true;

            // 加载数据库信息
            using (var context = new OurDbContext())
            {
                imagesAll = await new SQLImageData(context).GetArrayAsync();
                imagesShow = (ImageEntity[])imagesAll.Clone();
            }

            // 设置文件选择过滤器
            string[] imageFormats = GlobalValues.appSetting.Data.Resources.Images.Format.Split(",");
            for (int i = 0; i < imageFormats.Length; i++)
            {
                imageFormatFilter += $".{imageFormats[i]},";
            }
            imageFormatFilter += ".zip";

            attrs = new Dictionary<string, object>{
                { "accept", imageFormatFilter},
                { "Action", "/api/image" },
                { "Name", "files" },
                { "Multiple", true }
            };
            loading = false;
        }


        /// <summary>
        /// 复制图片链接到剪贴板
        /// </summary>
        /// <param name="content">图片链接</param>
        async void CopyUrlToClip(string? content)
        {
            if (!string.IsNullOrEmpty(content))
            {
                await JS.InvokeVoidAsync("CopyToClip", content);
                _ = _message.Success("已拷贝链接到剪贴板！");
            }
            else
            {
                _ = _message.Error("拷贝链接失败！");
            }
        }


        /// <summary>
        /// 调用 JS 下载文件
        /// </summary>
        /// <param name="url">文件url</param>
        /// <param name="imagename">下载时的文件名称</param>
        async void DownloadFile(string url, string? imagename)
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
        public async void RemoveImage(ImageEntity image)
        {
            GlobalValues.Logger.Info($"Removing image {image.Name}...");
            
            using(var context = new OurDbContext())
            {
                await new SQLImageData(context).RemoveAsync(image);
            }
            imagesAll = imagesAll.Remove(image);
            imagesShow = imagesShow.Remove(image);

            _ = _message.Success("图片已删除!");

            GlobalValues.Logger.Info($"Remove image {image.Name} done");
        }
    }
}
