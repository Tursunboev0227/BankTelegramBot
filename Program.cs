namespace BankTelegramBot
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            const string token = "6887897602:AAFuow0EI5Ks7sZbz_eDVYJEettrtMTIwz4";
            TelegramBotHandler handler = new TelegramBotHandler(token);

            try
            {
                await handler.BotHandle();
            }
            catch (Exception ex)
            {
                throw new Exception("What's up");
            }
        }
    }
}
