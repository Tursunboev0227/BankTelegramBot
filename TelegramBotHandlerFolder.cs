using Telegram.Bot.Polling;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using System.Threading;
using Telegram.Bot.Types.ReplyMarkups;
using System.ComponentModel.Design;

namespace BankTelegramBot
{
    public class TelegramBotHandler
    {
        public string Token { get; set; }
        public TelegramBotHandler(string token)
        {
            this.Token = token;
        }

        public async Task BotHandle()
        {
            var botClient = new TelegramBotClient($"{this.Token}");

            using CancellationTokenSource cts = new();

            // StartReceiving does not block the caller thread. Receiving is done on the ThreadPool.
            ReceiverOptions receiverOptions = new()
            {
                AllowedUpdates = Array.Empty<UpdateType>() // receive all update types except ChatMember related updates
            };

            botClient.StartReceiving(
                updateHandler: HandleUpdateAsync,
                pollingErrorHandler: HandlePollingErrorAsync,
                receiverOptions: receiverOptions,
                cancellationToken: cts.Token
            );

            var me = await botClient.GetMeAsync();

            Console.WriteLine($"Start listening for @{me.Username}");
            Console.ReadLine();

            // Send cancellation request to stop bot
            cts.Cancel();

        }

        public async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            // Only process Message updates: https://core.telegram.org/bots/api#message
            if (update.Message is not { } message)
                return;
            // Only process text messages
            if (message.Text is not { } messageText)
                return;
            List<string> CountryCodes = new List<string>()
            {
                "AED","AUD","CAD","CHF","CNY","DKK","EGP"
            };

            var chatId = message.Chat.Id;
            Console.WriteLine($"Received a '{messageText}' message in chat {chatId}. UserName =>  {message.Chat.Username}");
            if (!CountryCodes.Contains(message.Text) && message.Text == "/start")
            {
                await botClient.SendTextMessageAsync(
                   chatId: chatId,
                   text: "Welcome ",
                   replyToMessageId: update.Message.MessageId,
                   cancellationToken: cancellationToken
                   );
            }

            else
            {
                var money = CenvertionMoney.Convertion(CenvertionMoney.ConnectWithJson(), messageText);
                await botClient.SendTextMessageAsync(
                    chatId: chatId,
                    text: $"{money} sums",
                    replyToMessageId: update.Message.MessageId,
                    cancellationToken: cancellationToken
                    );
            }
        
            var replyKeyboard = new ReplyKeyboardMarkup(
            new List<KeyboardButton[]>()
             {
            new KeyboardButton[]
            {
                new KeyboardButton("AED"),
                new KeyboardButton("AUD"),
                new KeyboardButton("CAD"),
                new KeyboardButton("CHF"),
                new KeyboardButton("CNY"),
                new KeyboardButton("DKK"),
                new KeyboardButton("EGP"),

            },
            new KeyboardButton[]
            {
                new KeyboardButton("EUR"),
                new KeyboardButton("GBP"),
                new KeyboardButton("ISK"),
                new KeyboardButton("JPY"),
                new KeyboardButton("KRW"),
                new KeyboardButton("KWD"),
                new KeyboardButton("KZT"),

            },
             new KeyboardButton[]
            {
                new KeyboardButton("LBP"),
                new KeyboardButton("MYR"),
                new KeyboardButton("NOK"),
                new KeyboardButton("PLN"),
                new KeyboardButton("RUB"),
                new KeyboardButton("SEK"),
                new KeyboardButton("SGD"),

            },
             new KeyboardButton[]
            {
                new KeyboardButton("TRY"),
                new KeyboardButton("UAH"),
                new KeyboardButton("USD"),
            }

             })
            {
                ResizeKeyboard = true
            };

            await botClient.SendTextMessageAsync(
                chatId: chatId,
                cancellationToken: cancellationToken,
                text: "Thanks for being with us",
                replyMarkup: replyKeyboard
                );

        }

        public Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
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
}
