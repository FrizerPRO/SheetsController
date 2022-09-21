// See https://aka.ms/new-console-template for more information

using Google.Apis.Sheets.v4.Data;
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

//await NewFunction();

using var cts = new CancellationTokenSource();
TelegramBot bot = new("641114157:AAHVLzkTq8QnfXuubRm3zXVM_CMzZQ8QODY",cts);
bot.Run();
Console.ReadLine();
SheetsController.SheetsController.Test();
