using DevopsDeploy.Exceptions;
using JsonPact.NewtonSoft;
using Newtonsoft.Json;
using Serilog;

namespace DevopsDeploy;

public static class Utils {
    /// <summary>
    /// Simple extension on string to remove any pre-fixed and post-fixed whitespace
    /// and convert the string to lower to prevent key mis-matches.
    /// </summary>
    public static string Sanitise(this string value) => value.Trim().ToLower();

    /// <summary>
    /// Simple extension on dictionary to get a value from a <see cref="Sanitise"/>d key.
    /// </summary>
    public static T? GetOrDefault<T>(this IReadOnlyDictionary<string, T> map, string key) => map.GetValueOrDefault(key.Sanitise());

    /// <summary>
    /// Add simple async deserialize as newtonsoft doesn't support async.
    /// </summary>
    public static Task<HashSet<T>> DeserializeHashSetAsync<T>(this JsonOptions options, string path) => Task.Run(() => {
        try {
            var serializer = JsonSerializer.Create(options);
            using var jsonReader = new JsonTextReader(File.OpenText(path));

            // We use a 'HashSet' to avoid any possible duplicate data for iteration performance.
            return serializer.Deserialize<HashSet<T>>(jsonReader) ??
                   throw new ValidationException($"Expected valid a list of {typeof(T).Name}'s.");
        } catch (Exception) {
            Log.Error("Failed to attempting to parse json from `{Path}` for {Type}", path, typeof(T).Name);
            throw;
        }
    });
}
