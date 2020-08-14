using System;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;

namespace SalgadoBot
{
    public class SalgadoModule : ModuleBase<SocketCommandContext>
    {
        [Command("pontos")] public async Task Pontos([Remainder][Summary("Quem acusar")] string userName)
        {
            var service = await SalgadoService.GetInstance();
            var users = await service.GetUsersByQuery(userName);

            if (!users.Any())
            {
                await ReplyAsync($"Não encontrei nenhum usuário com nome {userName}");
            } else if (users.Count > 1)
            {
                await ReplyAsync($"Nome ambíguo ({string.Join(",", users.Select(u => u.Name))})");
            } else if (users.Count == 1)
            {
                var user = users.First();
                await ReplyAsync($"{user.Name} tem {user.Pontos} pontos");
            }
        }

        [Command("leaderboard")] public async Task Leaderboard()
        {
            var service = await SalgadoService.GetInstance();
            var users = await service.GetUsers();
            var orderedUsers = users.OrderByDescending(u => u.Pontos).ToArray();

            string message = "`";
            for (int i = 0; i < orderedUsers.Length; i++)
            {
                var user = orderedUsers[i];
                message += $"{i + 1,-3} -- {user.Name,-30} -- {user.Pontos}\n";
            }

            message += "`";

            await ReplyAsync(message);
        }

        //[RequireUserPermission(GuildPermission.Administrator)]
        [Command("addponto")] public async Task AddPoint([Remainder] [Summary("Quem subir ponto")]
            string userName)
        {
            var service = await SalgadoService.GetInstance();
            var users = await service.GetUsersByQuery(userName);

            if (!users.Any())
            {
                await ReplyAsync($"Não encontrei nenhum usuário com nome {userName}");
            } else if (users.Count > 1)
            {
                await ReplyAsync($"Nome ambíguo ({string.Join(",", users.Select(u => u.Name))})");
            } else if (users.Count == 1)
            {
                var user = users.First();
                var updatedUser = await service.EditPoints(user.ChangePoints(user.Pontos + 1));
                await ReplyAsync($"{updatedUser.Name} agora tem {updatedUser.Pontos} pontos.");
            }
        }
        
        //[RequireUserPermission(GuildPermission.Administrator)]
        [Command("rmvponto")] public async Task RemovePoint([Remainder] [Summary("Quem tirar ponto")]
            string userName)
        {
            var service = await SalgadoService.GetInstance();
            var users = await service.GetUsersByQuery(userName);

            if (!users.Any())
            {
                await ReplyAsync($"Não encontrei nenhum usuário com nome {userName}");
            } else if (users.Count > 1)
            {
                await ReplyAsync($"Nome ambíguo ({string.Join(",", users.Select(u => u.Name))})");
            } else if (users.Count == 1)
            {
                var user = users.First();
                var updatedUser = await service.EditPoints(user.ChangePoints(user.Pontos -1));
                await ReplyAsync($"{updatedUser.Name} agora tem {updatedUser.Pontos} pontos.");
            }
        }
        
        [Command("code")] public async Task LinkRepo()
        {
            await ReplyAsync(Environment.GetEnvironmentVariable("discord_bot_repo")!);
        }

        [Command("help")] public async Task Help()
        {
            var helpText = @"`Comandos:
!leaderboard: veja quem deve mais centos
!pontos [nome]: acusar um coleguinha de dever centos.
!addponto [nome]: dê um ponto para um coleguinha
!rmvponto [nome]: tire um ponto de um coleguinha
!code: veja o código fonte`";
            await ReplyAsync(helpText);
        }
    }
}
