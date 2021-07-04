using System.Threading.Tasks;

namespace Discord.RanSelBot
{
  class Program
  {
    public static Task Main(string[] args)
    {
      return Startup.RunAsync(args);
    }
  }
}
