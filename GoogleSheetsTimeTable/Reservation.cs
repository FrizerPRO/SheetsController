namespace SheetsController;

[Serializable]
public class Reservation
{
    public bool InProcess
    {
        get;
        set;
    }

    public PlayZone Zone
    {
        get;
        set;
    }
    public GameTable Table { get; set; }

    public string AdditionalInfo
    {
        get;
        set;
    }

    public int TableNumber
    {
        get;
        set;
    }
    public TimeSpan StartTime { get; set; }
    public TimeSpan Duration { get; set; }

    public override bool Equals(object? obj)
    {
        var res = (Reservation)obj!;
        return res.StartTime == StartTime &&
               res.Duration == Duration &&
               res.Table.Number == Table.Number;
    }

    

    public Reservation(GameTable table, TimeSpan startTime, TimeSpan duration,string additionalInfo)
    {
        this.Table = table;
        StartTime = startTime;
        this.Duration = duration;
        AdditionalInfo = additionalInfo;
        InProcess = true;
        Zone = table.Zone;
    }
}