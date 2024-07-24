namespace JesusASM.Lexer;

public struct Token
{
    public TokenType Type;
    public string Value;
    public SourceLocation Loc;

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
