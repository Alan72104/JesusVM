using JesusASM;
using JesusASM.Lexer;

namespace JesusVM.Test;

internal class Program
{
    internal static void Main(string[] args)
    {
        Lexer lexer = new();
        JesusAsm jasm = new();
        LexedSource src = lexer.LexSource(
            """
            .entry main
            .stack 1024

            define public main()i:
            	ldi 2
            	ldi 34
            	ldi 35

            	call test(ii)i

            	mul

            	hlt

            define public test(ii)i:
            	load 0
            	load 1

            	add

            	ret
            """);

        int lastLinePrinted = -1;
        foreach (var tok in src.Tokens)
        {
            string val = Escape(tok.Value);
            val = $"\"{val}\"";

            string line = "";
            if (tok.Loc.Line > lastLinePrinted)
            {
                line = Escape(src.Lines[tok.Loc.Line]);
                line = $"\"{line}\"";
                lastLinePrinted = tok.Loc.Line;
            }

            Console.WriteLine($"{tok.Type,15} {tok.Loc,10} {val,-30} {line}");
        }
        Module module1 = jasm.Compile(src);

        JesusVm jvm = new();
        jvm.AddModules(
        [
            module1
        ]);
        jvm.Run();
    }

    private static string Escape(string s)
    {
        return s
            .Replace("\t", "\\t")
            .Replace("\r", "\\r")
            .Replace("\n", "\\n");
    }
}
