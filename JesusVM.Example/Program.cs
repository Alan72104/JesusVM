﻿using JesusASM;
using JesusASM.Lexing;

namespace JesusVM.Example;

internal class Program
{
    internal static void Main(string[] args)
    {
        Lexer lexer = new();
        JesusAsm jasm = new();
        LexedSource src = lexer.LexSource(
            """
            .module "Test"
            .stack 2mb

            const "main()i" = .function int main()
            const "test(ii)i" = .function int test(int, int)

            section code

            public int main():
                .locals 1

                ldi 420
                store 0

                ldi 69
                load 0

                add
                dup

                call "test(ii)i"

                ret

            public int test(int, int):
                load 0
                load 1

                mul

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
