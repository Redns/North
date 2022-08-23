namespace North.PluginBase
{
    public interface IStorage
    {
        string Id { get; }
        string Upload(Stream stream, string imageName);
        bool Delete();

    }
}
