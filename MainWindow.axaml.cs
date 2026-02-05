using Avalonia;
using Avalonia.Controls;
using System;
using System.Threading.Tasks;
using Avalonia.Input;


namespace Kresleni;


public partial class MainWindow : Window
{
    public CanvasBitmap Canvas;
    public DrawingController Controller;

    public MainWindow()
    {
        InitializeComponent();
        Opened += OnOpened;
    }
    
    
    
    private void OnWindowSizeChanged(object? sender, SizeChangedEventArgs e)
    {
        Img.Width = e.NewSize.Width;
        Img.Height = e.NewSize.Height;
        Canvas.Resize((int)e.NewSize.Width, (int)e.NewSize.Height);
    }
    
    
    private async void OnOpened(object? sender, EventArgs e)
    {
        await Task.Delay(100);
        Canvas = new CanvasBitmap(Img, (int)Bounds.Width, (int)Bounds.Height);
        Controller = new DrawingController(Canvas);
        
        Img.PointerPressed += Controller.PointerPressed;
        Img.PointerMoved += Controller.PointerMoved;
        Img.PointerReleased += Controller.PointerReleased;
        
        SizeChanged += OnWindowSizeChanged;
        KeyDown += Controller.OnKeyDown;
        KeyUp += Controller.OnKeyUp;

    }
    
}