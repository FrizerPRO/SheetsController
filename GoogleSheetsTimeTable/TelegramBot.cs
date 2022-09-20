using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types.Enums;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Requests;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
namespace SheetsController;

public class TelegramBot
{
    public TelegramBotClient BotClient
    {
        get;
    }

    public CancellationTokenSource CancellationTokenSource
    {
        get;
    }
    public TelegramBot(string token,CancellationTokenSource cancellationToken)
    {
        CancellationTokenSource = cancellationToken;
        BotClient = new TelegramBotClient(token);
    }

    public void Run()
    {
        var receiverOptions = new ReceiverOptions
        {
            AllowedUpdates = new []{UpdateType.Message,UpdateType.CallbackQuery}
        };
        BotClient.StartReceiving(
            updateHandler: TelegramBotController.HandleUpdateAsync,
            pollingErrorHandler: TelegramBotController.HandlePollingErrorAsync,
            cancellationToken: CancellationTokenSource.Token,
            receiverOptions: receiverOptions
        );
    }

}