namespace Services;

public class Utils
{
    public static string SanitiseFileName(string title)
    {
        title = Path.GetInvalidFileNameChars().Aggregate(title, (current, c) => current.Replace(c, '_'));

        title = title.Replace(' ', '_');

        return title;
    }
}