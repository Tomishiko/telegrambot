namespace telegramBot.Services;
public class DirectoryService
{
    readonly DirectoryInfo dirInfo;
    public DirectoryService()
    {
        dirInfo = new DirectoryInfo(Directory.GetCurrentDirectory());
    }
    public FileInfo GetLastFile()
    {
        dirInfo.Refresh();
        return dirInfo.EnumerateFiles().OrderByDescending(f => f.LastWriteTime).First();
    }
}
