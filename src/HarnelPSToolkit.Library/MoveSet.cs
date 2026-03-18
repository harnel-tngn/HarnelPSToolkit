namespace HarnelPSToolkit.Library;

[IncludeIfReferenced]
public static class Moveset
{
    public static readonly (int dy, int dx)[] Moveset4 = new (int dy, int dx)[]
    {
        (-1,0),(1,0),(0,-1),(0,1),
    };

    public static readonly (int dy, int dx)[] Moveset8 = new (int dy, int dx)[]
    {
        (-1,-1),(-1,0),(-1,1),(0,-1),(0,1),(1,-1),(1,0),(1,1)
    };
}
