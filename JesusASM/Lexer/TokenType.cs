namespace JesusASM.Lexer;

public enum TokenType
{
    Error,
    Whitespace,

    Period,
    Colon,
    LParen,
    RParen,

    Identifier,
    Number,

    Define,
    Public,
    Private,
}
