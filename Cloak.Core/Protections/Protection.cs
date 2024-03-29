namespace Cloak.Core.Protections;

public abstract class Protection(string name, string description)
{
    public string Name { get; } = name;
    public string Description { get; } = description;
    public bool Enabled { get; set; }

    internal virtual List<string> RequiredTypes { get; } = [];

    internal abstract void Execute(Cloak cloak);
}