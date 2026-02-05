namespace Kresleni;

public class Color
{
    public byte R => (byte)_r;
    private int _r; 
        
    public byte G => (byte)_g;
    private int _g; 
        
    public byte B => (byte)_b;
    public int _b;
        
    public byte Alpha => (byte)_alpha;
    public int _alpha;

    public Color(int r, int g, int b, int alpha = 255)
    {
        _r = r;
        _g = g;
        _b = b;
        _alpha = alpha;
    }

    public override string ToString()
    {
        return $"({_r},{_g},{_b},{_alpha})";
    }
}