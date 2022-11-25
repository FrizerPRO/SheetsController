using SheetsController;

using var cts = new CancellationTokenSource();
TelegramBot bot = new(TELEGRAM_BOT_KEY, cts);
bot.Run();
Console.ReadLine();
