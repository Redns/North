namespace North.PluginBase
{
    public interface IStorage
    {
        string Upload(Stream stream, string imageName);
        ValueTask<string> UploadAsync(Stream stream, string imageName);
    }
}
