namespace JesusASM.Lexing;

public readonly record struct SourceLocation
(
    int Line,
    int Char,
    int Index
)
{
    public override string ToString()
    {
        return $"({Char}:{Line},{Index})";
    }
}
