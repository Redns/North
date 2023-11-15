using Microsoft.AspNetCore.Http;
using North.Core.Entities;
using North.Core.Models;

namespace North.Plugin
{
    /// <summary>
    /// 图片鉴权接口
    /// </summary>
    public interface IAuth
    {
        /// <summary>
        /// 图片上传鉴权（检查上传数量、容量是否满足要求）
        /// </summary>
        /// <param name="images">待上传的图片</param>
        /// <param name="user">当前用户</param>
        /// <returns></returns>
        void Upload(in IEnumerable<ImageUploadModel> images, in UserDTOEntity user);


        /// <summary>
        /// 图片访问鉴权（检查是否有权限访问）
        /// </summary>
        /// <param name="request">下载请求</param>
        /// <param name="user">当前用户</param>
        /// <returns>允许访问返回 true，否则返回 false</returns>
        bool CanAccess(in HttpRequest request, in ImageEntity image, in UserDTOEntity? user);
    }
}
