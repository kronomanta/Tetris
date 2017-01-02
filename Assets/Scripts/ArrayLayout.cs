using System.Linq;

public class Pattern
{
    public readonly int[][] SymbolRows;

    public Pattern(params int[][] symbolRows)
    {
        SymbolRows = symbolRows;
        Width = SymbolRows[0].Length;
    }

    public int[] this[int index]
    {
        get
        {
            return SymbolRows[index];
        }
    }

    public int Length { get { return SymbolRows.Length; } }
    public int Width { get; private set; }

    public Pattern Clone() {
        return new Pattern(SymbolRows.Select(x => (int[])x.Clone()).ToArray());
    }

}

public class PatternContainer
{
    public readonly Pattern[] Patterns;

    public PatternContainer(params Pattern[] patterns)
    {
        Patterns = patterns;
    }

    public Pattern this[int index]
    {
        get
        {
            return Patterns[index];
        }
    }

    public int Length { get { return Patterns.Length; } }

    public PatternContainer Clone() {
        return new PatternContainer(Patterns.Select(x => x.Clone()).ToArray());
    }
}

