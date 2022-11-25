using SheetsController;
using System.IO;
using var cts = new CancellationTokenSource();
var key =
            File.ReadAllText(
                Directory.GetParent(Assembly.GetEntryAssembly().Location) +
                "/Credential/telegram_token.txt");
TelegramBot bot = new(key, cts);
bot.Run();
Console.ReadLine();
