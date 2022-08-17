namespace DevopsDeploy;

public static class Utils
{
    public static string Sanitise(this string value) => value.Trim().ToLower();

    public static bool HasKey<T>(this Dictionary<string, T> map, string key) => map.ContainsKey(key.Sanitise());
}