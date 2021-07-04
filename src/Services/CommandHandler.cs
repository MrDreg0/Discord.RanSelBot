using System;
using System.Threading.Tasks;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;

namespace Discord.RanSelBot.Services
{
  public class CommandHandler
  {
    private readonly DiscordSocketClient _discord;
    private readonly CommandService _commands;
    private readonly IConfigurationRoot _config;
    private readonly IServiceProvider _provider;

    public CommandHandler(
        DiscordSocketClient discord,
        CommandService commands,
        IConfigurationRoot config,
        IServiceProvider provider)
    {
      _discord = discord;
      _commands = commands;
      _config = config;
      _provider = provider;

      _discord.MessageReceived += OnMessageReceivedAsync;
    }

    private async Task OnMessageReceivedAsync(SocketMessage s)
    {
      if (s is not SocketUserMessage msg)
      {
        return;
      }

      if (msg.Author.Id == _discord.CurrentUser.Id)
      {
        return;
      }

      var context = new SocketCommandContext(_discord, msg);

      var argPos = 0;

      if (msg.HasStringPrefix(_config["prefix"], ref argPos) 
        || msg.HasMentionPrefix(_discord.CurrentUser, ref argPos))
      {
        var result = await _commands.ExecuteAsync(context, argPos, _provider);

        if (!result.IsSuccess)
        {
          await context.Channel.SendMessageAsync(
            text: $"{msg.Author.Mention}, не понимаю чего ты хочешь. Все что я умею описано в '!help'.");
        }
      }
    }
  }
}
