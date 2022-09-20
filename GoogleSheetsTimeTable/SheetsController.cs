using System.Data;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;

namespace SheetsController;
[Serializable]
public class SheetsController
{
    public static readonly TimeSpan MinimumStep = TimeSpan.FromMinutes(30);

    private static readonly TimeSpan TotalDuration = TimeSpan.FromDays(2) - MinimumStep;
    public static readonly TimeSpan MaxReservationDuration = TimeSpan.FromHours(5);

    private static readonly string ApplicationName = "DICK";

    protected internal static readonly SheetsService Service;
    private static readonly string[] Scopes = { SheetsService.Scope.Spreadsheets };

    //public static string SpreadsheetId => DataBase.SpreadSheetId;


    static SheetsController()
    {
        Service = new SheetsService(new BaseClientService.Initializer
        {
            HttpClientInitializer = GetCredential(),
            ApplicationName = ApplicationName
        });
    }
    

    protected internal static int TotalRows
    {
        get
        {
            var counter = 0;
            var time = TotalDuration + MinimumStep;
            while (time >= TimeSpan.Zero)
            {
                time -= MinimumStep;
                counter++;
            }

            return counter;
        }
    }

    protected internal static Dictionary<TimeSpan, int> TimeToRowNumber
    {
        get
        {
            Dictionary<TimeSpan, int> result = new();
            var counter = 2;
            for (var i = TimeSpan.Zero; i <= TotalDuration; i += MinimumStep) result[i] = counter++;
            return result;
        }
    }

    public void Test()
    {
        PlayZone zone = new("Comfort", 9, this);
        //zone.RefreshInMidnight();
        //zone.Refresh();
        var duration = TimeSpan.FromHours(1);
        var tableNumber = 5;
        var values = GetFreeTables(zone, duration, tableNumber);

        var startTime = TimeSpan.FromHours(11);
        var nickname = "Peter";
        User user = new User(nickname);
        //UserControl.AddReservation(user,values,startTime,duration,tableNumber);
        user = UserControl.DeserializeUser(user);
        //UserControl.AddReservation(user, values, startTime, duration, tableNumber);
        Console.WriteLine(user.Reservations.Count);
        //UserControl.RemoveReservation(user,user.Reservations[0]);
        zone.RefreshInMidnight();
    }

    public Reservation? TrySetFreeTime(List<GameTable> values, TimeSpan startTime, string nickname,
        TimeSpan duration, PlayZone zone,string additionalInfo, int tableNumber = -1)
    {
        var valuesToCompare = GetFreeTables(zone, duration, tableNumber);
        if (valuesToCompare.Find(table => table.FreeTime.Contains(startTime)) == null)
            return null;
        return SetFreeTime(values, startTime, nickname, duration,additionalInfo);
    }

    public static List<GameTable> GetFreeTables(PlayZone zone, TimeSpan duration, int tableNumber = -1)
    {
        var values = GetTable(GetRange(zone, tableNumber));
        values = ChangeTimeForFree(values, duration);
        CheckForTimeCapableToNow(values);
        return values;
    }

    private static void CheckForTimeCapableToNow(List<GameTable> values)
    {
        values.ForEach((table) =>
        {
            var resTable = new List<TimeSpan>();
            table.FreeTime.ForEach((time) =>
            {
                if (time >= DateTime.Now - DateTime.Now.Date)
                    resTable.Add(time);
            });
            table.FreeTime = resTable;
        });
    }

    public static List<TimeSpan> GetAllFreeTimeSpans(List<GameTable> gameTables)
    {
        List<TimeSpan> result = new();
        foreach (var table in gameTables)
        {
            foreach (var freeTime in table.FreeTime)
            {
                if (!result.Contains(freeTime))
                {
                    result.Add(freeTime);
                }
            }
        }
        result.Sort();
        return result;
    }
    private static UserCredential GetCredential()
    {
        using var stream =
            new FileStream("../../../../ConsoleApp1/Credential/credentials.json", FileMode.Open, FileAccess.Read);
        /* The file token.json stores the user's access and refresh tokens, and is created
             automatically when the authorization flow completes for the first time. */
        return GoogleWebAuthorizationBroker.AuthorizeAsync(
            GoogleClientSecrets.FromStream(stream).Secrets,
            Scopes,
            "user",
            CancellationToken.None).Result;
    }

    private Reservation? SetFreeTime(List<GameTable> tables, TimeSpan userTime, string nickname, TimeSpan duration,string additionalInfo)
    {
        GameTable? resultTable = null;
        foreach (var table in tables.Where(table => table.FreeTime.Contains(userTime))) resultTable = table;
        if (resultTable == null)
            return null;
        var range = GetRangeForFillingTables(resultTable, userTime, duration);
        var cell = CreateCell(additionalInfo==""? nickname:
            nickname+"::->"+additionalInfo, new CellFormat
        {
            BackgroundColor = new Color
            {
                Red = 1,
                Blue = 0,
                Green = 0,
                Alpha = 1
            },
            TextFormat = new TextFormat
            {
                Bold = true
            }
        });
        Console.WriteLine(cell.ToString());
        var requests = new List<Request>
        {
            GetRequestToFill(range, cell)
        };
        FillCells(requests, DataBase.SpreadSheetId);
        
        return new Reservation(resultTable,userTime,duration,additionalInfo);
    }

