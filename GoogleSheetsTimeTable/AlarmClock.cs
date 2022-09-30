using System.Timers;
using Timer = System.Timers.Timer;

namespace SheetsController;

public class AlarmClock
{
    private readonly DateTime alarmTime;
    private readonly Timer timer;
    private EventHandler alarmEvent;
    private bool enabled;

    public AlarmClock(DateTime alarmTime)
    {
        this.alarmTime = alarmTime;

        timer = new Timer();
        timer.Elapsed += timer_Elapsed;
        timer.Interval = 1000;
        timer.Start();

        enabled = true;
    }

    private void timer_Elapsed(object sender, ElapsedEventArgs e)
    {
        if (enabled && SheetsController.Now > alarmTime)
        {
            enabled = false;
            OnAlarm();
            timer.Stop();
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