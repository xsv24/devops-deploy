namespace DevopsDeploy;
public class NotFoundException : KeyNotFoundException {
    public readonly string Id;

    public readonly string Name;

    public NotFoundException(string id, string name) : base($"No match found for {name} with identifier {id}") {
        Id = id;
        Name = name;
    }

    public void Log() => Serilog.Log.Error(
       "No match found for {name} with identifier {id}",
       Name,
       Id
    );
}