    protected internal static void FillCells( /*Data.GridRange range, Data.CellData cell,string spreadSheetId*/
        List<Request> requests, string spreadSheetId)
    {
        if(requests.Count == 0)
            return;
        var request = new BatchUpdateSpreadsheetRequest
        {
            Requests = requests
        };
        var bur = Service.Spreadsheets.BatchUpdate(request, spreadSheetId);
        bur.Execute();
    }

    protected internal static Request GetRequestToFill(GridRange range, CellData cell)
    {
        var updateCellsRequest = new Request
        {
            RepeatCell = new RepeatCellRequest
            {
                Range = range,
                Cell = cell,
                Fields = "*"
            }
        };
        return updateCellsRequest;
    }

    protected internal static CellData CreateCell(string text,
        CellFormat cellFormat)
    {
        var userEnteredFormat = cellFormat;
        var cell = new CellData
        {
            UserEnteredFormat = userEnteredFormat,
            UserEnteredValue = new ExtendedValue
            {
                StringValue = text
            }
        };
        return cell;
    }

    protected internal GridRange GetRangeForFillingTables(GameTable table, TimeSpan startTime, TimeSpan duration)
    {
        var spr = Service.Spreadsheets.Get(DataBase.SpreadSheetId).Execute();
        var sh = spr.Sheets.FirstOrDefault(s => s.Properties.Title ==
                                                table.Zone.Name);
        var sheetId = (int)(sh!).Properties.SheetId!;
        
        return new GridRange
        {
            SheetId = sheetId,
            StartColumnIndex = table.Number - 1,
            EndColumnIndex = table.Number,
            StartRowIndex = TimeToRowNumber[startTime] - 1,
            EndRowIndex = TimeToRowNumber[startTime + duration - MinimumStep]
        };
        
    }

    public static GridRange GetRangeForSingleCell(int startColumn, int startRow, Sheet sheet)
    {
        var sheetId = (int)sheet.Properties.SheetId!;
        return new GridRange
        {
            SheetId = sheetId,
            StartColumnIndex = startColumn - 1,
            EndColumnIndex = startColumn ,
            StartRowIndex = startRow - 1,
            EndRowIndex = startRow
        };
    }

    private static List<GameTable> GetRange(PlayZone zone, int tableNumber = -1)
    {
        List<GameTable> tables = new();

        if (tableNumber == -1)
        {
            for (var i = 2; i <= zone.Capacity + 1; i++) tables.Add(new GameTable(zone, i));

            return tables;
        }

        tableNumber += 1;
        tables.Add(
            new GameTable(zone, tableNumber));
        return tables;
    }

    private static List<GameTable> ChangeTimeForFree(List<GameTable> tables, TimeSpan duration)
    {
        foreach (var table in tables)
        {
            var tableArray = table.TimeTable.ToArray();
            if (table.TimeTable.Count == 0)
            {
                var freeTime = TotalDuration + MinimumStep;
                table.FreeTime.AddRange(GetAllFreeCells(freeTime,
                    duration, TotalDuration + MinimumStep));
            }
            else
            {
                if (tableArray[0].Key >= duration)
                    table.FreeTime.AddRange(GetAllFreeCells(tableArray[0].Key, duration,
                        tableArray[0].Key));
                for (var i = 1; i < tableArray.Length; i++)
                {
                    var localDuration = tableArray[i].Key - tableArray[i - 1].Key - MinimumStep;
                    if (localDuration >= duration)
                        table.FreeTime.AddRange(GetAllFreeCells(localDuration, duration,
                            tableArray[i].Key));
                }

                if (TotalDuration - tableArray[^1].Key >= duration)
                    table.FreeTime.AddRange(GetAllFreeCells(TotalDuration - tableArray[^1].Key,
                        duration, TotalDuration + MinimumStep));
            }

            table.FreeTime.Sort();
        }

        return tables;
    }

    private static List<TimeSpan> GetAllFreeCells(TimeSpan maxDuration, TimeSpan duration, TimeSpan endTime)
    {
        var localTime = endTime - duration;
        List<TimeSpan> result = new();

        while (localTime >= endTime - maxDuration)
        {
            result.Add(localTime);
            localTime -= MinimumStep;
        }

        return result;
    }

    private static List<GameTable> GetTable(List<GameTable> rangeOfTables)
    {
        foreach (var table in rangeOfTables)
        {
            var values = GetValuesFromRange(table.Range,DataBase.SpreadSheetId);
            TimeSpan counter = new();
            if (values == null)
                return rangeOfTables;

            foreach (var timeColumn in values)
            {
                for (var i = 0; i < timeColumn.Count; i++)
                    if (timeColumn[i]!=new Object())
                        table.TimeTable[counter] = timeColumn[i].ToString()!;
                counter += MinimumStep;
            }
        }

        return rangeOfTables;
    }
    protected internal static IList<IList<object>>? GetValuesFromRange(string range, string spreadsheetId)
    {
        
        var request =
            Service.Spreadsheets.Values.Get(spreadsheetId, range);
        var response = request.Execute();
        
        IList<IList<object>> values = response.Values;
        return values;
    }

    public static string GetColumnName(int columnNumber)
    {
        var columnName = "";
        while (columnNumber > 0)
        {
            var modulo = (columnNumber - 1) % 26;
            columnName = Convert.ToChar('A' + modulo) + columnName;
            columnNumber = (columnNumber - modulo) / 26;
        }

        return columnName;
    }
}