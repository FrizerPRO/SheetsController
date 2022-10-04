using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types.Enums;

namespace SheetsController;

public class TelegramBot
{
    private readonly DateTime _midnight = SheetsController.Today.AddDays(1).AddMinutes(-2);

    public TelegramBot(string token, CancellationTokenSource cancellationToken)
    {
        CancellationTokenSource = cancellationToken;
        BotClient = new TelegramBotClient(token);
    }

    public TelegramBotClient BotClient { get; }

    public CancellationTokenSource CancellationTokenSource { get; }

    public void Run()
    {
        BotClient.Timeout = TimeSpan.FromMinutes(10);
        var clock = new AlarmClock(_midnight);
        clock.Alarm += async (sender, e) =>
            {
                UserControl.DeleteAllUsersJson();
                foreach (var zone in DataBase.Zones) await zone.RefreshInMidnight();
            }
            ;
        var receiverOptions = new ReceiverOptions();
        receiverOptions.AllowedUpdates = new[] { UpdateType.Message, UpdateType.CallbackQuery };
        BotClient.StartReceiving(
            TelegramBotController.HandleUpdateAsyncWithTryCatch,
            TelegramBotController.HandlePollingErrorAsync,
            cancellationToken: CancellationTokenSource.Token,
            receiverOptions: receiverOptions
        );
    }
}