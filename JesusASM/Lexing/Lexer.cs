using System.Diagnostics;
using System.Text;

namespace JesusASM.Lexing;

public class LexedSource
{
    public required List<string> Lines { get; set; }
    public required List<Token> Tokens { get; set; }
}

public class Lexer
{
    private static List<KeyValuePair<TokenType, string>> Literals;

    private char Cur =>
        idx >= str.Length ? '\0' : str[idx];
    private SourceLocation Location =>
        new(lines, chars, idx);

    private List<Token> tokens = new();
    private List<string> srcLines = new();
    private string str = null!;
    private int lines;
    private int chars;
    private int idx;
    private int lineStartIdx;
    private StringBuilder errSeq = new();
    private SourceLocation errSeqLoc;

    static Lexer()
    {
        Literals = new()
        {
            new(TokenType.Period,  "."),
            new(TokenType.Comma,   ","),
            new(TokenType.Colon,   ":"),
            new(TokenType.LParen,  "("),
            new(TokenType.RParen,  ")"),
            new(TokenType.Equals,  "="),
            new(TokenType.Define,  "define"),
            new(TokenType.Public,  "public"),
            new(TokenType.Private, "private"),
        };
    }

    public LexedSource LexSource(string s)
    {
        tokens.Clear();
        srcLines.Clear();
        str = s;
        lines = 0;
        chars = 0;
        idx = 0;
        lineStartIdx = 0;
        errSeq.Clear();

        while (Cur != '\0')
        {
            if (TryLex(out Token tok))
            {
                if (errSeq.Length > 0)
                {
                    tokens.Add(new Token(TokenType.Error, errSeq.ToString(), errSeqLoc));
                    errSeq.Clear();
                }
                tokens.Add(tok);
            }
            else
            {
                if (errSeq.Length == 0)
                    errSeqLoc = Location;
                errSeq.Append(Eat());
            }
        }

        if (errSeq.Length > 0)
        {
            tokens.Add(new Token(TokenType.Error, errSeq.ToString(), errSeqLoc));
            errSeq.Clear();
        }

        if (idx - lineStartIdx > 0)
        {
            srcLines.Add(str[lineStartIdx..(idx)]);
            lineStartIdx = idx + 1;
        }

        return new LexedSource()
        {
            Lines = srcLines,
            Tokens = tokens,
        };
    }

    private bool TryLex(out Token token)
    {
        Debug.Assert(HasMore());
        SourceLocation start = Location;

        foreach (var (type, value) in Literals)
        {
            if (Matches(value))
            {
                token = new(type, Eat(value), start);
                return true;
            }
        }

        if (Cur is
            '_' or
            (>= 'a' and <= 'z') or
            (>= 'A' and <= 'Z'))
        {
            Eat();
            while (HasMore() && Cur is
                '_' or
                (>= 'a' and <= 'z') or
                (>= 'A' and <= 'Z') or
                (>= '0' and <= '9'))
            {
                Eat();
            }
            token = new(TokenType.Identifier, str[start.Index..idx], start);
            return true;
        }
        else if (Cur is >= '0' and <= '9')
        {
            while (HasMore() && Cur is >= '0' and <= '9')
            {
                Eat();
            }
            token = new(TokenType.Number, str[start.Index..idx], start);
            return true;
        }
        else if (Cur is '"')
        {
            Eat();
            while (HasMore())
            {
                if (Cur is '"')
                {
                    Eat();
                    token = new(TokenType.String, str[start.Index..idx], start);
                    return true;
                }
                Eat();
            }
            // Unterminated
            token = new(TokenType.Error, str[start.Index..idx], start);
            return true;
        }
        else if (char.IsWhiteSpace(Cur))
        {
            while (HasMore() && char.IsWhiteSpace(Cur))
            {
                if (Cur == '\n')
                {
                    srcLines.Add(str[lineStartIdx..(idx + 1)]);
                    lineStartIdx = idx + 1;
                    chars = -1; // Incremented to 0 in Eat()
                    lines++;
                }
                Eat();
            }
            token = new(TokenType.Whitespace, str[start.Index..idx], start);
            return true;
        }

        token = default;
        return false;
    }

    private char Eat()
    {
        chars++;
        return str[idx++];
    }

    private string Eat(string s)
    {
        Debug.Assert(Matches(s));
        chars += s.Length;
        idx += s.Length;
        return s;
    }

    private bool Matches(ReadOnlySpan<char> s)
    {
        return str.Length - idx >= s.Length &&
            str.AsSpan()[idx..(idx + s.Length)].SequenceEqual(s);
    }

    private char Peek(int n)
    {
        if (idx + n >= str.Length)
            return '\0';
        else
            return str[idx + n];
    }

    private bool HasMore()
    {
        return idx < str.Length;
    }
}
