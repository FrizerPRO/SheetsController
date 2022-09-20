using System.Net.Mime;
using Google.Apis.Sheets.v4.Data;
using SheetsController;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
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
                InlineKeyboardButton.WithCallbackData(text: zone.Name, callbackData: $"ZoneNumber {counter.ToString()}"),
            });
            counter++;
        }

        result.Add(new[]
        {
            InlineKeyboardButton.WithCallbackData(text: "Назад", callbackData: "Main"),
        });
        return new InlineKeyboardMarkup(result);
    }

    public static InlineKeyboardMarkup GetTableNumber(int amountOfTables)
    {
        List<IList<InlineKeyboardButton>> result = new();
        result.Add(new[]
        {
            InlineKeyboardButton.WithCallbackData(text: "Любой", callbackData: "TableNumber -1"),
        });
        for (var i = 1; i <= amountOfTables;)
        {
            List<InlineKeyboardButton> tableNumbers = new();
            for (var j = 0; j < 3 && i <= amountOfTables; j++, i++)
            {
                tableNumbers.Add(InlineKeyboardButton.WithCallbackData(text: i.ToString(), $"TableNumber {i}"));
            }

            result.Add(tableNumbers);
        }

        result.Add(new[]
        {
            InlineKeyboardButton.WithCallbackData(text: "Назад", callbackData: "PlayZone"),
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
                tableNumbers.Add(InlineKeyboardButton.WithCallbackData(text: i.ToString(), callbackData: $"Duration {i.ToString()}"));
            }

            result.Add(tableNumbers);
        }

        result.Add(new[]
        {
            InlineKeyboardButton.WithCallbackData(text: "Назад", callbackData: "TableNumber"),
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
                    callbackData: $"FreeTime {allFreeTime[i].ToString()}"));
            }

            result.Add(tableNumbers);
        }

        result.Add(new[]
        {
            InlineKeyboardButton.WithCallbackData(text: "Назад", callbackData: "Duration"),
        });
        return new InlineKeyboardMarkup(result);
    }

    public static InlineKeyboardMarkup GetIfNeedsAdditionalInfoTab()
    {
        List<IList<InlineKeyboardButton>> result = new();
        result.Add(new[]
        {
            InlineKeyboardButton.WithCallbackData(text: "Ввести дополнительную\nинформацию",
                callbackData: "SetAdditionalInfo"),
        });
        result.Add(new[]
        {
            InlineKeyboardButton.WithCallbackData(text: "Продолжить", callbackData: "ConformationMessage"),
        });
        result.Add(new[]
        {
            InlineKeyboardButton.WithCallbackData(text: "Назад", callbackData: "FreeTime"),
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
            InlineKeyboardButton.WithCallbackData(text: "Назад", callbackData: "IfNeedInfo"),
        });
        return new InlineKeyboardMarkup(result);
    }

    public static InlineKeyboardMarkup GetReservationInfoTab()
    {
        List<IList<InlineKeyboardButton>> result = new();
        result.Add(new[]
        {
            InlineKeyboardButton.WithCallbackData(text: "Отменить", callbackData: "CancelReservation"),
        });
        result.Add(new[]
        {
            InlineKeyboardButton.WithCallbackData(text: "Назад", callbackData: "Reservations"),
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
                    callbackData: $"Reservations {counter.ToString()}"),
            });
            counter++;
        }

        result.Add(new[]
        {
            InlineKeyboardButton.WithCallbackData(text: "Назад", callbackData: "Main"),
        });
        return new InlineKeyboardMarkup(result);
    }

    public static InlineKeyboardMarkup GetBackButtonTab()
    {
        List<IList<InlineKeyboardButton>> result = new();
        result.Add(new[]
        {
            InlineKeyboardButton.WithCallbackData(text: "Назад", callbackData: "IfNeedInfo"),
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
    public static Message GetSetAdditionalInfoMessage()
    {
        Message message = new()
        {
            ReplyMarkup = GetBackButtonTab(),
            Text = File.ReadAllText(FolderWithText + "SetAdditionalInfoMessageText.txt")
        };
        return message;
    }
    public static Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
    {
        var ErrorMessage = exception switch
        {
            ApiRequestException apiRequestException
                => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
            _ => exception.ToString()
        };

        Console.WriteLine(ErrorMessage);
        return Task.CompletedTask;
    }

    public async static Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        string nickname = update.Message!.Chat.Username!;
        string additionalInfo = "";
        User user = new User(nickname);
        Reservation? reservation = user.Reservations.Find((reservation1 => reservation1.InProcess));
        if (reservation == null)
            reservation = new Reservation(new GameTable(), TimeSpan.FromHours(-1), TimeSpan.FromHours(-1), "");
        if (update.Type == UpdateType.CallbackQuery)
        {
            var updateData = update.CallbackQuery.Data.Split(" ",StringSplitOptions.RemoveEmptyEntries);
            switch (updateData[0])
            {
                case "Main":
                    await CallMain(botClient, update, cancellationToken, user);
                    break;
                case "PlayZone":
                {
                    if (updateData.Length > 0)
                    {
                        reservation.
                    }
                } 
                        
            }
        }
        if (update.Type == UpdateType.Message)
        {
            additionalInfo = update.Message.Text!;
            GetConformationMessage();
        }
        
    }
    
    public static async Task CallMain(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken,User user)
    {
        var message = GetMainMessage(user);
        await botClient.SendTextMessageAsync(chatId: update.Message.Chat.Id, text: message.Text,
            cancellationToken: cancellationToken, replyMarkup: message.ReplyMarkup);
        
    }
    public static async Task CallPlayZone(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken,List<PlayZone> zones)
    {
        var message = GetZoneMessage(zones);
        await botClient.SendTextMessageAsync(chatId: update.Message.Chat.Id, text: message.Text,
            cancellationToken: cancellationToken, replyMarkup: message.ReplyMarkup);
    }
    public static async Task CallTableNumber(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken,int amountOfTables)
    {
        var message = GetTableNumberMessage(amountOfTables);
        await botClient.SendTextMessageAsync(chatId: update.Message.Chat.Id, text: message.Text,
            cancellationToken: cancellationToken, replyMarkup: message.ReplyMarkup);
    }
    public static async Task CallDuration(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        var message = GetDurationMessage();
        await botClient.SendTextMessageAsync(chatId: update.Message.Chat.Id, text: message.Text,
            cancellationToken: cancellationToken, replyMarkup: message.ReplyMarkup);
    }
    public static async Task CallFreeTime(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken,List<GameTable> tables)
    {
        var message = GetFreeTimeMessage(tables);
        await botClient.SendTextMessageAsync(chatId: update.Message.Chat.Id, text: message.Text,
            cancellationToken: cancellationToken, replyMarkup: message.ReplyMarkup);
    }
    public static async Task CallIfNeedAdditionalInfo(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        var message = GetIfNeedAdditionalInfoMessage();
        await botClient.SendTextMessageAsync(chatId: update.Message.Chat.Id, text: message.Text,
            cancellationToken: cancellationToken, replyMarkup: message.ReplyMarkup);
    }
    public static async Task CallSetAdditionalInfo(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        var message = GetSetAdditionalInfoMessage();
        await botClient.SendTextMessageAsync(chatId: update.Message.Chat.Id, text: message.Text,
            cancellationToken: cancellationToken, replyMarkup: message.ReplyMarkup);
    }
    public static async Task CallConformationMessage(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken,Reservation reservation, User user)
    {
        var message = GetConformationMessage(reservation,user);
        await botClient.SendTextMessageAsync(chatId: update.Message.Chat.Id, text: message.Text,
            cancellationToken: cancellationToken, replyMarkup: message.ReplyMarkup);
    }
    public static async Task CallReservations(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken,User user)
    {
        var message = GetAllReservationsMessage(user);
        await botClient.SendTextMessageAsync(chatId: update.Message.Chat.Id, text: message.Text,
            cancellationToken: cancellationToken, replyMarkup: message.ReplyMarkup);
    }
    public static async Task CallReservationInfo(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken,Reservation reservation,User user)
    {
        var message = GetReservationInfoMessage(reservation,user);
        await botClient.SendTextMessageAsync(chatId: update.Message.Chat.Id, text: message.Text,
            cancellationToken: cancellationToken, replyMarkup: message.ReplyMarkup);
    }



    
}