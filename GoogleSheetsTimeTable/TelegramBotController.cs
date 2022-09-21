using System.Data;
using System.Net.Mime;
using Google.Apis.Sheets.v4.Data;
using SheetsController;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Requests;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using File = System.IO.File;
using User = SheetsController.User;


public static class TelegramBotController
{
    private static readonly string FolderWithText = "../../../../ConsoleApp1/TextForMessages/";

    public static async Task<InlineKeyboardMarkup> GetMainTab(User user)
    {
        List<IList<InlineKeyboardButton>> result = new();
        result.Add(new[]
        {
            InlineKeyboardButton.WithCallbackData(text: "Новая Запись", callbackData: "PlayZone"),
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

    public static async Task<InlineKeyboardMarkup> GetZonesTab(List<PlayZone> zones)
    {
        List<IList<InlineKeyboardButton>> result = new();
        var counter = 0;
        foreach (var zone in zones)
        {
            result.Add(new[]
            {
                InlineKeyboardButton.WithCallbackData(text: zone.Name,
                    callbackData: $"PlayZone {counter.ToString()}"),
            });
            counter++;
        }

        result.Add(new[]
        {
            InlineKeyboardButton.WithCallbackData(text: "Назад", callbackData: "Main"),
        });
        return new InlineKeyboardMarkup(result);
    }

    public static async Task<InlineKeyboardMarkup> GetTableNumber(int amountOfTables)
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

    public static async Task<InlineKeyboardMarkup> GetDurationTab()
    {
        List<IList<InlineKeyboardButton>> result = new();
        for (var i = SheetsController.SheetsController.MinimumStep; i <= SheetsController.SheetsController.MaxReservationDuration;)
        {
            List<InlineKeyboardButton> tableNumbers = new();
            for (var j = 0;
                 j < 3 && i <= SheetsController.SheetsController.MaxReservationDuration;
                 j++, i += SheetsController.SheetsController.MinimumStep)
            {
                tableNumbers.Add(InlineKeyboardButton.WithCallbackData(text: i.ToString(),
                    callbackData: $"Duration {i.ToString()}"));
            }

            result.Add(tableNumbers);
        }

        result.Add(new[]
        {
            InlineKeyboardButton.WithCallbackData(text: "Назад", callbackData: "TableNumber"),
        });
        return new InlineKeyboardMarkup(result);
    }

    public static async Task<InlineKeyboardMarkup> GetFreeTimeTab(List<GameTable> gameTables)
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

    public static async Task<InlineKeyboardMarkup> GetIfNeedsAdditionalInfoTab()
    {
        List<IList<InlineKeyboardButton>> result = new();
        result.Add(new[]
        {
            InlineKeyboardButton.WithCallbackData(text: "Ввести дополнительную\nинформацию",
                callbackData: "SetAdditionalInfo"),
        });
        result.Add(new[]
        {
            InlineKeyboardButton.WithCallbackData(text: "Пропустить", callbackData: "ConformationMessage"),
        });
        result.Add(new[]
        {
            InlineKeyboardButton.WithCallbackData(text: "Назад", callbackData: "FreeTime"),
        });
        return new InlineKeyboardMarkup(result);
    }

    public static async Task<InlineKeyboardMarkup> GetConformationTab()
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

    public static async Task<InlineKeyboardMarkup> GetReservationInfoTab(int indexOfReservation)
    {
        List<IList<InlineKeyboardButton>> result = new();
        result.Add(new[]
        {
            InlineKeyboardButton.WithCallbackData(text: "Отменить",
                callbackData: $"CancelReservation {indexOfReservation}"),
        });
        result.Add(new[]
        {
            InlineKeyboardButton.WithCallbackData(text: "Назад", callbackData: "Main"),
        });
        return new InlineKeyboardMarkup(result);
    }

    public static async Task<InlineKeyboardMarkup> GetAllReservationsTab(User user)
    {
        List<IList<InlineKeyboardButton>> result = new();
        var counter = 0;
        foreach (var reservation in user.Reservations)
        {
            result.Add(new[]
            {
                InlineKeyboardButton.WithCallbackData(text: reservation.StartTime.ToString() +
                                                            " " + reservation.Table.Zone.Name,
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

    public static async Task<InlineKeyboardMarkup> GetBackButtonTab()
    {
        List<IList<InlineKeyboardButton>> result = new();
        result.Add(new[]
        {
            InlineKeyboardButton.WithCallbackData(text: "Назад", callbackData: "IfNeedInfo"),
        });
        return new InlineKeyboardMarkup(result);
    }

    public static async Task<Message> GetMainMessage(User user)
    {
        Message message = new()
        {
            ReplyMarkup = await GetMainTab(user),
            Text = File.ReadAllText(FolderWithText + "MainMessageText.txt")
        };
        return message;
    }

    public static async Task<Message> GetZoneMessage(List<PlayZone> zones)
    {
        Message message = new()
        {
            ReplyMarkup = await GetZonesTab(zones),
            Text = File.ReadAllText(FolderWithText + "ZoneMessageText.txt")
        };
        return message;
    }

    public static async Task<Message> GetTableNumberMessage(int amountOfTables)
    {
        Message message = new()
        {
            ReplyMarkup = await GetTableNumber(amountOfTables),
            Text = File.ReadAllText(FolderWithText + "AmountOfTablesMessageText.txt")
        };
        return message;
    }

    public static async Task<Message> GetDurationMessage()
    {
        Message message = new()
        {
            ReplyMarkup = await GetDurationTab(),
            Text = File.ReadAllText(FolderWithText + "DurationMessageText.txt")
        };
        return message;
    }

    public static async Task<Message> GetFreeTimeMessage(List<GameTable> gameTables)
    {
        Message message = new()
        {
            ReplyMarkup = await GetFreeTimeTab(gameTables),
            Text = File.ReadAllText(FolderWithText + "FreeTimeMessageText.txt")
        };
        return message;
    }

    public static async Task<Message> GetIfNeedAdditionalInfoMessage()
    {
        Message message = new()
        {
            ReplyMarkup = await GetIfNeedsAdditionalInfoTab(),
            Text = File.ReadAllText(FolderWithText + "IfNeedAdditionalInfoMessageText.txt")
        };
        return message;
    }

    public static async Task<Message> GetConformationMessage(Reservation reservation, User user)
    {
        Message message = new()
        {
            ReplyMarkup = await GetConformationTab(),
            Text = GetReservationInfo(reservation, user)
        };
        return message;
    }

    private static string GetReservationInfo(Reservation reservation, User user)
    {
        string result = "Запись:\n" +
                        $"Начало: *{reservation.StartTime}*\n" +
                        $"Период: *{reservation.Duration}*\n" +
                        $"Зал: *{reservation.Table.Zone.Name}*\n" +
                        $"На имя: *{user.Nickname}*";
        return result;
    }

    public static async Task<Message> GetAllReservationsMessage(User user)
    {
        Message message = new()
        {
            ReplyMarkup = await GetAllReservationsTab(user),
            Text = File.ReadAllText(FolderWithText + "AllReservationsMessageText.txt")
        };
        return message;
    }

    public static async Task<Message> GetReservationInfoMessage(Reservation reservation, User user)
    {
        Message message = new()
        {
            ReplyMarkup = await GetReservationInfoTab(user.Reservations.IndexOf(reservation)),
            Text = GetReservationInfo(reservation, user)
        };
        return message;
    }

    public static async Task<Message> GetSetAdditionalInfoMessage()
    {
        Message message = new()
        {
            ReplyMarkup = await GetBackButtonTab(),
            Text = File.ReadAllText(FolderWithText + "SetAdditionalInfoMessageText.txt")
        };
        return message;
    }

    public static Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception,
        CancellationToken cancellationToken)
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

    public async static Task HandleUpdateAsync(ITelegramBotClient botClient, Update update,
        CancellationToken cancellationToken)
    {
        
        string nickname = "EMPTY!!!";
        switch (update.Type)
        {
            case UpdateType.Message:
                Console.WriteLine(update.Message.Text);
                nickname = update.Message!.Chat.Username!;
                break;
            case UpdateType.CallbackQuery:
                Console.WriteLine(update.CallbackQuery.Data);
                nickname = update.CallbackQuery!.From.Username!;
                break;
        }

        User user = new User(nickname);
        Reservation? reservation = user.Reservations.Find((reservation1 => reservation1.InProcess));
        if (reservation == null)
            reservation = new Reservation(new GameTable(), TimeSpan.FromHours(-1), TimeSpan.FromHours(-1), "");
        if (update.Type == UpdateType.CallbackQuery)
        {
            var updateData = update.CallbackQuery.Data.Split(" ", StringSplitOptions.RemoveEmptyEntries);
            switch (updateData[0])
            {
                
                case "Main":
                    await CallMain(botClient, update, cancellationToken, user);
                    break;
                case "PlayZone":
                {
                    if (updateData.Length > 1)
                    {
                        reservation.Zone = DataBase.Zones[int.Parse(updateData[1])];
                        UserControl.AddReservation(user, reservation);
                        await CallTableNumber(botClient, update, cancellationToken, reservation.Zone.Capacity);
                        return;
                    }

                    await CallPlayZone(botClient, update, cancellationToken, DataBase.Zones);
                    break;
                }
                case "TableNumber":
                {
                    if (updateData.Length > 1)
                    {
                        reservation.Table = new GameTable(reservation.Zone, int.Parse(updateData[1]));
                        UserControl.AddReservation(user, reservation);
                        await CallDuration(botClient, update, cancellationToken);
                        return;
                    }

                    await CallTableNumber(botClient, update, cancellationToken, reservation.Zone.Capacity);
                    break;
                }
                case "Duration":
                {
                    if (updateData.Length > 1)
                    {
                        reservation.Duration = TimeSpan.Parse(updateData[1]);
                        var tables = SheetsController.SheetsController.GetFreeTables(reservation.Zone,
                            reservation.Duration,
                            reservation.Table.Number);
                        UserControl.AddReservation(user, reservation);
                        await CallFreeTime(botClient, update, cancellationToken, tables);
                        return;
                    }

                    await CallDuration(botClient, update, cancellationToken);
                    break;
                }
                case "FreeTime":
                {
                    var freeTables = SheetsController.SheetsController.GetFreeTables(reservation.Zone,
                        reservation.Duration,
                        reservation.Table.Number);
                    if (updateData.Length > 1)
                    {
                        reservation.StartTime = TimeSpan.Parse(updateData[1]);
                        var table =
                            SheetsController.SheetsController.GetFreeTable(freeTables, reservation.StartTime);
                        if (table == null)
                        {
                            await CallFreeTime(botClient, update, cancellationToken, freeTables);
                            return;
                        }

                        reservation.Table = table;
                        UserControl.AddReservation(user, reservation);
                        await CallIfNeedAdditionalInfo(botClient, update, cancellationToken);
                        return;
                    }

                    await CallFreeTime(botClient, update, cancellationToken, freeTables);
                    break;
                }
                case "SetAdditionalInfo":
                {
                    await CallSetAdditionalInfo(botClient, update, cancellationToken);
                    break;
                }
                case "ConformationMessage":
                {
                    await CallConformationMessage(botClient, update, cancellationToken, reservation, user);
                    break;
                }
                case "Confirm":
                {
                    UserControl.RemoveReservation(user, reservation);
                    UserControl.AddReservation(user, new List<GameTable>(){reservation.Table}, reservation.StartTime,
                        reservation.Duration, reservation.AdditionalInfo, reservation.Table.Number);
                    await CallMain(botClient, update, cancellationToken, user);
                    break;
                }
                case "Reservations":
                {
                    if (updateData.Length > 1)
                    {
                        await CallReservationInfo(botClient, update, cancellationToken,
                            user.Reservations[int.Parse(updateData[1])], user);
                        return;
                    }

                    await CallReservations(botClient, update, cancellationToken, user);
                    break;
                }
                case "IfNeedInfo":
                {
                    await CallIfNeedAdditionalInfo(botClient, update, cancellationToken);
                    break;
                }
                case "CancelReservation":
                {
                    user = await UserControl.RemoveReservation(user, user.Reservations[int.Parse(updateData[1])]);
                    await CallMain(botClient, update, cancellationToken, user);
                    break;
                }
            }
        }

        if (update.Type == UpdateType.Message)
        {
            if (update.Message.Text == "/start")
            {
                await CallMainWithNewMessage(botClient, update, cancellationToken, user);
            }
            else if (reservation.StartTime != TimeSpan.FromHours(-1))
            {
                reservation.AdditionalInfo = update.Message.Text!;
                UserControl.AddReservation(user, reservation);
                await CallConformationWithNewMessage(botClient, update, cancellationToken, reservation, user);
            }
        }
    }

    public static async Task CallMain(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken,
        User user)
    {
        var message = await GetMainMessage(user);
        Console.WriteLine(user.Nickname);
        await botClient.EditMessageTextAsync(chatId:await GetChatIdFromUpdate(update),
            messageId:(int)await GetMessageIdFromUpdate(update), text:message.Text,replyMarkup:message.ReplyMarkup, cancellationToken: cancellationToken,parseMode:ParseMode.Markdown);
    }
    public static async Task CallMainWithNewMessage(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken,
        User user)
    {
        var message = await GetMainMessage(user);
        Console.WriteLine(user.Nickname);
        await botClient.SendTextMessageAsync(chatId: await GetChatIdFromUpdate(update), text: message.Text,parseMode:ParseMode.Markdown,
            cancellationToken: cancellationToken, replyMarkup: message.ReplyMarkup);
    }

    public static async Task CallPlayZone(ITelegramBotClient botClient, Update update,
        CancellationToken cancellationToken, List<PlayZone> zones)
    {
        var message = await GetZoneMessage(zones);
        await botClient.EditMessageTextAsync(chatId:await GetChatIdFromUpdate(update),
            messageId:(int)await GetMessageIdFromUpdate(update), text:message.Text,replyMarkup:message.ReplyMarkup, cancellationToken: cancellationToken,parseMode:ParseMode.Markdown);
    }

    public static async Task CallTableNumber(ITelegramBotClient botClient, Update update,
        CancellationToken cancellationToken, int amountOfTables)
    {
        var message = await GetTableNumberMessage(amountOfTables);
        await botClient.EditMessageTextAsync(chatId:await GetChatIdFromUpdate(update),
            messageId:(int)await GetMessageIdFromUpdate(update), text:message.Text,replyMarkup:message.ReplyMarkup, cancellationToken: cancellationToken,parseMode:ParseMode.Markdown);
    }

    private static async Task<long> GetChatIdFromUpdate(Update update)
    {
        switch (update.Type)
        {
            case UpdateType.Message:
                return update.Message!.Chat.Id;
            default:
                return update.CallbackQuery!.From.Id;
        }
    }
    private static async Task<long> GetMessageIdFromUpdate(Update update)
    {
        switch (update.Type)
        {
            case UpdateType.Message:
                return update.Message!.MessageId;
            default:
                return update.CallbackQuery!.Message!.MessageId;
        }
    }
    public static async Task CallDuration(ITelegramBotClient botClient, Update update,
        CancellationToken cancellationToken)
    {
        var message = await GetDurationMessage();
        await botClient.EditMessageTextAsync(chatId:await GetChatIdFromUpdate(update),
            messageId:(int)await GetMessageIdFromUpdate(update), text:message.Text,replyMarkup:message.ReplyMarkup,parseMode:ParseMode.Markdown);
    }

    public static async Task CallFreeTime(ITelegramBotClient botClient, Update update,
        CancellationToken cancellationToken, List<GameTable> tables)
    {
        var message = await GetFreeTimeMessage(tables);
        await botClient.EditMessageTextAsync(chatId:await GetChatIdFromUpdate(update),
            messageId:(int)await GetMessageIdFromUpdate(update), text:message.Text,replyMarkup:message.ReplyMarkup,parseMode:ParseMode.Markdown);
    }

    public static async Task CallIfNeedAdditionalInfo(ITelegramBotClient botClient, Update update,
        CancellationToken cancellationToken)
    {
        var message = await GetIfNeedAdditionalInfoMessage();
        await botClient.EditMessageTextAsync(chatId:await GetChatIdFromUpdate(update),
            messageId:(int)await GetMessageIdFromUpdate(update), text:message.Text,replyMarkup:message.ReplyMarkup, cancellationToken: cancellationToken,parseMode:ParseMode.Markdown);
    }

    public static async Task CallSetAdditionalInfo(ITelegramBotClient botClient, Update update,
        CancellationToken cancellationToken)
    {
        var message = await GetSetAdditionalInfoMessage();
        await botClient.EditMessageTextAsync(chatId:await GetChatIdFromUpdate(update),
            messageId:(int)await GetMessageIdFromUpdate(update), text:message.Text,replyMarkup:message.ReplyMarkup, cancellationToken: cancellationToken,parseMode:ParseMode.Markdown);
    }

    public static async Task CallConformationMessage(ITelegramBotClient botClient, Update update,
        CancellationToken cancellationToken, Reservation reservation, User user)
    {
        var message =await GetConformationMessage(reservation, user);
        await botClient.EditMessageTextAsync(chatId:await GetChatIdFromUpdate(update),
            messageId:(int)await GetMessageIdFromUpdate(update), text:message.Text,replyMarkup:message.ReplyMarkup, cancellationToken: cancellationToken,parseMode:ParseMode.Markdown);
    }
    public static async Task CallConformationWithNewMessage(ITelegramBotClient botClient, Update update,
        CancellationToken cancellationToken, Reservation reservation, User user)
    {
        var message =await GetConformationMessage(reservation, user);
        await botClient.SendTextMessageAsync(chatId:await GetChatIdFromUpdate(update), text: message.Text,
            cancellationToken: cancellationToken, replyMarkup: message.ReplyMarkup,parseMode:ParseMode.Markdown);
    }

    public static async Task CallReservations(ITelegramBotClient botClient, Update update,
        CancellationToken cancellationToken, User user)
    {
        user.Reservations.RemoveAll((reservation1) => reservation1.InProcess);
        var message = await GetAllReservationsMessage(user);
        await botClient.EditMessageTextAsync(chatId:await GetChatIdFromUpdate(update),
            messageId:(int)await GetMessageIdFromUpdate(update), text:message.Text,replyMarkup:message.ReplyMarkup, cancellationToken: cancellationToken,parseMode:ParseMode.Markdown);
    }

    public static async Task CallReservationInfo(ITelegramBotClient botClient, Update update,
        CancellationToken cancellationToken, Reservation reservation, User user)
    {
        var message = await GetReservationInfoMessage(reservation, user);
        await botClient.EditMessageTextAsync(chatId:await GetChatIdFromUpdate(update),
            messageId:(int)await GetMessageIdFromUpdate(update), text:message.Text,replyMarkup:message.ReplyMarkup, cancellationToken: cancellationToken,parseMode:ParseMode.Markdown);
    }
}