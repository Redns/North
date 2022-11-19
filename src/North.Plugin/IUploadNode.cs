using North.Core.Models;

namespace North.Plugin
{
    /// <summary>
    /// 图片上传流程节点
    /// </summary>
    public interface IUploadNode
    {
        Task InvokeAsync(ImageUploadModel image);
    }
}
