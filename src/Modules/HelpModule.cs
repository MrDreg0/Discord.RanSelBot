using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord.Commands;
using Discord.RanSelBot.Models;
using Microsoft.Extensions.Configuration;

namespace Discord.RanSelBot.Modules
{
  public class HelpModule : ModuleBase<SocketCommandContext>
  {
    private readonly CommandService _service;
    private readonly IConfigurationRoot _config;

    public HelpModule(CommandService service, IConfigurationRoot config)
    {
      _service = service;
      _config = config;
    }

    [Command("help")]
    public async Task HelpAsync()
    {
      var prefix = _config["prefix"];
      var builder = new EmbedBuilder()
      {
        Color = new Color(114, 137, 218),
        Description = "Список команд, которые понимает бот"
      };

      foreach (var module in _service.Modules)
      {
        string description = null;
        foreach (var cmd in module.Commands)
        {
          var result = await cmd.CheckPreconditionsAsync(Context);
          if (result.IsSuccess)
          {
            description += $"{prefix}{cmd.Aliases.First()}\n";
          }
        }

        if (!string.IsNullOrWhiteSpace(description))
        {
          builder.AddField(x =>
          {
            x.Name = module.Name;
            x.Value = description;
            x.IsInline = false;
          });
        }
      }

      await ReplyAsync("", false, builder.Build());
    }

    [Command("help")]
    public async Task HelpAsync(string command)
    {
      var result = _service.Search(Context, command);

      if (!result.IsSuccess)
      {
        await ReplyAsync($"Я не знаю команды **{command}**.");
        return;
      }

      var builder = new EmbedBuilder()
      {
        Color = new Color(114, 137, 218),
        Description = $"Описание работы команды '**{command}**'"
      };

      foreach (var match in result.Commands)
      {
        var cmd = match.Command;

        builder.AddField(x =>
        {
          x.Name = string.Join(", ", cmd.Aliases);
          x.Value =
            $"Параметры: {GetParameterDescription(cmd.Parameters)}\n" +
            $"Описание: {cmd.Summary}";
          x.IsInline = false;
        });
      }

      await ReplyAsync("", false, builder.Build());
    }

    private string GetParameterDescription(IEnumerable<ParameterInfo> parameters)
    {
      var builder = new StringBuilder();

      builder.AppendLine();

      foreach (var parameter in parameters)
      {
        if (parameter.Type.Name.Equals(nameof(NamableArguments), StringComparison.OrdinalIgnoreCase))
        {
          var properties = typeof(NamableArguments).GetProperties();

          foreach (var property in properties)
          {
            var propertyAttribute = Attribute.GetCustomAttribute(
              typeof(NamableArguments).GetProperty(property.Name),
              typeof(DescriptionAttribute));

            if(propertyAttribute is DescriptionAttribute descriptionAttribute)
            {
              builder.AppendLine(
              $"*{property.Name.ToLowerInvariant()}* - " +
              $"{descriptionAttribute.Description}");
            }
          }
        }
        else
        {
          builder.AppendLine($"*{parameter.Name.ToLowerInvariant()}*");
        }
      }

      return builder.ToString();
    }
  }
}
