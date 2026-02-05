using System;

namespace Kresleni;

public class Coords
{
    public int X;
    public int Y;

    public Coords(int x, int y)
    {
        X = x;
        Y = y;
    }
        
    public static implicit operator Coords((int x, int y) tuple)
    {
        return new Coords(tuple.x, tuple.y);
    }
        
    public static implicit operator Coords((double x, double y) tuple)
    {
        return new Coords((int)Math.Round(tuple.x), (int)Math.Round(tuple.y));
    }
}