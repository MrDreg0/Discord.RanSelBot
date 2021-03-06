using System;
using System.Threading.Tasks;
using Discord.Commands;
using Discord.RanSelBot.Services;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Discord.RanSelBot
{
  public class Startup
  {
    public IConfigurationRoot Configuration { get; }

    public Startup(string[] args)
    {
      var builder = new ConfigurationBuilder()
        .SetBasePath(AppContext.BaseDirectory)
        .AddYamlFile("_config.yml");

      Configuration = builder.Build();
    }

    public static async Task RunAsync(string[] args)
    {
      var startup = new Startup(args);
      await startup.RunAsync();
    }

    public async Task RunAsync()
    {
      var services = new ServiceCollection();
      ConfigureServices(services);

      var provider = services.BuildServiceProvider();
      provider.GetRequiredService<LoggingService>();
      provider.GetRequiredService<CommandHandler>();

      await provider.GetRequiredService<StartupService>().StartAsync();
      await Task.Delay(-1);
    }

    private void ConfigureServices(IServiceCollection services)
    {
      services.AddSingleton(new DiscordSocketClient(new DiscordSocketConfig
      {
        LogLevel = LogSeverity.Verbose,
        MessageCacheSize = 1000
      }))
      .AddSingleton(new CommandService(new CommandServiceConfig
      {
        LogLevel = LogSeverity.Verbose,
        DefaultRunMode = RunMode.Async,
      }))
      .AddSingleton<CommandHandler>()
      .AddSingleton<StartupService>()
      .AddSingleton<LoggingService>()
      .AddSingleton<Random>()
      .AddSingleton(Configuration);
    }
  }
}
