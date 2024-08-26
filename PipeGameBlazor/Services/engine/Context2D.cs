using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Threading.Tasks;

public class Context2D
{
    private bool _won;
    private readonly Engine _engine;
    private readonly ElementReference _canvas;
    private double _stepX;
    private double _stepY;
    private readonly IJSRuntime _jsRuntime;

    public Context2D(ElementReference canvas, int rows, int columns, IJSRuntime jsRuntime)
    {
        _won = false;
        _canvas = canvas;
        _engine = new Engine(rows, columns);
        _jsRuntime = jsRuntime;
        _stepX = 50; // Default value
        _stepY = 50; // Default value
    }

    public async Task InitializeAsync()
    {
        // Initialize canvas size using JS interop
        var canvasSize = await _jsRuntime.InvokeAsync<CanvasSize>("getCanvasSize", _canvas);
        _stepX = (canvasSize.Width - 1) / _engine.Columns;
        _stepY = (canvasSize.Height - 1) / _engine.Rows;
    }

    public async Task DrawAsync()
    {
        // Use JS interop for drawing
        await _jsRuntime.InvokeVoidAsync("drawGridAndCells", _canvas, _engine.Map, _stepX, _stepY);
    }

    public async Task DrawWinAsync()
    {
        // Use JS interop for displaying win message
        await _jsRuntime.InvokeVoidAsync("drawWinMessage", _canvas);
    }

    public async Task StartAsync()
    {
        _won = false;
        _engine.Restart();
        await DrawAsync();
    }

    public async Task ClickAsync(int button, double x, double y, Func<Task> onWin)
    {
        if (_won) return;

        var rect = await _jsRuntime.InvokeAsync<BoundingClientRect>("getBoundingClientRect", _canvas);
        int i = (int)Math.Floor((x - rect.Left) / _stepX);
        int j = (int)Math.Floor((y - rect.Top) / _stepY);

        if (!_engine.Map[i, j].Locked)
        {
            if (button == 0)
            {
                _engine.RotateCellCCW(i, j);
            }
            else if (button == 2)
            {
                _engine.RotateCellCW(i, j);
            }
        }

        if (button == 1)
        {
            _engine.ToggleLock(i, j);
        }

        _engine.ResetConnections();
        await DrawAsync();

        if (_engine.CheckSolution())
        {
            _won = true;
            await DrawWinAsync();
            await onWin();
        }
    }
}

// Supporting classes for JS interop
public class CanvasSize
{
    public double Width { get; set; }
    public double Height { get; set; }
}

public class BoundingClientRect
{
    public double Left { get; set; }
    public double Top { get; set; }
    public double Right { get; set; }
    public double Bottom { get; set; }
    public double Width { get; set; }
    public double Height { get; set; }
}