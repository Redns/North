﻿using AntDesign;
using ImageBed.Common;
using ImageBed.Data.Access;
using ImageBed.Data.Entity;
using Microsoft.JSInterop;

namespace ImageBed.Pages
{
    partial class Pics
    {
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

            imagesShow.Clear();
            imagesShow.AddRange(imagesAll.Where(image => (searchText == null) ||
                                               (image.Name ?? "").Contains(searchText) ||
                                               (image.Owner ?? "").Contains(searchText)));
            searchRunning = false;
        }


        /// <summary>
        /// 表格部分
        /// </summary>
        List<UploadFileItem> fileList = new();
        readonly Dictionary<string, object> attrs = new()
        {
            {"accept", "image/*,.zip"},
            {"Action", "/api/image" },
            {"Name", "files" },
            {"Multiple", true }
        };

        bool uploadRunning = false;

        /// <summary>
        /// 图片导入前调用
        /// </summary>
        /// <param name="imageInfo">待导入的图片信息</param>
        void StartImport(UploadInfo imageInfo)
        {
            if (!uploadRunning)
            {
                _notice.Open(new NotificationConfig()
                {
                    Message = "图片开始导入",
                    Description = "图片后台导入中，导入完成前请勿再次导入！"
                });
                uploadRunning = true;
            }
        }


        /// <summary>
        /// 图片导入完成后调用
        /// </summary>
        async Task UploadFinished()
        {
            using (var context = new OurDbContext())
            {
                var sqlImageData = new SQLImageData(context);
                imagesAll = await sqlImageData.GetAsync();
                OnSearch();

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
                    _ = sqlImageData.RemoveRangeAsync(imagesSelectedList);
                    foreach (var image in imagesSelected)
                    {
                        imagesAll.Remove(image);
                        imagesShow.Remove(image);
                    }
                }
                await _message.Success("图片已删除!");
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
        int _total;
        int _pageSize = 8;

        List<ImageEntity> imagesShow = new();
        List<ImageEntity> imagesAll = new();
        IEnumerable<ImageEntity>? imagesSelected;


        /// <summary>
        /// 初始化界面
        /// </summary>
        /// <returns></returns>
        protected override async Task OnInitializedAsync()
        {
            using (var context = new OurDbContext())
            {
                imagesAll = await new SQLImageData(context).GetAsync();
                OnSearch();
            }
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
        async void RemoveImage(ImageEntity image)
        {
            using (var context = new OurDbContext())
            {
                _ = new SQLImageData(context).RemovaAsync(image);
                imagesAll.Remove(image);
                imagesShow.Remove(image);

                await _message.Success("图片已删除！");
            }
        }
    }
}