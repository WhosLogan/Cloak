namespace Cloak.Core;

public abstract class Protection(string name, string description)
{
    public string Name { get; } = name;
    public string Description { get; } = description;
    public bool Enabled { get; set; }

    internal abstract void Execute(Cloak cloak);
}