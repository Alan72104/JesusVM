using System.Diagnostics;
using System.Text;

namespace JesusASM.Lexer;

public class LexedSource
{
    public required List<string> Lines { get; set; }
    public required List<Token> Tokens { get; set; }
}

public class Lexer
{
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
    private StringBuilder seq = new();
    private StringBuilder errSeq = new();
    private SourceLocation errSeqLoc;

    public LexedSource LexSource(string s)
    {
        tokens.Clear();
        srcLines.Clear();
        str = s;
        lines = 0;
        chars = 0;
        idx = 0;
        lineStartIdx = 0;
        seq.Clear();
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

        Console.WriteLine($"Lexed {str.Length} chars");
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
        if (Cur == '.')
        {
            token = new(TokenType.Period, Eat(), start);
            return true;
        }
        else if (Cur == ':')
        {
            token = new(TokenType.Colon, Eat(), start);
            return true;
        }
        else if (Cur == '(')
        {
            token = new(TokenType.LParen, Eat(), start);
            return true;
        }
        else if (Cur == ')')
        {
            token = new(TokenType.RParen, Eat(), start);
            return true;
        }
        else if (Matches("define"))
        {
            token = new(TokenType.Define, Eat("define"), start);
            return true;
        }
        else if (Matches("public"))
        {
            token = new(TokenType.Public, Eat("public"), start);
            return true;
        }
        else if (Matches("private"))
        {
            token = new(TokenType.Private, Eat("private"), start);
            return true;
        }
        else if (Cur is
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
        else if (char.IsWhiteSpace(Cur))
        {
            while (HasMore() && char.IsWhiteSpace(Cur))
            {
                if (Cur == '\n')
                {
                    srcLines.Add(str[lineStartIdx..(idx + 1)]);
                    lineStartIdx = idx + 1;
                    chars = 0;
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

    private bool Matches(string s)
    {
        return str.Length - idx >= s.Length &&
            str[idx..(idx + s.Length)] == s;
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
