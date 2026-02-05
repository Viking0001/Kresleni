using Avalonia;
using Avalonia.Controls;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using System;
using System.Threading.Tasks;
using Avalonia.Threading;

namespace Kresleni
{
    public class CanvasBitmap
    {
        private WriteableBitmap _bitmap;
        private Image _target;

        public int Width => _bitmap.PixelSize.Width;
        public int Height => _bitmap.PixelSize.Height;
        public Image Image => _target;
        public WriteableBitmap Bitmap => _bitmap;

        public CanvasBitmap(Image target, int width, int height)
        {
            this._target = target;
            _bitmap = new WriteableBitmap(
                new PixelSize(width, height),
                new Avalonia.Vector(96, 96),
                PixelFormat.Bgra8888,
                AlphaFormat.Premul);

            target.Source = _bitmap;
        }
        
        


        public unsafe void DrawPixel(Coords c, Color? color = null)
        {
            color ??= new Color(0, 0, 0);
            if (c.X < 0 || c.X >= Width || c.Y < 0 || c.Y >= Height) return;
            
            using var fb = _bitmap.Lock();

            byte* ptr = (byte*)fb.Address;

            int index = c.Y * fb.RowBytes + c.X * 4;

            ptr[index + 0] = color.B;
            ptr[index + 1] = color.G;
            ptr[index + 2] = color.R;
            ptr[index + 3] = color.Alpha;
            
        }

        public void DrawCircle(Coords c, int r, Color? color = null) => DravCircle(c, (double)r, color);
        
        public void DravCircle(Coords c , double r, Color? color = null)
        {
            color ??= new Color(0, 0, 0);
            
            
            double r2 = r * r;

            for (double i = c.X - r; i <= c.X + r; i++)
            {
                for (double n = c.Y - r; n <= c.Y + r; n++)
                {
                    double dx = c.X - i;
                    double dy = c.Y - n;

                    if (dx * dx + dy * dy <= r2) DrawPixel(((int)Math.Round(i,0), (int)Math.Round(n,0)), color); 
                }
            }
        }

        public void DravLine(Line line, bool constraintMode = false)
        {
            Coords first = line.Start;
            Coords second;
            if (constraintMode) second = line.ConsttraintEnd();
            else second = line.End;
            
            int width = line.Width;
            Color? color = line.Color;
            
            int pixelGap = 10;

            

            if (first.X == second.X)
            {
                if (second.Y > first.Y)
                {
                    for (int y = first.Y; y < second.Y; y += (width * pixelGap) / 2)
                        DravCircle((first.X, y), width, color);
                }
                else
                {
                    for (int y = second.Y; y < first.Y; y += (width * pixelGap) / 2)
                        DravCircle((first.X, y), width, color);
                }
                return;
            }

            double a = (double)(second.Y - first.Y) / (second.X - first.X);
            double b = second.Y - a * second.X;

            if (Math.Abs(first.X - second.X) > Math.Abs(first.Y - second.Y))
            {
                int xStart = Math.Min(first.X, second.X);
                int xEnd = Math.Max(first.X, second.X);

                for (int x = xStart; x <= xEnd; x += (width * pixelGap) / 2)
                {
                    int y = (int)Math.Round(a * x + b, 0);
                    DravCircle((x, y), width, color);
                }
            }
            else
            {
                int yStart = Math.Min(first.Y, second.Y);
                int yEnd = Math.Max(first.Y, second.Y);

                for (int y = yStart; y <= yEnd; y += (width * pixelGap) / 2)
                {
                    int x = (int)Math.Round((y - b) / a, 0);
                    DravCircle((x, y), width, color);
                }
            }
        }

        



        public void UpdateUI()
        {
            Dispatcher.UIThread.Post(() => _target.InvalidateVisual());
        }
        
        public unsafe void Resize(int width, int height)
        {
            if (width <= 0 || height <= 0)
                return;

            var newBitmap = new WriteableBitmap(
                new PixelSize(width, height),
                new Vector(96, 96),
                PixelFormat.Bgra8888,
                AlphaFormat.Premul);

            int minWidth = Math.Min(Width, width);
            int minHeight = Math.Min(Height, height);

            using var fbOld = _bitmap.Lock();
            using var fbNew = newBitmap.Lock();

            byte* oldPtr = (byte*)fbOld.Address;
            byte* newPtr = (byte*)fbNew.Address;

            for (int y = 0; y < minHeight; y++)
            {
                byte* oldRow = oldPtr + y * fbOld.RowBytes;
                byte* newRow = newPtr + y * fbNew.RowBytes;

                for (int x = 0; x < minWidth; x++)
                {
                    int i = x * 4;

                    newRow[i + 0] = oldRow[i + 0]; 
                    newRow[i + 1] = oldRow[i + 1];
                    newRow[i + 2] = oldRow[i + 2]; 
                    newRow[i + 3] = oldRow[i + 3]; 
                }
            }

            _bitmap = newBitmap;
            _target.Source = _bitmap;
            _target.InvalidateVisual();
        }


        public void Clear()
        {
            var newBitmap = new WriteableBitmap(
                new PixelSize(Width, Height),
                new Vector(96, 96),
                PixelFormat.Bgra8888,
                AlphaFormat.Premul);
            
            _target.Source = newBitmap;
            _bitmap = newBitmap;
        }


    }

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
    
    
    public class Line
    {
        public Coords Start;
        public Coords End;
        public int Width;
        public Color Color;

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
    
}
