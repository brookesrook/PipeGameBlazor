using System;
using System.Threading;

public class TimerService : IDisposable
{
    private Timer _timer;
    private DateTime _startTime;
    private string _timerText = "00:00";

    public string TimerText => _timerText;

    public TimerService()
    {
        _timer = new Timer(UpdateTimer, null, Timeout.Infinite, Timeout.Infinite);
    }

    public void Start()
    {
        _startTime = DateTime.Now;
        _timer.Change(0, 1000);
    }

    public void Stop()
    {
        _timer.Change(Timeout.Infinite, Timeout.Infinite);
    }

    private void UpdateTimer(object? state)
    {
        var timeElapsed = DateTime.Now - _startTime;
        _timerText = $"{timeElapsed.Minutes:D2}:{timeElapsed.Seconds:D2}";
    }

    public void Dispose()
    {
        _timer.Dispose();
    }
}