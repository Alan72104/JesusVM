namespace JesusASM.Lexing;

public readonly struct Token
{
    public readonly TokenType Type;
    public readonly string Value;
    public readonly SourceLocation Loc;

    public Token(TokenType type, char value, SourceLocation loc) :
        this(type, value.ToString(), loc)
    {
    }

    public Token(TokenType type, string value, SourceLocation loc)
    {
        Type = type;
        Value = value;
        Loc = loc;
    }
}
