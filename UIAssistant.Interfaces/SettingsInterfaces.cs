namespace UIAssistant.Interfaces.Settings
{
    public interface IFileIO<T>
    {
        string FilePath { get; }
        T Read();
        void Write(T data);
    }

    public interface ISettings
    {
        void Load();
        void Save();
    }
}
