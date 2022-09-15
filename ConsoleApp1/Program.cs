// See https://aka.ms/new-console-template for more information

using SheetsController;

// var t = new SheetsController.SheetsController("1PQTy_fIQSOaw2RQoDwkY2F_TCRdPxbQ3E64jclb-Av0");
// t.Test();
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Requests;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

async Task NewFunction()
{
    var botClient = new TelegramBotClient("641114157:AAHVLzkTq8QnfXuubRm3zXVM_CMzZQ8QODY");

    using var cts = new CancellationTokenSource();

// StartReceiving does not block the caller thread. Receiving is done on the ThreadPool.
    var receiverOptions = new ReceiverOptions
    {
        AllowedUpdates = Array.Empty<UpdateType>() // receive all update types
    };
    botClient.StartReceiving(
        updateHandler: HandleUpdateAsync,
        pollingErrorHandler: HandlePollingErrorAsync,
        cancellationToken: cts.Token,
        receiverOptions: receiverOptions
    );

    var me = await botClient.GetMeAsync();

    Console.WriteLine($"Start listening for @{me.Username}");
    Console.ReadLine();

// Send cancellation request to stop bot
    cts.Cancel();

    async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        // Only process Message updates: https://core.telegram.org/bots/api#message
        if (update.Type == UpdateType.CallbackQuery)
        {
            Console.WriteLine(update.CallbackQuery.Data);
        }

        if (update.Message is not { } message)
            return;
        // Only process text messages
        if (message.Text is not { } messageText)
            return;
        
        var chatId = message.Chat.Username;
        Console.WriteLine($"Received a '{messageText}' message in chat {chatId}.");
        
        // Echo received message text
        //TODO: DELETE!!!!!!!!!!
        SheetsController.SheetsController sheetsController = new("1PQTy_fIQSOaw2RQoDwkY2F_TCRdPxbQ3E64jclb-Av0");
        PlayZone zone = new("Comfort", 9, sheetsController);
        //zone.RefreshInMidnight();
        //zone.Refresh();
        var duration = TimeSpan.FromHours(1);
        var tableNumber = 5;
        var values = sheetsController.GetFreeTables(zone, duration, tableNumber);
        //TODO: UP to here
        InlineKeyboardMarkup inlineKeyboard = TelegramBotController.GetFreeTimeTab(values);

        Message sentMessage = await botClient.SendTextMessageAsync(
            chatId: message.Chat.Id,
            text: "You said:\n" + messageText,
            cancellationToken: cancellationToken, replyMarkup: inlineKeyboard);
    }

    Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
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
}

await NewFunction();