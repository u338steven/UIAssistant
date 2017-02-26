namespace UIAssistant.Infrastructure.Resource.Language
{
    public class Language : IResourceItem
    {
        public string Id { get; }
        public string FilePath { get; }
        public string DisplayName { get; }

        public Language(string id, string filePath, string displayName)
        {
            Id = id;
            FilePath = filePath;
            DisplayName = displayName;
        }

        public override string ToString()
        {
            return DisplayName;
        }
    }
}
