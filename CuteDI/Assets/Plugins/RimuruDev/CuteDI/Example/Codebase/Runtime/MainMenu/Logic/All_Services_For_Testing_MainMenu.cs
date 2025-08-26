namespace AbyssMoth.CuteDI.Example
{
    public interface IMenuService { }
    
    public sealed class MenuService : IMenuService { }
    
    public interface IMenuRoot { }

    public sealed class MenuViewModel
    {
        public MenuViewModel(IMenuService s) { }
    }
}