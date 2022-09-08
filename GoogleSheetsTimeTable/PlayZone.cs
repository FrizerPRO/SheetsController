﻿using Google.Apis.Sheets.v4.Data;

namespace SheetsController;

public class PlayZone
{
    public SheetsController SheetsController { get; set; }

    public PlayZone(string name, int capacity, SheetsController sheetsController)
    {
        Name = name;
        Capacity = capacity;
        SheetsController = sheetsController;
        var spr = SheetsController.Service.Spreadsheets.Get(SheetsController.SpreadsheetId).Execute();
        _sheet = spr.Sheets.FirstOrDefault(s => s.Properties.Title == Name)!;
    }

    public string Name { get; set; }
    public int Capacity { get; set; }

    private readonly Sheet _sheet;

    public PlayZone()
    {
    }
    public void RefreshInMidnight()
    {
        int rowToRenewFrom = (SheetsController.TotalRows + 1) / 2 + 1;
        var range = Name + "!";
        range +=
            $"{SheetsController.GetColumnName(2)}{rowToRenewFrom}:" +
            $"{SheetsController.GetColumnName(Capacity + 1)}" +
            $"{SheetsController.TotalRows}";
        var valuesToUpdate = SheetsController.GetValuesFromRange(range, SheetsController.SpreadsheetId)
                             ?? new List<IList<object>>();
        var requests = new List<Request>();
        Refresh();
        var turnedValues = TurnValues(valuesToUpdate,Capacity,rowToRenewFrom);
        for (var i = 0; i < Capacity; i++)
        {
            for (var j = 0; j < rowToRenewFrom; j++)
            {
                int duration = 0;
                while (j < rowToRenewFrom && turnedValues[i][j].ToString() != string.Empty)
                {
                    duration++;
                    j++;
                }
                
                if (duration <= 0) continue;
                TimeSpan startTime = SheetsController.TimeToRowNumber.FirstOrDefault(
                    item => item.Value == j - duration + 2).Key;
                TimeSpan durationInTime = SheetsController.TimeToRowNumber.FirstOrDefault(
                    item => item.Value == duration + 2).Key;
                Console.WriteLine();
                var values = SheetsController.GetFreeTables(this,durationInTime,
                    i + 1);
                UserControl.AddReservation(new User(turnedValues[i][j-1].ToString()!), values,startTime,
                    durationInTime, i + 1);
            }
        }

        SheetsController.FillCells(requests, SheetsController.SpreadsheetId);
    }
    

    private IList<IList<object>> TurnValues(IList<IList<object>> valuesToUpdate,int capacity,int rowToRenewFrom)
    {
        var res = new List<IList<object>>();
        res.AddRange(Enumerable.Repeat(new List<object>(),capacity));
        for (int i = 0; i < capacity; i++)
        {
            res[i] = new List<object>();
            for (int j = 0; j < rowToRenewFrom; j++)
            {
                res[i].Add( (object)string.Empty);
            }
        }

        for (var i = 0; i < rowToRenewFrom; i++)
        {
            for (var j = 0; j < Capacity; j++)
            {
                if ((valuesToUpdate.Count > i) &&
                    (valuesToUpdate[i].Count > j) && 
                    (valuesToUpdate[i][j].ToString() != string.Empty))
                {
                    res[j][i] = valuesToUpdate[i][j];
                    Console.WriteLine(valuesToUpdate[i][j].ToString() + " i: " + i+" j: " + j);
                }
            }
        }
        return res;
    }

    public void Refresh()
    {
        var requests = new List<Request>();
        for (var i = 1; i <= Capacity + 1; i++)
        for (var j = 1; j <= SheetsController.TotalRows; j++)
        {
            var range = SheetsController.GetRangeForSingleCell(i, j, _sheet);
            var text = GetCellTextForRefreshing(i - 1, j);
            var cell = SheetsController.CreateCell(text, new CellFormat());
            requests.Add(SheetsController.GetRequestToFill(range, cell));
        }

        SheetsController.FillCells(requests, SheetsController.SpreadsheetId);
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

    public void CancelReservation(Reservation reservation)
    {
        var requests = new List<Request>();
        var range = SheetsController.GetRangeForFillingTables(reservation.Table, reservation.StartTime,
            reservation.Duration);
        var cell = SheetsController.CreateCell(string.Empty, new CellFormat());
        requests.Add(SheetsController.GetRequestToFill(range, cell));
        SheetsController.FillCells(requests, SheetsController.SpreadsheetId);
    }
}