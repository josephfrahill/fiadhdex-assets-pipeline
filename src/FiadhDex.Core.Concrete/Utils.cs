namespace FiadhDex.Core.Concrete;

public static class Utils
{
    public static string SanitiseFileName(string title)
    {
        title = Path.GetInvalidFileNameChars().Aggregate(title, (current, c) => current.Replace(c, '_'));

        title = title.Replace(' ', '_');

        return title;
    }

    public static string GetSolutionDirectory()
    {
        var currentDir = new DirectoryInfo(AppContext.BaseDirectory);

        while (currentDir != null)
        {
            if (currentDir.GetFiles("*.slnx").Length != 0)
            {
                return currentDir.FullName;
            }

            currentDir = currentDir.Parent;
        }

        throw new FileNotFoundException("Could not find the solution (.sln) root directory.");
    }
}