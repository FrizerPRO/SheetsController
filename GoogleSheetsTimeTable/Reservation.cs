namespace SheetsController;

[Serializable]
public class Reservation
{
    public Reservation(GameTable table, TimeSpan startTime, TimeSpan duration, string additionalInfo)
    {
        Table = table;
        StartTime = startTime;
        Duration = duration;
        AdditionalInfo = additionalInfo;
        InProcess = true;
        Zone = table.Zone;
    }

    public bool InProcess { get; set; }

    public PlayZone Zone { get; set; }

    public GameTable Table { get; set; }

    public string AdditionalInfo { get; set; }

    public TimeSpan StartTime { get; set; }
    public TimeSpan Duration { get; set; }

    public override bool Equals(object? obj)
    {
        var res = (Reservation)obj!;
        return res.StartTime == StartTime &&
               res.Duration == Duration &&
               res.Table.Number == Table.Number;
    }
}