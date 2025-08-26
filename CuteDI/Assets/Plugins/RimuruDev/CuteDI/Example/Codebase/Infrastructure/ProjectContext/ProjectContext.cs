namespace AbyssMoth.CuteDI.Example
{
    public class ProjectContext : IProjectContext
    {
        public string ContainerName { get; private set; } = "Project Context";
        public DIContainer Container { get; private set; }
        private readonly bool containerNameInConstructorIsEmpty;

        public ProjectContext(string containerName = null)
        {
            if (string.IsNullOrWhiteSpace(containerName))
                containerNameInConstructorIsEmpty = true;
            else
                ContainerName = containerName;

            Container = new DIContainer(containerName: ContainerName);
            DiProvider.SetProject(Container);
        }

        public void Register()
        {
            RegisterSelf();
            RegisterCoroutineRunner();
            RegisterNavigation();
        }

        public void Resolve()
        {
        }

        public void Release()
        {
            Container?.Dispose();
        }

        private void RegisterSelf()
        {
            Container
                .RegisterInstance<IProjectContext>(this,
                    containerNameInConstructorIsEmpty ? null : ContainerName)
                .AsSingle()
                .NonLazy();
        }

        private void RegisterCoroutineRunner() =>
            Container.BindNewGo<ICoroutineRunner, CoroutineRunner>("CoroutineRunner", true);

        private void RegisterNavigation()
        {
            Container.RegisterType(typeof(IGameNavigation), typeof(GameNavigation))
                .AsSingle()
                .NonLazy();
        }
    }
}