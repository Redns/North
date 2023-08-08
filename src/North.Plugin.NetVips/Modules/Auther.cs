using Masuit.Tools;
using Microsoft.AspNetCore.Http;
using North.Core.Entities;
using North.Core.Models;

namespace North.Plugin.NetVips.Modules
{
    /// <summary>
    /// 图片鉴权模块
    /// </summary>
    public class Auther : IAuth
    {
        /// <summary>
        /// 图片访问权限控制
        /// </summary>
        /// <param name="request">图片访问请求</param>
        /// <param name="image">图片实体</param>
        /// <param name="user">访问用户实体</param>
        /// <returns></returns>
        public bool Download(in HttpRequest request, in ImageEntity image, in UserDTOEntity? user)
        {
            return image.AccessPermission is ImageAccessPermission.Public ||
                   (image.AccessPermission is ImageAccessPermission.LoggedInUser && user is not null) ||
                   (image.AccessPermission is ImageAccessPermission.Private && image.UserId == user?.Id); 
        }


        /// <summary>
        /// 上传图片
        /// </summary>
        /// <param name="images">待上传的图片</param>
        /// <param name="user">用户实体</param>
        /// <returns></returns>
        public void Upload(in IEnumerable<ImageUploadModel> images, in UserDTOEntity user)
        {
            // 获取插件设置
            var imageUploadPreference = UploadPreference.None;

            // 去除超出尺寸限制的图片
            var imageSingleMaxSize = user.SingleMaxUploadSizeByte;
            images.Where(i => i.Length > imageSingleMaxSize).ForEach(image =>
            {
                image.Progress = 0;
                image.Message = "图片大小超出限制";
                image.State = ImageUploadState.Failed;
            });

            // 计算剩余待上传图片相关参数
            var remainImages = images.Where(image => image.State == ImageUploadState.Uploading);
            var imageRemainNum = remainImages.Count();
            var imageRemainSize = remainImages.Sum(image => image.Length);
            var imageMaxAllowedNum = user.UplaodRemainedNums < user.SingleMaxUploadNums ? user.UplaodRemainedNums : user.SingleMaxUploadNums;
            var imageMaxAllowedSize = user.TotalUploadedCapacityByte;

            // 计算图片上传溢出数量和容量
            var imageOverNum = imageRemainNum - imageMaxAllowedNum;
            var imageOverSize = imageRemainSize - imageMaxAllowedSize;

            // 根据上传偏好筛选图片
            if (imageOverNum > 0 && imageOverSize > 0)
            {
                switch (imageUploadPreference)
                {
                    case UploadPreference.SizeFirst:
                        {
                            
                        }
                        break;
                    case UploadPreference.QuantityFirst:
                        {

                        }
                        break;
                    default:
                        {

                        }
                        break;
                }
            }
            else if(imageOverNum > 0 && imageOverSize < 0)
            {
                // 上传数量溢出
                // 因上传容量足够，上传偏好仅考虑 SizeFirst  
                switch(imageUploadPreference)
                {
                    case UploadPreference.SizeFirst:
                        remainImages.OrderByDescending(i => i.Length)
                                    .Take(imageMaxAllowedNum)
                                    .ForEach(image =>
                                    {
                                        image.Progress = 0;
                                        image.Message = "图片上传数量超出限制";
                                        image.State = ImageUploadState.Failed;
                                    });
                        break;
                    default:
                        remainImages.Take(imageMaxAllowedNum)
                                    .ForEach(image =>
                                    {
                                        image.Progress = 0;
                                        image.Message = "图片上传数量超出限制";
                                        image.State = ImageUploadState.Failed;
                                    });
                        break;
                }
            }
            else if(imageOverNum < 0 && imageOverSize > 0)
            {
                // 上传容量溢出
                switch (imageUploadPreference)
                {
                    case UploadPreference.SizeFirst:
                        {
                            // 优先上传大尺寸照片
                            // 按照图片尺寸降序排列，上传最大尺寸照片后，从下一个满足剩余上传容量的图片继续检索
                            var finalImagesTotalSize = 0L;
                            var finalImages = remainImages.OrderByDescending(i => i.Length).ToArray();
                            for (int i = 0; i < finalImages.Length; i++)
                            {
                                finalImagesTotalSize += finalImages[i].Length;
                                if (finalImagesTotalSize > imageMaxAllowedSize)
                                {
                                    finalImages.Skip(i).ForEach(image =>
                                    {
                                        image.Progress = 0;
                                        image.Message = "图片上传数量超出限制";
                                        image.State = ImageUploadState.Failed;
                                    });
                                    break;
                                }
                            }
                        }
                        break;
                    case UploadPreference.QuantityFirst:
                        {

                        }
                        break;
                    default:
                        {
                            // 不对图片进行排序
                            var finalImagesTotalSize = 0L; 
                            var finalImages = remainImages.ToArray();
                            for(int i = 0; i < finalImages.Length; i++)
                            {
                                finalImagesTotalSize += finalImages[i].Length;
                                if(finalImagesTotalSize > imageMaxAllowedSize)
                                {
                                    finalImages.Skip(i).ForEach(image =>
                                    {
                                        image.Progress = 0;
                                        image.Message = "图片上传数量超出限制";
                                        image.State = ImageUploadState.Failed;
                                    });
                                    break;
                                }
                            }
                        }
                        break;
                }
            }
        }
    }


    /// <summary>
    /// 上传偏好设置
    /// </summary>
    public enum UploadPreference
    {
        None = 0,           // 默认
        SizeFirst,          // 尺寸优先（上传超出限制时优先上传大尺寸图片）
        QuantityFirst       // 数量优先（上传超出限制时优先上传小尺寸图片）
    }
}
