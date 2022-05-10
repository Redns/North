using ImageBed.Common;
using ImageBed.Data.Access;
using ImageBed.Data.Entity;
using Microsoft.AspNetCore.Mvc;

namespace ImageBed.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImageController : ControllerBase
    {
        /// <summary>
        /// 上传图片
        /// </summary>
        /// <returns></returns>
        [HttpPost("upload")]
        public async Task<ApiResult<object>> Post([FromQuery] string UserName, [FromQuery] string Token, [FromForm] IFormCollection formCollection)
        {
            var imageConfig = GlobalValues.AppSetting.Data.Image;   // 图片设置
            var imageUrls = new List<string>();                     // 图片url(相对路径)
            var images = new List<ImageEntity>();                   // 图片信息

            try
            {
                using (var context = new OurDbContext())
                {
                    var sqlUserData = new SqlUserData(context);
                    var sqlImageData = new SqlImageData(context);
                    var sqlRecordData = new SqlRecordData(context);

                    var user = await sqlUserData.GetFirstAsync(u => (u.UserName == UserName) && (u.Token == Token) && u.IsTokenValid());
                    var record = await sqlRecordData.GetFirstAsync(r => r.Date == DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));

                    var uploadImages = (FormFileCollection)formCollection.Files;
                    var remainderNum = user.TotalUploadMaxNum - user.TotalUploadNum;
                    var remainderSize = (user.TotalUploadMaxSize - user.TotalUploadSize) * FileHelper.FILESIZE_1MB;

                    if (user != null)
                    {
                        // 检查图片尺寸
                        uploadImages.RemoveAll(image => (user.SingleUploadMaxSize > 0) && (image.Length > user.SingleUploadMaxSize * FileHelper.FILESIZE_1MB));

                        // 检查单次上传数量
                        if ((user.SingleUploadMaxNum > 0) && (uploadImages.Count > user.SingleUploadMaxNum))
                        {
                            uploadImages.RemoveRange(user.SingleUploadMaxNum, uploadImages.Count - user.SingleUploadMaxNum);
                        }

                        // 检查用户是否有剩余空间
                        var uploadTotalSize = 0.0;
                        uploadImages?.ForEach(image => uploadTotalSize += image.Length);
                        if (((user.TotalUploadMaxSize > 0) && (uploadTotalSize > remainderSize)) || ((user.TotalUploadMaxNum > 0) && (uploadImages.Count > remainderNum)))
                        {
                            throw new ApiException(ApiResultCode.SpaceNotEnough);
                        }

                        // 上传图片
                        foreach (IFormFile fileReader in uploadImages)
                        {
                            using (var imageReadStream = fileReader.OpenReadStream())
                            {
                                var image = await FileHelper.SaveImage(imageReadStream, fileReader.FileName, GlobalValues.AppSetting.Data.Image.RootPath, UserName);
                                images.Add(image);
                                imageUrls.Add($"{image.Url}");
                            }
                        }
                        await sqlImageData.AddRangeAsync(images);

                        // 更新Record数据库表
                        if (record != null)
                        {
                            record.UploadImageSize += (int)(uploadTotalSize / FileHelper.FILESIZE_1MB);
                            record.UploadImageNum += uploadImages.Count;
                            _ = sqlRecordData.UpdateAsync(record);
                        }
                        else
                        {
                            _ = sqlRecordData.AddAsync(new RecordEntity(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), uploadImages.Count, (int)uploadTotalSize, 0));
                        }
                        return new ApiResult<object>(ApiResultCode.Success, "Upload succes", imageUrls);
                    }
                    throw new ApiException(ApiResultCode.TokenInvalid);
                }
            }
            catch (ApiException apiException)
            {
                var message = apiException.ApiResultCode switch
                {
                    ApiResultCode.TokenInvalid => "User not exist or token invalid",
                    ApiResultCode.SpaceNotEnough => "Insufficient upload space, please delete some images and try again",
                    _ => ""
                };
                return new ApiResult<object>(apiException.ApiResultCode, message, apiException.Param);
            }
            catch(Exception ex)
            {
                GlobalValues.Logger.Error($"Upload failed, {ex.Message}");
                return new ApiResult<object>(ApiResultCode.InternalError, "Server internal error", null);
            }
        }


        /// <summary>
        /// 下载图片
        /// </summary>
        /// <param name="imageName">图片名称</param>
        /// <returns></returns>
        [HttpGet("{imageName}")]
        public async Task<IActionResult> Get(string imageName)
        {
            try
            {
                string imageFullPath = $"{GlobalValues.AppSetting.Data.Image.RootPath}/{imageName}";
                string imageExtension = FileHelper.GetFileExtension(imageName);
                if (System.IO.File.Exists(imageFullPath))
                {
                    using (var context = new OurDbContext())
                    {
                        var sqlImageData = new SqlImageData(context);
                        var sqlRecordData = new SqlRecordData(context);
                        
                        var image = await sqlImageData.GetFirstAsync(i => i.Name == imageName);
                        var record = await sqlRecordData.GetFirstAsync(r => r.Date == DateTime.Now.ToString("yyyy/MM/dd"));

                        if (image != null)
                        {
                            image.RequestNum++;
                            await sqlImageData.UpdateAsync(image);
                        }

                        if (record != null)
                        {
                            record.RequestNum++;
                            _ = sqlRecordData.UpdateAsync(record);
                        }
                        else
                        {
                            _ = sqlRecordData.AddAsync(new RecordEntity(DateTime.Now.ToString("yyyy/MM/dd"), 0, 0, 1));
                        }
                    }
                    return File(System.IO.File.ReadAllBytes(imageFullPath), $"image/{imageExtension}");
                }
            }
            catch (Exception ex)
            {
                GlobalValues.Logger.Error($"Get {imageName} failed, {ex.Message}");
            }
            return File(System.IO.File.ReadAllBytes($"{GlobalValues.AppSetting.Data.Image.RootPath}/imageNotFound.jpg"), $"image/jpg");
        }


        /// <summary>
        /// 删除指定图片
        /// </summary>
        /// <param name="UserName">用户账号</param>
        /// <param name="Token">用户令牌</param>
        /// <param name="imageName">图片名称</param>
        /// <returns></returns>
        [HttpDelete("{imageName}")]
        public async Task<ApiResult<object>> Delete([FromQuery] string UserName, [FromQuery] string Token, string imageName)
        {
            try
            {
                using(var context = new OurDbContext())
                {
                    var sqlUserData = new SqlUserData(context);
                    var sqlImageData = new SqlImageData(context);

                    var user = await sqlUserData.GetFirstAsync(u => (u.UserName == UserName) && (u.Token == Token) && u.IsTokenValid());
                    var image = await sqlImageData.GetFirstAsync(i => i.Name == imageName);
                    var imageOwner = await sqlUserData.GetFirstAsync(u => u.UserName == image.Owner);

                    // 验证用户
                    if (user != null)
                    {
                        if((imageOwner == null) || (imageOwner.UserName == UserName) || (user.UserType > imageOwner.UserType))
                        {
                            if (image != null)
                            {
                                System.IO.File.Delete($"{GlobalValues.AppSetting.Data.Image.RootPath}/{imageName}");
                                System.IO.File.Delete($"{GlobalValues.AppSetting.Data.Image.RootPath}/thumbnails_{imageName}");
                                
                                if(imageOwner != null)
                                {
                                    imageOwner.TotalUploadNum--;
                                    imageOwner.TotalUploadSize -= FileHelper.ParseFileSize(image.Size);

                                    // 更新数据库表 Users
                                    if (!sqlUserData.Update(imageOwner))
                                    {
                                        throw new ApiException(ApiResultCode.InternalError);
                                    }
                                }

                                // 更新数据库表Images
                                if (!await sqlImageData.RemoveAsync(image))
                                {
                                    throw new ApiException(ApiResultCode.InternalError);
                                }
                            }
                            return new ApiResult<object>(ApiResultCode.Success, $"Remove {imageName} success", null);
                        }
                        throw new ApiException(ApiResultCode.AccessDenied);
                    }
                    throw new ApiException(ApiResultCode.TokenInvalid);
                }
            }
            catch(ApiException apiException)
            {
                var message = apiException.ApiResultCode switch
                {
                    ApiResultCode.TokenInvalid => "Token invalid",
                    ApiResultCode.AccessDenied => "Access denied",
                    ApiResultCode.InternalError => $"Remove {imageName} from database failed",
                    _ => ""
                };
                return new ApiResult<object>(apiException.ApiResultCode, message, apiException.Param);
            }
            catch (Exception ex)
            {
                GlobalValues.Logger.Error($"Remove {imageName} failed, {ex.Message}");
                return new ApiResult<object>(ApiResultCode.InternalError, $"Remove {imageName} failed", null);
            }
        }
    }
}