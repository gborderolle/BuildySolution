namespace BuildyBackend.Core.Helpers;

public static class Utils
{
    public static string ToCamelCase(string str)
    {
        if (string.IsNullOrEmpty(str))
            return str;

        string[] words = str.Split(' ');
        for (int i = 0; i < words.Length; i++)
        {
            if (words[i].Length > 0)
            {
                words[i] = char.ToUpper(words[i][0]) + words[i].Substring(1).ToLower();
            }
        }
        return string.Join(" ", words);
    }
}
