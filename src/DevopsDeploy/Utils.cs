using JsonPact.NewtonSoft;
using Newtonsoft.Json;
using Serilog;

namespace DevopsDeploy;

public static class Utils
{
    public static string Sanitise(this string value) => value.Trim().ToLower();

    public static T? GetOrDefault<T>(this IReadOnlyDictionary<string, T> map, string key) => map.GetValueOrDefault(key.Sanitise());

    /// <summary>
    /// Add simple async deserialize as newtonsoft doesn't support async.
    /// </summary>
    public static Task<HashSet<T>> DeserializeHashSetAsync<T>(this JsonOptions options, string path) => Task.Run(() =>
    {
        try
        {
            var serializer = JsonSerializer.Create(options);
            using var jsonReader = new JsonTextReader(File.OpenText(path));

            // We use a 'HashSet' to avoid any possible duplicate data for iteration performance.
            return serializer.Deserialize<HashSet<T>>(jsonReader) ??
                   throw new ArgumentException($"Expected valid a list of {typeof(T).Name}'s.");
        }
        catch (Exception)
        {
            Log.Error("Failed to attempting to parse json from `{Path}` for {Type}", path, typeof(T).Name);
            throw;
        }
    });
}