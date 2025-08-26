namespace AbyssMoth.CuteDI.Example
{
    public interface IProjectContext
    {
        public string ContainerName { get; }
        public DIContainer Container { get; }

        public void Register();
        public void Resolve();
        public void Release();
    }
}