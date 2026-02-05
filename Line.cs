using System;

namespace Kresleni;

public class Line
{
    public Coords Start;
    public Coords End;
    public int Width;
    public Color Color;
                
    public bool ConstraintMode;
    public bool DashedMode;


    public Line(Coords start, Coords end, int width, Color? color = null)
    {
        color ??= new Color(0, 0, 0);
            
        Start = start;
        End = end;
        Width = width;
        Color = color;
    }

    public Coords ConsttraintEnd()
    {
        var angle = Math.Atan2(Start.X - End.X, Start.Y - End.Y);
        if (angle < 0)
            angle += 2*Math.PI;

        double length = Math.Sqrt(Math.Pow(Start.X - End.X, 2) + Math.Pow(Start.Y - End.Y, 2));

        double quadrant = Math.PI / 4;
        double sqrTwo = 1.41;  // 1.41421356237
        angle += quadrant / 2;

        //if (angle / quadrant % 2 == 0) length /= sqrTwo;
        //else length = 0;
        //if (angle / quadrant / 4 == 0) length *= -1;
                
        if (angle < quadrant) return (Start.X, Start.Y - (int)length);
        if (angle < quadrant * 2) return (Start.X - (int)(length / sqrTwo), Start.Y - (int)(length / sqrTwo));
        if (angle < quadrant * 3) return (Start.X - (int)length, Start.Y);
        if (angle < quadrant * 4) return (Start.X - (int)(length / sqrTwo), Start.Y + (int)(length / sqrTwo));
        if (angle < quadrant * 5) return (Start.X, Start.Y + (int)length);
        if (angle < quadrant * 6) return (Start.X + (int)(length / sqrTwo), Start.Y + (int)(length / sqrTwo));
        if (angle < quadrant * 7) return (Start.X + (int)length, Start.Y);
        if (angle < quadrant * 8) return (Start.X + (int)(length / sqrTwo), Start.Y - (int)(length / sqrTwo));
        return (Start.X, Start.Y - (int)length);

    }

}