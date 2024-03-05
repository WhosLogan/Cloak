using System.Text;

namespace Cloak.Core;

internal sealed class Generator
{
    private readonly Random _random = new();
    private readonly List<int> _alreadyGeneratedInts = [];
    private readonly Dictionary<string, string> _oldNames = new();
    private readonly List<string> _alreadyGeneratedStrings = [];
    private static readonly List<char> AcceptableChars = [];
    private const int NameLength = 8;

    static Generator()
    {
        for (var i = 65; i < 90; i++)
        {
            // A-Z
            AcceptableChars.Add((char)i);
            
            // a-z
            AcceptableChars.Add((char)(i + 32));
        }
    }

    internal int GenerateInt()
    {
        start:
        var i = _random.Next();
        if (_alreadyGeneratedInts.Contains(i)) goto start;
        _alreadyGeneratedInts.Add(i);
        return i;
    }

    internal string GenerateName(string oldName)
    {
        if (_oldNames.TryGetValue(oldName, out var newName))
            return newName;
        var str = GenerateName();
        _oldNames.Add(oldName, str);
        return str;
    }

    internal string GenerateName()
    {
        start:
        var builder = new StringBuilder();
        for (var i = 0; i < NameLength; i++)
        {
            builder.Append(AcceptableChars[_random.Next(0, AcceptableChars.Count - 1)]);
        }
        
        var str = builder.ToString();
        if (_alreadyGeneratedStrings.Contains(str))
            goto start;
        _alreadyGeneratedStrings.Add(str);
        return str;
    }
}