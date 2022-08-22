using MudBlazor;
using North.Common;
using NuGet.Protocol.Core.Types;

namespace North.Pages
{
    partial class Plugins
    {
        private string PackageName { get; set; } = string.Empty;
        private bool PackageSearching { get; set; } = false;
        private IEnumerable<IPackageSearchMetadata> Packages { get; set; } = Enumerable.Empty<IPackageSearchMetadata>();

        private async Task SearchPackagesAsync()
        {
            try
            {
                await InvokeAsync(() =>
                {
                    PackageSearching = true;
                    StateHasChanged();
                });
                Packages = await GlobalValues.NugetEngine.GetPackagesAsync(PackageName);
            }
            catch (Exception e)
            {
                _logger.Error("Search package error", e);
                _snackbar.Add("搜索失败，服务器内部错误", Severity.Error);
            }
            finally
            {
                PackageSearching = false;
            }
        }


        /// <summary>
        /// 格式化字符串
        /// </summary>
        /// <param name="str">源字符串</param>
        /// <param name="maxLength">最大长度</param>
        /// <returns></returns>
        private string FormatString(string str, int maxLength = 60)
        {
            if(str.Length <= maxLength)
            {
                return str;
            }
            else
            {
                var formatDescription = str[..(maxLength - 3)];
                var lastSpliterIndex = formatDescription.Length;
                for(int i = 0; i < formatDescription.Length; i++)
                {
                    if((formatDescription[i] == ' ') || (formatDescription[i] == ','))
                    {
                        lastSpliterIndex = i;
                    }
                }
                return $"{formatDescription[0..lastSpliterIndex]}...";
            }
        }
    }
}
