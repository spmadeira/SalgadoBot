using System.Reflection;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using SalgadoBot;

public class CommandHandler
{
    private readonly DiscordSocketClient _client;
    private readonly CommandService _commands;

    public CommandHandler(DiscordSocketClient client, CommandService commands)
    {
        _commands = commands;
        _client = client;
    }

    public async Task InstallCommandsAsync()
    {
        _client.MessageReceived += HandleCommandAsync;

        var modules = await _commands.AddModulesAsync(Assembly.GetEntryAssembly(), null);

        foreach (var module in modules)
        {
            await Logger.Instance.Log(new LogMessage(LogSeverity.Info, "Module", $"Loaded module {module.Name}"));
        }
    }

    private async Task HandleCommandAsync(SocketMessage messageParam)
    {
        if (!(messageParam is SocketUserMessage message)) return;

        int argPos = 0;
        
        if (!(message.HasCharPrefix('!', ref argPos) || 
              message.HasMentionPrefix(_client.CurrentUser, ref argPos)) ||
            message.Author.IsBot)
            return;
        
        var context = new SocketCommandContext(_client,message);

        var result = await _commands.ExecuteAsync(context, argPos, null);

        if (!result.IsSuccess)
        {
            await Logger.Instance.Log(new LogMessage(LogSeverity.Error, "Cmd. Fail", result.ErrorReason));
        }
    }
}
