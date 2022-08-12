namespace North.Models.Events.PasteMultimediaEvent
{
    /// <summary>
    /// PasteMultimedia 事件参数
    /// </summary>
    public class PasteMultimediaEventsArgs : EventArgs
    {
        /// <summary>
        /// 剪贴板是否为文本文件
        /// </summary>
        public bool IsMultimedia { get; set; }

        /// <summary>
        /// 剪贴板数据
        /// 1.若为文本，则 Data[0] 即为文本数据
        /// 2.若为文件，则 Data 中每个元素的格式为 "{content_type}&{blob_url}"
        /// </summary>
        public string[] Data { get; set; }

        /// <summary>
        /// 剪贴板文本
        /// </summary>
        public string Text => IsMultimedia ? string.Empty : Data[0];

        /// <summary>
        /// 剪贴板 Blob 数据
        /// </summary>
        public Blob[] Blobs
        {
            get
            {
                return IsMultimedia ? Array.ConvertAll(Data, data =>
                {
                    var blobParams = data.Split('&');
                    return new Blob(blobParams[0], blobParams[1]);
                }) : Array.Empty<Blob>();
            }
        } 

        public PasteMultimediaEventsArgs(bool isMultimedia, string[] data)
        {
            IsMultimedia = isMultimedia;
            Data = data;
        }
    }


    /// <summary>
    /// Blob 数据
    /// </summary>
    public class Blob
    {
        /// <summary>
        /// 文件类型
        /// </summary>
        public string ContentType { get; set; }

        /// <summary>
        /// 链接
        /// </summary>
        public string Url { get; set; }

        public Blob(string contentType, string url)
        {
            ContentType = contentType;
            Url = url;
        }
    }
}
