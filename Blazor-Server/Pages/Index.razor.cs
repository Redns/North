using AntDesign;
using ImageBed.Common;
using ImageBed.Data.Access;
using ImageBed.Data.Entity;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace ImageBed.Pages
{
    partial class Index
    {
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);
            if (firstRender)
            {
                // TODO 恢复CV上传功能
                // 监听 Ctrl + V 快捷键，实现快捷键上传功能
                // _ = JS.InvokeVoidAsync("BindPasteEvent", User.SingleUploadMaxSize, User.SingleUploadMaxNum);
            }
        }


        /// <summary>
        /// 上传图片
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        private async Task Upload(InputFileChangeEventArgs args)
        {
            var imageTotalNum = 0;
            var imageSuccessNum = 0;
            var imageTotalSize = 0L;
            var imageUploadedSize = 0L;

            var imageUrls = string.Empty;
            var imageEntities = new List<ImageEntity>();

            using (var context = new OurDbContext())
            {
                var sqlUserData = new SqlUserData(context);
                var sqlImageData = new SqlImageData(context);
                var sqlRecordData = new SqlRecordData(context);

                var token = await _storage.GetItemAsync<string>(GlobalValues.LOCALSTORE_KEY_TOKEN);
                var username = await _storage.GetItemAsync<string>(GlobalValues.LOCALSTORE_KEY_USERNAME);
                if (!string.IsNullOrEmpty(token) && !string.IsNullOrEmpty(username))
                {
                    var user = await sqlUserData.GetFirstAsync(u => (u.UserName == username) && (u.Token == token) && u.IsTokenValid());
                    var record = await sqlRecordData.GetFirstAsync(r => r.Date == DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));

                    if (user != null)
                    {
                        // 移除尺寸超出限制的图片
                        var images = args.GetMultipleFiles(9999)
                                         .Where(i => (user.SingleUploadMaxSize <= 0) || (i.Size <= user.SingleUploadMaxSize * FileHelper.FILESIZE_1MB));

                        // 检查上传数量是否超出单次限制
                        if ((user.SingleUploadMaxNum > 0) && (images.Count() > user.SingleUploadMaxNum))
                        {
                            images = images.Take(user.SingleUploadMaxNum);
                        }

                        // 检查用户是否有剩余空间
                        var remainNum = user.TotalUploadMaxNum - user.TotalUploadNum;
                        var remainSize = user.TotalUploadMaxSize - user.TotalUploadSize;

                        imageTotalNum = images.Count();
                        images.ForEach(i => { imageTotalSize += i.Size; });
                        if (((user.TotalUploadMaxNum > 0) && (images.Count() > remainNum)) || (user.TotalUploadMaxSize > 0) && (imageTotalSize > remainSize))
                        {
                            _ = _message.Error("存储空间不足! ", 1.5);
                            return;
                        }

                        // 上传图片
                        if (images.Any())
                        {
                            var uploadModelProp = new ConfirmOptions()
                            {
                                Title = "0.00 %",
                                Content = $"正在上传 {images.First().Name}",
                            };
                            var uploadModelRef = _modalService.Info(uploadModelProp);

                            images.ForEach(async i =>
                            {
                                try
                                {
                                    using (var imageReadStream = i.OpenReadStream((long)(user.SingleUploadMaxSize * FileHelper.FILESIZE_1MB)))
                                    {
                                        var image = await FileHelper.SaveImage(imageReadStream, i.Name, GlobalValues.AppSetting.Data.Image.RootPath, user.UserName);

                                        imageEntities.Add(image);
                                        imageUrls += $"{image.Url}\n";

                                        imageSuccessNum++;
                                        imageUploadedSize += i.Size;

                                        uploadModelProp.Title = $"{imageUploadedSize / imageTotalSize:f2} %";
                                        uploadModelProp.Content = $"正在上传 {i.Name}";
                                        await uploadModelRef.UpdateConfigAsync(uploadModelProp);
                                    }
                                }
                                catch (Exception ex)
                                {
                                    GlobalValues.Logger.Error($"Upload {i.Name} failed, {ex.Message}");
                                }
                            });

                            // 更新Images数据库表
                            await sqlImageData.AddRangeAsync(imageEntities);

                            // 更新Users数据库表
                            user.TotalUploadNum += imageSuccessNum;
                            user.TotalUploadSize += imageUploadedSize / FileHelper.FILESIZE_1MB;
                            sqlUserData.Update(user);

                            // 更新Record数据库表
                            if (record != null)
                            {
                                record.UploadImageSize += (int)(imageUploadedSize / FileHelper.FILESIZE_1MB);
                                record.UploadImageNum += imageSuccessNum;
                                _ = sqlRecordData.UpdateAsync(record);
                            }
                            else
                            {
                                _ = sqlRecordData.AddAsync(new RecordEntity(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), imageSuccessNum, (int)(imageUploadedSize / FileHelper.FILESIZE_1MB), 0));
                            }

                            await _modalService.DestroyConfirmAsync(uploadModelRef);
                        }
                        _ = _message.Success($"上传完成, {imageSuccessNum} 张成功, {imageTotalNum} 张失败! ", 1.5);
                    }
                    else
                    {
                        NavigationManager.NavigateTo(GlobalValues.ROUTER_LOGIN, true);
                    }
                }
                else
                {
                    NavigationManager.NavigateTo(GlobalValues.ROUTER_LOGIN, true);
                }
            }
        }
    }
}
