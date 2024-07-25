using JesusASM.Lexing;

namespace JesusASM.Parsing;

public class Parser
{
    private Token Cur
    {
        get
        {
            if (!HasMore())
                throw new InvalidOperationException($"No more tokens");
            return src.Tokens[idx];
        }
    }

    private Module module = null!;
    private LexedSource src = null!;
    private int idx;

    public void ModuleDef(Module module, LexedSource src)
    {
        this.module = module;
        this.src = src;
        idx = 0;

        while (HasMore())
        {
            Expect([TokenType.Period, TokenType.Define]);
            if (Cur.Type == TokenType.Period)
            {
                Metadata();
            }
            else if (Cur.Type == TokenType.Define)
            {
                FuncDef();
            }
        }
    }

    private void Metadata()
    {
        Eat(TokenType.Period);
        Token key = Eat(TokenType.Identifier);
        Token value = Eat([TokenType.Identifier, TokenType.Number]);
        module.GlobalDefinitions.Add(key.Value, value.Value);
    }

    private void FuncDef()
    {
        Eat(TokenType.Define);
        Eat([TokenType.Public, TokenType.Private]);

        Token tok = Eat(TokenType.Identifier);
        if (tok.Value.Any(c => c is >= '0' and <= '9'))
            throw NewException($"Function name cannot contain numbers");

        string name = tok.Value;

        tok = Eat(TokenType.LParen); name += tok.Value;
        if (Optional(TokenType.Identifier, out tok))
        {
            if (tok.Value.Any(c => c is >= '0' and <= '9'))
                throw NewException($"Function argument list cannot contain numbers");
            name += tok.Value;
        }
        tok = Eat(TokenType.RParen); name += tok.Value;
        tok = Eat(TokenType.Identifier); name += tok.Value;
        if (tok.Value.Any(c => c is >= '0' and <= '9'))
            throw NewException($"Function return type cannot contain numbers");
        Eat(TokenType.Colon);

        while (HasMore() && Cur.Type is
            TokenType.Identifier or
            TokenType.Number or
            TokenType.LParen or
            TokenType.RParen)
        {
            Eat();
        }

        module.Functions.Add(name, new byte[0]);
    }

    private bool OptionalTypeDef(out string type)
    {
        //if (Cur.Type == TokenType.Identifier)
        //    Token tok = Eat(TokenType.Identifier);
        type = "";
        return false;
    }

    private void Expect(TokenType type)
    {
        if (!HasMore() || Cur.Type != type)
        {
            string curType = HasMore() ? Cur.Type.ToString() : "EOF";
            throw NewException($"Expected {type} but got {curType}");
        }
    }

    private void Expect(IEnumerable<TokenType> types)
    {
        if (!HasMore() || !types.Any(type => Cur.Type == type))
        {
            string curType = HasMore() ? Cur.Type.ToString() : "EOF";
            throw NewException($"Expected [{string.Join(',', types)}] but got {curType}");
        }
    }

    private Token Eat(TokenType type)
    {
        Expect(type);
        return Eat();
    }

    private Token Eat(IEnumerable<TokenType> types)
    {
        Expect(types);
        return Eat();
    }

    private Token Eat()
    {
        return src.Tokens[idx++];
    }

    private bool Optional(TokenType type, out Token token)
    {
        if (HasMore() && Cur.Type == type)
        {
            token = Eat();
            return true;
        }
        token = default;
        return false;
    }

    private Token Peek(int n)
    {
        if (idx + n >= src.Tokens.Count)
            throw new InvalidOperationException($"Can't peek over {src.Tokens.Count} at {n}");
        else
            return src.Tokens[idx + n];
    }

    private bool HasMore()
    {
        return idx < src.Tokens.Count;
    }

    private ParseException NewException(string msg)
    {
        return ParseException.New(src, HasMore() ? Cur : null, msg);
    }
}
