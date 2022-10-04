using System.Timers;
using Timer = System.Timers.Timer;

namespace SheetsController;

public class AlarmClock
{
    private readonly DateTime alarmTime;
    private readonly Timer timer;
    private EventHandler alarmEvent;
    private readonly bool enabled;

    public AlarmClock(DateTime alarmTime)
    {
        this.alarmTime = alarmTime;

        timer = new Timer();
        timer.Elapsed += timer_Elapsed;
        timer.Interval = TimeSpan.FromMinutes(1).TotalMilliseconds;
        timer.Start();
        enabled = true;
        // timer.AutoReset = true;
    }

    private void timer_Elapsed(object sender, ElapsedEventArgs e)
    {
        if (enabled && SheetsController.Now > alarmTime)
        {
            timer.Interval = TimeSpan.FromDays(1).TotalMilliseconds;
            OnAlarm();
            Task.WaitAll();
        }
    }

    protected virtual void OnAlarm()
    {
        if (alarmEvent != null)
            alarmEvent(this, EventArgs.Empty);
    }


    public event EventHandler Alarm
    {
        add => alarmEvent += value;
        remove => alarmEvent -= value;
    }
}