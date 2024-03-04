namespace Cloak.Core;

internal sealed class Generator
{
    private readonly Random _random = new();
    private readonly List<int> _alreadyGeneratedInts = [];

    internal int GenerateInt()
    {
        start:
        var i = _random.Next(1000000000, 999999999);
        if (_alreadyGeneratedInts.Contains(i)) goto start;
        _alreadyGeneratedInts.Add(i);
        return i;
    }
}