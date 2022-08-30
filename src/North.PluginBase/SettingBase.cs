using System.Text.Json;

namespace North.PluginBase
{
    /// <summary>
    /// 插件设置基类
    /// </summary>
    public class SettingBase
    {
        /// <summary>
        /// 加载设置
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static SettingBase Load()
        {
            return JsonSerializer.Deserialize<SettingBase>(File.ReadAllText("settings.json")) ?? throw new Exception("Load settings failed");
        }


        /// <summary>
        /// 保存设置
        /// </summary>
        public virtual void Save()
        {
            File.WriteAllText("settings.json", ToString());
        }


        public override string ToString()
        {
            return JsonSerializer.Serialize(this);
        }
    }
}
