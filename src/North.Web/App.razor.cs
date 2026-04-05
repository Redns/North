namespace North.Web
{
    public partial class App
    {
        /// <summary>
        /// TODO 通过 API 获取应用名称
        /// </summary>
        public string AppName { get; set; } = "North";

        /// <summary>
        /// TODO 通过 API 获取页脚信息
        /// </summary>
        public string FooterText { get; set; } =
            @"<a href=""https://github.com/Redns/ImageBed"" target=""_blank"" style=""color: rgba(0, 164, 255, 1); font-size: 16px"">Powered by North © 2022-2026 Redns, All rights reserved</a>";
    }
}
