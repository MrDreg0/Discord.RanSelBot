using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord.Commands;
using Discord.RanSelBot.Models;
using Discord.WebSocket;

namespace Discord.RanSelBot.Modules
{
  [Name("SelectModule")]
  public class SelectModule : ModuleBase<SocketCommandContext>
  {
    private static readonly Random random = new();

    [Command("Выбери кто будет")]
    [Summary("Выбирает одного пользователя, с необходимыми ролями, \nчтобы сообщить ему описание действия. \n\n " +
      "Пример: \n " +
      "`!выбери кто будет \"мыть посуду\" роли: \"Admins, Users\"` \n" +
      "`!выбери кто будет \"выносить мусор\" роли: \"Admins\" исключая: \"@user_tag\"`\n\n" +
      "Можно обратиться к боту напрямую (тогда префикс команды не нужен):\n" +
      "`@bot_name выбери кто будет \"править баги\" роли: \"бедолага\"`")]
    [RequireUserPermission(GuildPermission.SendMessages)]
    public Task Select(string описание_действия, NamableArguments args)
    {
      var users = Context.Channel.GetUsersAsync().FlattenAsync().GetAwaiter().GetResult();

      if(args.Роли is null
        || args.Роли.Contains("")
        || args.Роли.Contains("роли:", StringComparer.OrdinalIgnoreCase)
        || !args.Роли.Any())
      {
        return ReplyAsync(message: $"{Context.Message.Author.Mention}," +
          $" если не выбрана роль(и), выбирать будем из пользователей всего сервера, а это плохая идея.");
      }

      var usersWithRoles =
        users
          .Select(user => user as SocketGuildUser)
          .Where(sockerGuildUser =>
            sockerGuildUser.Roles
              .Any(userRole =>
                args.Роли
                  .Contains(userRole.Name, StringComparer.OrdinalIgnoreCase)))
          .ToList();

      if (!usersWithRoles.Any())
      {
        return ReplyAsync(message: $"Не смог найти пользователей с ролями: '{string.Join(", ", args.Роли)}'");
      }

      var usersWithoutExcluded = new List<SocketGuildUser>();

      if (args.Исключая is null
        || args.Исключая.Contains("")
        || args.Исключая.Contains("исключая:", StringComparer.OrdinalIgnoreCase)
        || !args.Исключая.Any())
      {
        usersWithoutExcluded = usersWithRoles;

      }
      else
      {
        var excludedUsers = usersWithRoles.Where(user => args.Исключая.Contains(user.Mention));

        usersWithoutExcluded = usersWithRoles.Except(excludedUsers).ToList();

        if (!usersWithoutExcluded.Any())
        {
          return ReplyAsync(message:
            $"После исключения пользователей: '{string.Join(", ", excludedUsers.Select(user => user.Mention))}'," +
            @" не осталось из кого выбирать ¯\_(ツ)_/¯.");
        }
      }

      var randomUser = GetRandomUser(usersWithoutExcluded);

      return ReplyAsync($"{randomUser.Mention}, тебе выпала честь {описание_действия}.");
    }

    private IUser GetRandomUser(IEnumerable<IUser> users)
    {
      var index = random.Next(users.Count());

      return users.ElementAt(index);
    }
  }
}
