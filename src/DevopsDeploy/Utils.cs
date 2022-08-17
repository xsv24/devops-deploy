namespace DevopsDeploy;

public static class Utils
{
    public static string Sanitise(this string value) => value.Trim().ToLower();

    public static T? GetOrDefault<T>(this IReadOnlyDictionary<string, T> map, string key) => map.GetValueOrDefault(key.Sanitise());
}