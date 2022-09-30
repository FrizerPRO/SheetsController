namespace SheetsController;

[Serializable]
public class Reservation
{
    public Reservation(GameTable table, TimeSpan startTime, TimeSpan duration, string additionalInfo,
        int messageIdToDelete = -1)
    {
        Table = table;
        StartTime = startTime;
        Duration = duration;
        AdditionalInfo = additionalInfo;
        InProcess = true;
        Zone = table.Zone;
        MessageIdToDelete = messageIdToDelete;
    }

    public int MessageIdToDelete { get; set; }

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