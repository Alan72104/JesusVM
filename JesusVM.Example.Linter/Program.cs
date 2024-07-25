using JesusASM.Lexing;
using Pastel;
using System.Text;

namespace JesusVM.Example.Linter;

internal class Program
{
    private static Lexer lexer = new();
    private static string filePath = "";

    internal static void Main(string[] args)
    {
        if (args.Length == 0)
            return;
        filePath = args[0];
        if (!File.Exists(filePath))
        {
            Console.WriteLine("File doesn't exist");
            return;
        }
        filePath = Path.GetFullPath(filePath);

        using var watcher = new FileSystemWatcher(
            Path.GetDirectoryName(filePath)!,
            Path.GetFileName(filePath));
        watcher.NotifyFilter = NotifyFilters.LastAccess
                             | NotifyFilters.LastWrite
                             | NotifyFilters.Size;
        watcher.Changed += OnChanged;
        watcher.EnableRaisingEvents = true;

        Console.CursorVisible = false;
        Console.ReadLine();
    }

    private static void OnChanged(object sender, FileSystemEventArgs e)
    {
        if (e.ChangeType != WatcherChangeTypes.Changed)
            return;
        using Stream stream = new FileStream(filePath, FileMode.Open,
            FileAccess.Read, FileShare.ReadWrite);
        using StreamReader reader = new StreamReader(stream);
        Print(reader.ReadToEnd());
    }

    private static void Print(string file)
    {
        try
        {
            LexedSource src = lexer.LexSource(file);
            Console.Clear();
            Console.WriteLine(GetColoredSource(src));
        }
        catch (Exception)
        {
        }
    }

    private static string GetColoredSource(LexedSource src)
    {
        Dictionary<TokenType, ConsoleColor> colorMap = new()
        {
            { TokenType.Error, ConsoleColor.Red},
            { TokenType.Whitespace, ConsoleColor.Gray},
            { TokenType.Period, ConsoleColor.Gray},
            { TokenType.Comma, ConsoleColor.Gray},
            { TokenType.Colon, ConsoleColor.Gray},
            { TokenType.LParen, ConsoleColor.Yellow},
            { TokenType.RParen, ConsoleColor.Yellow},
            { TokenType.Equals, ConsoleColor.Gray},
            { TokenType.Identifier, ConsoleColor.Cyan},
            { TokenType.String, ConsoleColor.DarkCyan},
            { TokenType.Number, ConsoleColor.DarkCyan},
            { TokenType.Define, ConsoleColor.Yellow},
            { TokenType.Public, ConsoleColor.Yellow},
            { TokenType.Private, ConsoleColor.Yellow},
        };
        StringBuilder sb = new();
        foreach (var tok in src.Tokens)
        {
            sb.Append(tok.Value.Pastel(colorMap[tok.Type]));
        }
        return sb.ToString();
    }
}
