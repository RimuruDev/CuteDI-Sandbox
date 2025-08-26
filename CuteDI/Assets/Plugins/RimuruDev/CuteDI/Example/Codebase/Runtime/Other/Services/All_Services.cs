using System.Diagnostics;

namespace AbyssMoth.CuteDI.Example
{
    public sealed class TaggedConsumer
    {
        public IStorage Mem { get; }
        public IStorage File { get; }
       
        public TaggedConsumer(IStorage mem, IStorage file)
        {
            Mem = mem;
            File = file;
            Debug.Assert(Mem != null);
            Debug.Assert(File != null);
        }
    }

    public sealed class AttrConsumer
    {
        public IStorage File { get; }
        public IClock Clock { get; }
       
        [InjectionConstructor]
        public AttrConsumer([Tag("file")] IStorage file, [Tag("utc")] IClock clock)
        {
            File = file;
            Clock = clock;
            Debug.Assert(File != null);
            Debug.Assert(Clock != null);
        }
    }

    public sealed class FactoryProduct
    {
        [InjectionConstructor]
        public FactoryProduct(IEnemySpawner spawner, IGameNavigation navigation)
        {
            Debug.Assert(spawner != null);
            Debug.Assert(navigation != null);
        }
    }
    
    public interface IProcessor { public void Execute(); }
    public sealed class ProcA : IProcessor { public void Execute() { } }
    public sealed class ProcB : IProcessor { public void Execute() { } }

    public sealed class ProcessorAggregator
    {
        public IProcessor[] Items { get; }
        
        public ProcessorAggregator(IProcessor[] items)
        {
            Items = items;
            Debug.Assert(Items != null && Items.Length >= 2);
        }
    }
    
    
    public interface IReplaceSample { public string Name { get; } }
    public sealed class ReplaceA : IReplaceSample { public string Name => "A"; }
    public sealed class ReplaceB : IReplaceSample { public string Name => "B"; }
}