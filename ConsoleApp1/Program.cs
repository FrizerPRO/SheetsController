using SheetsController;

using var cts = new CancellationTokenSource();
TelegramBot bot = new("641114157:AAHVLzkTq8QnfXuubRm3zXVM_CMzZQ8QODY", cts);
bot.Run();
Console.ReadLine();