namespace North.PluginBase
{
    public interface INode<T> where T : SettingBase 
    {
        IEnumerable<string> Invoke(in IServiceProvider Services, in T settings, IEnumerable<string> images);
        ValueTask<IEnumerable<string>> InvokeAsync(in IServiceProvider Services, in T settings);
    }
}
