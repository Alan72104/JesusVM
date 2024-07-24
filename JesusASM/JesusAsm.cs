using JesusASM.Lexing;
using JesusASM.Parsing;

namespace JesusASM;

public class JesusAsm
{
    private Parser parser = new();

    public Module Compile(LexedSource src)
    {
        Module module = new()
        {
            Functions = new(),
            GlobalDefinitions = new()
        };

        parser.Module(module, src);

        return module;
    }
}
