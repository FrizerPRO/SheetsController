using Google.Apis.Sheets.v4.Data;

namespace SheetsController;

public class PlayZone
{
    private readonly Sheet _sheet;

    public PlayZone(string name, int capacity)
    {
        Name = name;
        Capacity = capacity;
        var spr = SheetsController.Service.Spreadsheets.Get(DataBase.SpreadSheetId).Execute();
        _sheet = spr.Sheets.FirstOrDefault(s => s.Properties.Title == Name)!;
    }

    public PlayZone()
    {
    }

    public string Name { get; set; }
    public int Capacity { get; set; }

    public async Task RefreshInMidnight()
    {
        var rowToRenewFrom = (SheetsController.TotalRows + 1) / 2 + 1;
        var range = Name + "!";
        range +=
            $"{SheetsController.GetColumnName(2)}{rowToRenewFrom}:" +
            $"{SheetsController.GetColumnName(Capacity + 1)}" +
            $"{SheetsController.TotalRows}";
        var valuesToUpdate = await SheetsController.GetValuesFromRange(range, DataBase.SpreadSheetId)
                             ?? new List<IList<object>>();
        await Refresh();
        var turnedValues = TurnValues(valuesToUpdate, Capacity, rowToRenewFrom);

        for (var i = 0; i < Capacity; i++)
        for (var j = 0; j < rowToRenewFrom; j++)
        {
            var duration = 0;
            while (j < rowToRenewFrom && turnedValues[i][j].ToString() != string.Empty && (duration == 0 ||
                       turnedValues[i][j - 1].ToString() == turnedValues[i][j].ToString()))
            {
                duration++;
                j++;
            }

            if (duration <= 0) continue;
            var allValues = turnedValues[i][j - 1].ToString()!
                .Split("::->", StringSplitOptions.RemoveEmptyEntries);
            var nickname = allValues[0];
            var startTime = SheetsController.TimeToRowNumber.FirstOrDefault(
                item => item.Value == j - duration + 2).Key;
            var durationInTime = SheetsController.TimeToRowNumber.FirstOrDefault(
                item => item.Value == duration + 2).Key;
            var values = SheetsController.GetFreeTables(this, durationInTime,
                i + 1);

            var additionalInfo = "";
            if (allValues.Length > 1)
                additionalInfo = allValues[1];
            var t = await UserControl.AddReservation(new User(nickname), await values, startTime,
                durationInTime, additionalInfo, i + 1);
            Console.WriteLine(nickname);
            j--;
        }
    }

    private IList<IList<object>> TurnValues(IList<IList<object>> valuesToUpdate, int capacity, int rowToRenewFrom)
    {
        var res = new List<IList<object>>();
        res.AddRange(Enumerable.Repeat(new List<object>(), capacity));
        for (var i = 0; i < capacity; i++)
        {
            res[i] = new List<object>();
            for (var j = 0; j < rowToRenewFrom; j++) res[i].Add(string.Empty);
        }

        for (var i = 0; i < rowToRenewFrom; i++)
        for (var j = 0; j < Capacity; j++)
            if (valuesToUpdate.Count > i &&
                valuesToUpdate[i].Count > j &&
                valuesToUpdate[i][j].ToString() != string.Empty)
            {
                res[j][i] = valuesToUpdate[i][j];
                Console.WriteLine(valuesToUpdate[i][j] + " i: " + i + " j: " + j);
            }

        return res;
    }

    public async Task Refresh()
    {
        var requests = new List<Request>();
        for (var i = 1; i <= Capacity + 1; i++)
        for (var j = 1; j <= SheetsController.TotalRows; j++)
        {
            var range = SheetsController.GetRangeForSingleCell(i, j, _sheet);
            var text = GetCellTextForRefreshing(i - 1, j);
            var cell = SheetsController.CreateCell(text, new CellFormat());
            requests.Add(await SheetsController.GetRequestToFill(range.Result, cell.Result));
        }

        await SheetsController.FillCells(requests, DataBase.SpreadSheetId);
    }

    private static string GetCellTextForRefreshing(int i, int j)
    {
        var text = string.Empty;
        if (i == 0 && j == 1)
        {
        }
        else if (i == 0)
        {
            text = SheetsController.TimeToRowNumber.FirstOrDefault(
                item => item.Value == j).Key.ToString();
        }
        else if (j == 1)
        {
            text = i.ToString();
        }

        return text;
    }

    public async Task CancelReservation(Reservation reservation)
    {
        var requests = new List<Request>();
        var range = SheetsController.GetRangeForFillingTables(reservation.Table, reservation.StartTime,
            reservation.Duration);
        var cell = SheetsController.CreateCell(string.Empty, new CellFormat());
        requests.Add(await SheetsController.GetRequestToFill(range.Result, cell.Result));
        await SheetsController.FillCells(requests, DataBase.SpreadSheetId);
    }
}