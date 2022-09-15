using Google.Apis.Sheets.v4.Data;
using SheetsController;
using Telegram.Bot.Types.ReplyMarkups;


public static class TelegramBotController
{
    public static InlineKeyboardMarkup GetZonesTab(List<PlayZone> zones)
    {
        List<IList<InlineKeyboardButton>> result = new();
        var counter = 0;
        foreach (var zone in zones)
        {
            result.Add(new[]
            {
                InlineKeyboardButton.WithCallbackData(text: zone.Name, callbackData: counter.ToString()),
            });
            counter++;
        }

        result.Add(new[]
        {
            InlineKeyboardButton.WithCallbackData(text: "Назад", callbackData: "-2"),
        });
        return new InlineKeyboardMarkup(result);
    }

    public static InlineKeyboardMarkup GetTableNumber(int amountOfTables)
    {
        List<IList<InlineKeyboardButton>> result = new();
        result.Add(new[]
        {
            InlineKeyboardButton.WithCallbackData(text: "Любой", callbackData: "-1"),
        });
        for (var i = 1; i <= amountOfTables;)
        {
            List<InlineKeyboardButton> tableNumbers = new();
            for (var j = 0; j < 3 && i <= amountOfTables; j++, i++)
            {
                tableNumbers.Add(InlineKeyboardButton.WithCallbackData(text: i.ToString(), callbackData: i.ToString()));
            }

            result.Add(tableNumbers);
        }

        result.Add(new[]
        {
            InlineKeyboardButton.WithCallbackData(text: "Назад", callbackData: "-2"),
        });
        return new InlineKeyboardMarkup(result);
    }

    public static InlineKeyboardMarkup GetDurationTab(int amountOfTables)
    {
        List<IList<InlineKeyboardButton>> result = new();
        for (var i = TimeSpan.Zero; i <= SheetsController.SheetsController.MaxReservationDuration;)
        {
            List<InlineKeyboardButton> tableNumbers = new();
            for (var j = 0;
                 j < 3 && i <= SheetsController.SheetsController.MaxReservationDuration;
                 j++, i += SheetsController.SheetsController.MinimumStep)
            {
                tableNumbers.Add(InlineKeyboardButton.WithCallbackData(text: i.ToString(), callbackData: i.ToString()));
            }

            result.Add(tableNumbers);
        }

        result.Add(new[]
        {
            InlineKeyboardButton.WithCallbackData(text: "Назад", callbackData: "-2"),
        });
        return new InlineKeyboardMarkup(result);
    }

    public static InlineKeyboardMarkup GetFreeTimeTab(List<GameTable> gameTables)
    {
        List<IList<InlineKeyboardButton>> result = new();
        var allFreeTime = SheetsController.SheetsController.GetAllFreeTimeSpans(gameTables);
        for (var i = 0; i < allFreeTime.Count;)
        {
            List<InlineKeyboardButton> tableNumbers = new();
            for (var j = 0; j < 3 && i < allFreeTime.Count; j++, i++)
            {
                tableNumbers.Add(InlineKeyboardButton.WithCallbackData(text: allFreeTime[i].ToString(),
                    callbackData: allFreeTime[i].ToString()));
            }

            result.Add(tableNumbers);
        }

        result.Add(new[]
        {
            InlineKeyboardButton.WithCallbackData(text: "Назад", callbackData: "-2"),
        });
        return new InlineKeyboardMarkup(result);
    }

    public static InlineKeyboardMarkup GetAdditionalInfoTab()
    {
        List<IList<InlineKeyboardButton>> result = new();
        result.Add(new[]
        {
            InlineKeyboardButton.WithCallbackData(text: "Ввести дополнительную\nинформацию", callbackData: "0"),
        });
        result.Add(new[]
        {
            InlineKeyboardButton.WithCallbackData(text: "Продолжить", callbackData: "1"),
        });
        result.Add(new[]
        {
            InlineKeyboardButton.WithCallbackData(text: "Назад", callbackData: "-2"),
        });
        return new InlineKeyboardMarkup(result);
    }
    public static InlineKeyboardMarkup GetBackButtonTab()
    {
        List<IList<InlineKeyboardButton>> result = new();
        result.Add(new[]
        {
            InlineKeyboardButton.WithCallbackData(text: "Назад", callbackData: "-2"),
        });
        return new InlineKeyboardMarkup(result);
    }

}