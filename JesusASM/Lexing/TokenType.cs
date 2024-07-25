namespace JesusASM.Lexing;

public enum TokenType
{
    Error,
    Whitespace,

    Period,
    Comma,
    Colon,
    LParen,
    RParen,
    Equals,

    Identifier,
    String,
    Number,

    Define,
    Public,
    Private,
}
