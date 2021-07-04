using System;
using System.Reflection;
using System.Threading.Tasks;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;

namespace Discord.RanSelBot.Services
{
  public class StartupService
  {
    private readonly IServiceProvider _provider;
    private readonly DiscordSocketClient _discord;
    private readonly CommandService _commands;
    private readonly IConfigurationRoot _config;

    public StartupService(
      IServiceProvider provider,
      DiscordSocketClient discord,
      CommandService commands,
      IConfigurationRoot config)
    {
      _provider = provider;
      _config = config;
      _discord = discord;
      _commands = commands;
    }

    public async Task StartAsync()
    {
      var discordToken = _config["tokens:discord"];

      if (string.IsNullOrWhiteSpace(discordToken))
      {
        discordToken = Environment.GetEnvironmentVariable("tokens__discord");

        if (string.IsNullOrWhiteSpace(discordToken))
        {
          throw new Exception("Необходимо заполнить параметр 'token:discord' в файле конфигурации.");
        }
      }

      await _discord.LoginAsync(TokenType.Bot, discordToken);
      await _discord.StartAsync();

      await _commands.AddModulesAsync(Assembly.GetEntryAssembly(), _provider);
    }
  }
}
