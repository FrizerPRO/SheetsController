using Google.Apis.Sheets.v4.Data;
using SheetsController;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using File = System.IO.File;
using User = SheetsController.User;


public static class TelegramBotController
{
    private static readonly string FolderWithText = "../../../../ConsoleApp1/TextForMessages/";

    public static InlineKeyboardMarkup GetMainTab(User user)
    {
        List<IList<InlineKeyboardButton>> result = new();
        result.Add(new[]
        {
            InlineKeyboardButton.WithCallbackData(text: "Новая Запись", callbackData: "NewReservation"),
        });
        if (user.Reservations.Count > 0)
        {
            result.Add(new[]
            {
                InlineKeyboardButton.WithCallbackData(text: "Записи", callbackData: "Reservations"),
            });
        }

        return new InlineKeyboardMarkup(result);
    }

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
            InlineKeyboardButton.WithCallbackData(text: "Назад", callbackData: "Back"),
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
            InlineKeyboardButton.WithCallbackData(text: "Назад", callbackData: "Back"),
        });
        return new InlineKeyboardMarkup(result);
    }

    public static InlineKeyboardMarkup GetDurationTab()
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
            InlineKeyboardButton.WithCallbackData(text: "Назад", callbackData: "Back"),
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
            InlineKeyboardButton.WithCallbackData(text: "Назад", callbackData: "Back"),
        });
        return new InlineKeyboardMarkup(result);
    }

    public static InlineKeyboardMarkup GetIfNeedsAdditionalInfoTab()
    {
        List<IList<InlineKeyboardButton>> result = new();
        result.Add(new[]
        {
            InlineKeyboardButton.WithCallbackData(text: "Ввести дополнительную\nинформацию",
                callbackData: "AdditionalInfo"),
        });
        result.Add(new[]
        {
            InlineKeyboardButton.WithCallbackData(text: "Продолжить", callbackData: "Continue"),
        });
        result.Add(new[]
        {
            InlineKeyboardButton.WithCallbackData(text: "Назад", callbackData: "Back"),
        });
        return new InlineKeyboardMarkup(result);
    }

    public static InlineKeyboardMarkup GetConformationTab()
    {
        List<IList<InlineKeyboardButton>> result = new();
        result.Add(new[]
        {
            InlineKeyboardButton.WithCallbackData(text: "Подтвердить", callbackData: "Confirm"),
        });
        result.Add(new[]
        {
            InlineKeyboardButton.WithCallbackData(text: "Назад", callbackData: "Back"),
        });
        return new InlineKeyboardMarkup(result);
    }

    public static InlineKeyboardMarkup GetReservationInfoTab()
    {
        List<IList<InlineKeyboardButton>> result = new();
        result.Add(new[]
        {
            InlineKeyboardButton.WithCallbackData(text: "Отменить", callbackData: "Cancel"),
        });
        result.Add(new[]
        {
            InlineKeyboardButton.WithCallbackData(text: "Назад", callbackData: "Back"),
        });
        return new InlineKeyboardMarkup(result);
    }

    public static InlineKeyboardMarkup GetAllReservationsTab(User user)
    {
        List<IList<InlineKeyboardButton>> result = new();
        var counter = 0;
        foreach (var reservation in user.Reservations)
        {
            result.Add(new[]
            {
                InlineKeyboardButton.WithCallbackData(text: reservation.StartTime.ToString() +
                                                            "\n" + reservation.Table.Zone.Name,
                    callbackData: counter.ToString()),
            });
            counter++;
        }

        result.Add(new[]
        {
            InlineKeyboardButton.WithCallbackData(text: "Назад", callbackData: "Back"),
        });
        return new InlineKeyboardMarkup(result);
    }

    public static InlineKeyboardMarkup GetBackButtonTab()
    {
        List<IList<InlineKeyboardButton>> result = new();
        result.Add(new[]
        {
            InlineKeyboardButton.WithCallbackData(text: "Назад", callbackData: "Back"),
        });
        return new InlineKeyboardMarkup(result);
    }

    public static Message GetMainMessage(User user)
    {
        Message message = new()
        {
            ReplyMarkup = GetMainTab(user),
            Text = File.ReadAllText(FolderWithText + "MainMessageText.txt")
        };
        return message;
    }

    public static Message GetZoneMessage(List<PlayZone> zones)
    {
        Message message = new()
        {
            ReplyMarkup = GetZonesTab(zones),
            Text = File.ReadAllText(FolderWithText + "ZoneMessageText.txt")
        };
        return message;
    }

    public static Message GetTableNumberMessage(int amountOfTables)
    {
        Message message = new()
        {
            ReplyMarkup = GetTableNumber(amountOfTables),
            Text = File.ReadAllText(FolderWithText + "AmountOfTablesMessageText.txt")
        };
        return message;
    }

    public static Message GetDurationMessage()
    {
        Message message = new()
        {
            ReplyMarkup = GetDurationTab(),
            Text = File.ReadAllText(FolderWithText + "DurationMessageText.txt")
        };
        return message;
    }

    public static Message GetFreeTimeMessage(List<GameTable> gameTables)
    {
        Message message = new()
        {
            ReplyMarkup = GetFreeTimeTab(gameTables),
            Text = File.ReadAllText(FolderWithText + "FreeTimeMessageText.txt")
        };
        return message;
    }

    public static Message GetIfNeedAdditionalInfoMessage()
    {
        Message message = new()
        {
            ReplyMarkup = GetIfNeedsAdditionalInfoTab(),
            Text = File.ReadAllText(FolderWithText + "IfNeedAdditionalInfoMessageText.txt")
        };
        return message;
    }
    public static Message GetConformationMessage(Reservation reservation,User user)
    {
        Message message = new()
        {
            ReplyMarkup = GetConformationTab(),
            Text = GetReservationInfo(reservation,user)
        };
        return message;
    }

    private static string GetReservationInfo(Reservation reservation,User user)
    {
        string result = "Запись:\n" +
                        $"Начало: *{reservation.StartTime}*\n" +
                        $"Период: *{reservation.Duration}*\n" +
                        $"Зал: *{reservation.Table.Zone.Name}*\n" +
                        $"На имя: *{user.Nickname}*";
        return result;
    }
    public static Message GetAllReservationsMessage(User user)
    {
        Message message = new()
        {
            ReplyMarkup = GetAllReservationsTab(user),
            Text = File.ReadAllText(FolderWithText + "AllReservationsMessageText.txt")
        };
        return message;
    }
    public static Message GetReservationInfoMessage(Reservation reservation,User user)
    {
        Message message = new()
        {
            ReplyMarkup = GetReservationInfoTab(),
            Text = GetReservationInfo(reservation,user)
        };
        return message;
    }
    public static Message GetSetAdditionalInfoMessage(Reservation reservation,User user)
    {
        Message message = new()
        {
            ReplyMarkup = GetBackButtonTab(),
            Text = File.ReadAllText(FolderWithText + "SetAdditionalInfoMessageText.txt")
        };
        return message;
    }

}