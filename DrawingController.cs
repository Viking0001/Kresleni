using Avalonia;
using Avalonia.Controls;
using System;
using System.Collections.Generic;
using Avalonia.Input;

namespace Kresleni;

public class DrawingController
{

    private readonly CanvasBitmap _canvas;
    private bool _drav;
    private List<Line> _lines = new List<Line>();
    private Line _focusLine;
    private int _widthLine;
    private bool _constraintMode;
    


    public DrawingController(CanvasBitmap canvas)
    {
        _canvas = canvas;
        _drav = false;
        _widthLine = 5;
        _constraintMode = false;
    }

    public void PointerPressed(object? sender, PointerPressedEventArgs e)
    {
        _drav = true;
        var start = (e.GetPosition(_canvas.Image).X, e.GetPosition(_canvas.Image).Y);
        _focusLine = new Line(start,start, _widthLine);
    }
    
    public void PointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        _drav = false;
        if (_constraintMode) _focusLine.End = _focusLine.ConsttraintEnd();
        _lines.Add(_focusLine);
    }
    
    public void PointerMoved(object? sender, PointerEventArgs e)
    {
        if (_drav)
        {
            RedrawAllLines();
            _focusLine.End = (e.GetPosition(_canvas.Image).X, e.GetPosition(_canvas.Image).Y);
            _canvas.DravLine(_focusLine, _constraintMode);
            _canvas.UpdateUI();
        }
    }

    public void RedrawAllLines()
    {
        _canvas.Clear();
        for (int i = 0; i < _lines.Count; i++)
        {
            _canvas.DravLine(_lines[i]);
        }
    }


    public void OnKeyDown(object? sender, KeyEventArgs e)
    {
        if (e.Key == Key.LeftShift || e.Key == Key.RightShift)
        {
            _constraintMode = true;
            RedrawAllLines();
            _canvas.DravLine(_focusLine, _constraintMode);
            _canvas.UpdateUI();
        }
    }

    public void OnKeyUp(object? sender, KeyEventArgs e)
    {
        if (e.Key == Key.LeftShift || e.Key == Key.RightShift)
        {
            _constraintMode = false;
            RedrawAllLines();
            _canvas.DravLine(_focusLine, _constraintMode);
            _canvas.UpdateUI();  
        }
    }
}