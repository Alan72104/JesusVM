namespace JesusASM;

public class Module
{
    public required Dictionary<string, string> GlobalDefinitions { get; set; }
    public required Dictionary<string, byte[]> Functions { get; set; }
}
