namespace UIAssistant.Infrastructure.Resource.Theme
{
    public class Theme : IResourceItem
    {
        public string Id { get; }
        public string FilePath { get; }

        public Theme(string id, string filePath)
        {
            Id = id;
            FilePath = filePath;
        }
    }
}
