namespace Cloak.Core.Processors;

internal abstract class Processor
{
    internal virtual void PreProcess(Cloak cloak) {}
    
    internal virtual void PostProcess(Cloak cloak) {}
}