namespace Services;

public class Utils
{
    public static string SanitiseFileName(string title)
    {
        title = Path.GetInvalidFileNameChars().Aggregate(title, (current, c) => current.Replace(c, '_'));

        title = title.Replace(' ', '_');

        return title;
    }

    public static string GetExecutingAppRoot()
    {
        var currentDir = new DirectoryInfo(AppContext.BaseDirectory);

        while (currentDir != null)
        {
            if (currentDir.GetFiles("*.slnx").Length != 0)
            {
                return Path.Combine(currentDir.FullName, "src", "AnimalAssetsPipeline");
            }

            currentDir = currentDir.Parent;
        }

        throw new FileNotFoundException("Could not find the solution (.sln) root directory.");
    }
}