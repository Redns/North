namespace North.Pages
{
    partial class Plugins
    {
        public bool IsSearching { get; set; }

        public string SearchPluginName {  get; set; }

        /// <summary>
        /// 搜索插件
        /// </summary>
        /// <returns></returns>
        public async Task SearchPluginsAsync()
        {
            await Task.CompletedTask;
        }
    
    }
}