using System;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace SalgadoBot
{
    class Program
    {
        public static DiscordSocketClient Client;

        static async Task Main(string[] args)
        {
            Console.CancelKeyPress += CancelHandler;
            
            Client = new DiscordSocketClient(new DiscordSocketConfig
            {
                LogLevel = LogSeverity.Info
            });

            Client.Log += Logger.Instance.Log;

            await Client.LoginAsync(TokenType.Bot, Environment.GetEnvironmentVariable("Discord_Token"));

            await Client.StartAsync();

            var commands = new CommandService();
            var commandHandler = new CommandHandler(Client, commands);
            await commandHandler.InstallCommandsAsync();
            
            await Task.Delay(-1);
        }

        private static void CancelHandler(object sender, ConsoleCancelEventArgs args)
        {
            args.Cancel = true;

            Task.Run(async () =>
            {
                await Client.LogoutAsync();
                await Client.StopAsync();
                Logger.Instance.CleanupLogger();
                Environment.Exit(0);
            });
        }
    }
}