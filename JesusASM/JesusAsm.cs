using JesusASM.Lexer;

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
