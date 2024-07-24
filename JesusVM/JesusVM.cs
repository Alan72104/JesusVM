using JesusASM;

namespace JesusVM;

public class JesusVm
{
    private List<Module> modules = new();

    public void AddModules(IEnumerable<Module> modules)
    {
        this.modules.AddRange(modules);
    }

    public void Run()
    {
    }
}
