using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types.Enums;

namespace SheetsController
{

    public class TelegramBot
    {
        public TelegramBot(string token, CancellationTokenSource cancellationToken)
        {
            CancellationTokenSource = cancellationToken;
            BotClient = new TelegramBotClient(token);
        }

        public TelegramBotClient BotClient { get; }

        public CancellationTokenSource CancellationTokenSource { get; }

        public void Run()
        {
            var receiverOptions = new ReceiverOptions
            {
                AllowedUpdates = new[] { UpdateType.Message, UpdateType.CallbackQuery }
            };
            BotClient.StartReceiving(
                TelegramBotController.HandleUpdateAsync,
                TelegramBotController.HandlePollingErrorAsync,
                cancellationToken: CancellationTokenSource.Token,
                receiverOptions: receiverOptions
            );
        }
    }
}