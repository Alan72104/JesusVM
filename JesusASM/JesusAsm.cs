using JesusASM.Lexing;

namespace JesusASM;

public class JesusAsm
{
    public Module Compile(LexedSource src)
    {
        return new Module()
        {
            Functions = new(),
            GlobalDefinitions = new()
        };
    }
}
