using UnityEngine;

namespace AbyssMoth.CuteDI.Example
{
    public interface IClock { public long NowTicks { get; } }
    public sealed class UtcClock : IClock { public long NowTicks => System.DateTime.UtcNow.Ticks; }
    public sealed class GameClock : IClock { public long NowTicks => (long)(Time.time * 1000f); }

    
    
    public interface IStorage { public string Id { get; } }
    public sealed class MemoryStorage : IStorage { public string Id { get; } = "mem"; }
    public sealed class FileStorage : IStorage { public string Id { get; } = "file"; }

    
    
    public interface IAnalytics { public void Track(string e); }
    public sealed class DummyAnalytics : IAnalytics { public void Track(string e) { } }
}