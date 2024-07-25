namespace JesusASM.Lexing;

public enum TokenType
{
    Error,
    Whitespace,

    Period,
    Colon,
    LParen,
    RParen,

    Identifier,
    String,
    Number,

    Define,
    Public,
    Private,
}
