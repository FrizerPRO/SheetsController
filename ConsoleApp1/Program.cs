using SheetsController;
using System.IO;
using var cts = new CancellationTokenSource();
var key =
            File.ReadAllText("/Credential/telegram_token.txt");

TelegramBot bot = new(key, cts);
bot.Run();
Console.ReadLine();
