using Avalonia;
using Avalonia.Controls;
using System;
using System.Collections.Generic;
using Avalonia.Input;
using Avalonia.Interactivity;

namespace Kresleni;

public class DrawingController
{

    private readonly CanvasBitmap _canvas;
    private bool _drav;
    private List<Line> _lines = new List<Line>();
    private Line _focusLine;
    private int _widthLine;

                    
    public bool _constraintMode = false;
    public bool _dashedMode = false;
    


    public DrawingController(CanvasBitmap canvas)
    {
        _canvas = canvas;
        _drav = false;
        _widthLine = 5;
    }

    public void PointerPressed(object? sender, PointerPressedEventArgs e)
    {
        _drav = true;
        var start = (e.GetPosition(_canvas.Image).X, e.GetPosition(_canvas.Image).Y);
        _focusLine = new Line(start,start, _widthLine);
        _focusLine.DashedMode = _dashedMode;
        _focusLine.ConstraintMode = _constraintMode;
    }
    
    public void PointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        _drav = false;
        _lines.Add(_focusLine);
    }
    
    public void PointerMoved(object? sender, PointerEventArgs e)
    {
        if (_drav)
        {
            RedrawAllLines();
            _focusLine.End = (e.GetPosition(_canvas.Image).X, e.GetPosition(_canvas.Image).Y);
            _canvas.DravLine(_focusLine);
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

    public void Clear()
    {
        _canvas.Clear();
        _canvas.UpdateUI();
        _lines.Clear();
    }


    public void OnKeyDown(object? sender, KeyEventArgs e)
    {
        if (e.Key == Key.LeftShift || e.Key == Key.RightShift)
        {
            _constraintMode = true;
            if (_drav)
            {
                _focusLine.ConstraintMode = true;
                RedrawAllLines();
                _canvas.DravLine(_focusLine);
                _canvas.UpdateUI();
            }
        }
        
        if (e.Key == Key.LeftCtrl || e.Key == Key.RightCtrl)
        {
            _dashedMode = true;
            if (_drav)
            {
                _focusLine.DashedMode = true;
                RedrawAllLines();
                _canvas.DravLine(_focusLine);
                _canvas.UpdateUI();
            }
        }
        

        if (e.Key == Key.C)
        {
            Clear();
        }
    }

    public void OnKeyUp(object? sender, KeyEventArgs e)
    {
        if (e.Key == Key.LeftShift || e.Key == Key.RightShift)
        {
            _constraintMode = false;
            if (_drav)
            {
                _focusLine.ConstraintMode = false;
                RedrawAllLines();
                _canvas.DravLine(_focusLine);
                _canvas.UpdateUI();
            }
        }
        
        if (e.Key == Key.LeftCtrl || e.Key == Key.RightCtrl)
        {
            _dashedMode = false;
            if (_drav)
            {
                _focusLine.DashedMode = false;
                RedrawAllLines();
                _canvas.DravLine(_focusLine);
                _canvas.UpdateUI();         
            }
        }
        
    }


}