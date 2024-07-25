using JesusASM.Lexing;
using Pastel;

namespace JesusASM.Parsing;

public class ParseException : Exception
{
    private ParseException(string msg) : base(msg)
    {
    }

    public static ParseException New(LexedSource src, Token? tokenOpt, string msg)
    {
        if (tokenOpt is not null)
        {
            Token token = tokenOpt.Value;
            string line = src.Lines[token.Loc.Line].TrimEnd().Replace('\t', ' ');

            string one = line[0..token.Loc.Char];
            string two = token.Value.Pastel(ConsoleColor.Magenta);
            string three = line[(token.Loc.Char + token.Value.Length)..];
            string four = new string(' ', token.Loc.Char);
            string five = new string('^', token.Value.Length).Pastel(ConsoleColor.Red);

            msg = $"""
                  {msg}
                          at {token.Loc.ToString().Pastel(ConsoleColor.Red)}
                          {one}{two}{three}
                          {four}{five}
                  """;
            return new ParseException(msg);
        }
        else
        {
            string line = src.Lines.Count > 0 ? src.Lines[^1].Replace('\t', ' ') : "";

            string one = line;
            string four = new string(' ', line.Length);
            string five = new string('^', 1).Pastel(ConsoleColor.Red);

            msg = $"""
                  {msg}
                          at {"EOF".ToString().Pastel(ConsoleColor.Red)}
                          {one}
                          {four}{five}
                  """;
            return new ParseException(msg);
        }
    }
}
