using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using North.Common;
using North.Core.Common;
using North.Core.Entities;
using North.Core.Models;
using North.Core.Repository;
using SqlSugar;
using System.Security.Claims;
using ILogger = North.Core.Services.Logger.ILogger;

namespace North.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImageController : ControllerBase
    {
        private readonly ILogger _logger;
        private readonly AppSetting _appSetting;
        private readonly PluginsContext _context;
        private readonly ISqlSugarClient _client;
        private readonly IHttpContextAccessor _accessor;

        public ImageController(ILogger logger, AppSetting appSetting, PluginsContext context, ISqlSugarClient client, IHttpContextAccessor accessor)
        {
            _logger = logger;
            _context = context;
            _client = client;
            _accessor = accessor;
            _appSetting = appSetting;
        }


        [HttpPost]
        [AllowAnonymous]
        public async ValueTask<object> UploadAsync([FromForm] IFormCollection formCollection)
        {
            try
            {
                var images = ((FormFileCollection)formCollection.Files).ConvertAll(file => new ImageUploadModel(file.OpenReadStream())
                {
                    Name = file.Name,
                    ContentType = file.ContentType,
                    PreviewUrl = string.Empty,
                    ThumbnailUrl = string.Empty,
                    SourceUrl = string.Empty
                });

                var currentOperateUserIdentify = User.Identities.FirstOrDefault();
                if (currentOperateUserIdentify?.IsAuthenticated is true)
                {
                    var userRepository = new UserRepository(_client, _appSetting.General.DataBase.EnabledName);
                    var currentOperateUserId = currentOperateUserIdentify.FindFirst(ClaimTypes.SerialNumber)?.Value;
                    var currentOperateUserLastModifyTime = currentOperateUserIdentify.FindFirst("LastModifyTime")?.Value;
                    var currentOperateUser = await userRepository.SingleAsync(u => u.Id.ToString() == currentOperateUserId);
                    if (currentOperateUser is null)
                    {
                        // 用户不存在
                        return new ApiResult<object>(ApiStatusCode.AccountNotExist);
                    }
                    else if (!currentOperateUser.IsApiAvailable)
                    {
                        // 无权访问 API
                        return new ApiResult<object>(ApiStatusCode.OperationDenied);
                    }
                    else if ((currentOperateUser.State is not UserState.Normal) || (currentOperateUser.LastModifyTime.ToString("G") != currentOperateUserLastModifyTime))
                    {
                        // 用户状态异常或改变
                        return new ApiResult<object>(ApiStatusCode.AccountStateAbnormal, "User status has changed");
                    }
                    else
                    {
                        return _context.OnUpload(images, currentOperateUser.DTO);
                    }
                }
                return _context.OnUpload(images, null);
            }
            catch(Exception e)
            {
                _logger.Error("Failed to upload images", e);
                return new ApiResult<object>(ApiStatusCode.ServerInternalError);
            }
        }


        [HttpGet]
        [AllowAnonymous]
        public async ValueTask<object> DownloadAsync()
        {
            try
            {
                var currentOperateUserIdentify = User.Identities.FirstOrDefault();
                if (currentOperateUserIdentify?.IsAuthenticated is true)
                {
                    var userRepository = new UserRepository(_client, _appSetting.General.DataBase.EnabledName);
                    var currentOperateUserId = currentOperateUserIdentify.FindFirst(ClaimTypes.SerialNumber)?.Value;
                    var currentOperateUserLastModifyTime = currentOperateUserIdentify.FindFirst("LastModifyTime")?.Value;
                    var currentOperateUser = await userRepository.SingleAsync(u => u.Id.ToString() == currentOperateUserId);
                    if (currentOperateUser is null)
                    {
                        // 用户不存在
                        return new ApiResult<object>(ApiStatusCode.AccountNotExist);
                    }
                    else if (!currentOperateUser.IsApiAvailable)
                    {
                        // 无权访问 API
                        return new ApiResult<object>(ApiStatusCode.OperationDenied);
                    }
                    else if ((currentOperateUser.State is not UserState.Normal) || (currentOperateUser.LastModifyTime.ToString("G") != currentOperateUserLastModifyTime))
                    {
                        // 用户状态异常或改变
                        return new ApiResult<object>(ApiStatusCode.AccountStateAbnormal, "User status has changed");
                    }
                    else
                    {
                        return _context.OnDownload(Request, currentOperateUser.DTO);
                    }
                }
                return _context.OnDownload(Request, null);
            }
            catch(Exception e)
            {
                _logger.Error("Failed to download the image", e);
                return new ApiResult<object>(ApiStatusCode.ServerInternalError);
            }
        }


        public async ValueTask<object> DeleteAsync()
        {
            return await ValueTask.FromResult(Ok());
        }
    }
}