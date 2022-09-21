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

    public Dictionary<TimeSpan, string> GetTimeTable()
    {
        Dictionary<TimeSpan, string> result = new();
        foreach (var i in TimeTableString)
        {
            result[TimeSpan.Parse(i.Key)] = i.Value;
        }

        return result;
    }

    public void SetTimeTable(Dictionary<TimeSpan, string> value)
    {
        foreach (var i in value)
        {
            TimeTableString[i.Key.ToString()] = i.Value;
        }
    }

    public Dictionary<string, string> TimeTableString
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
        TimeTableString = new Dictionary<string, string>();
        FreeTime = new List<TimeSpan>();
        Zone = new PlayZone();
    }
    public GameTable(PlayZone zone, int number)
    {
        Zone = zone;
        Number = number;
        FreeTime = new List<TimeSpan>();
        TimeTableString = new Dictionary<string, string>();

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