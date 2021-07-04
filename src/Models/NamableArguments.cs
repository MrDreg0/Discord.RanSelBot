using System.Collections.Generic;
using System.ComponentModel;
using Discord.Commands;

namespace Discord.RanSelBot.Models
{
  [NamedArgumentType]
  public class NamableArguments
  {
    [Description("роли, которыми должен обладать кандидат.")]
    public IEnumerable<string> Роли { get; set; }

    [Description("кандидаты, которые не дожны участвовать в выборке.")]
    public IEnumerable<string> Исключая { get; set; }
  }
}
