using System.Globalization;

namespace OsuDojo.Web.Utility;

public static class StringExtension
{
    public static string ToPascalCase(this string text)
    {
        var textInfo = CultureInfo.InvariantCulture.TextInfo;
        return string.Concat(text.Split('_').Select(textInfo.ToTitleCase));
    }
}
