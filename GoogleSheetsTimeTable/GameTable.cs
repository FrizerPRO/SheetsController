namespace SheetsController;
[Serializable]

public class GameTable
{
    public List<TimeSpan> FreeTime
    {
        get;
        set;
    }
    public int Number
    {
        get;
        set;
    }
    public Dictionary<TimeSpan, string> TimeTable
    {
        get;
        set;
    }
    public PlayZone Zone
    {
        get;
        set;
    }

    public GameTable()
    {
        TimeTable = new Dictionary<TimeSpan, string>();
        FreeTime = new List<TimeSpan>();
        Zone = new PlayZone();
    }
    public GameTable(PlayZone zone, int number)
    {
        Zone = zone;
        Number = number;
        TimeTable = new Dictionary<TimeSpan, string>();
        FreeTime = new List<TimeSpan>();
        
    }

    public string Range
    {
        get
        {
            var range = Zone.Name + "!";
            range +=
                $"{SheetsController.GetColumnName(Number)}2:" +
                $"{SheetsController.GetColumnName(Number)}" +
                $"{SheetsController.TotalRows}";
            return range;
        }
    }
}