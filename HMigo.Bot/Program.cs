using Microsoft.Extensions.Configuration;
using System;

namespace HMigo.Bot
{
    class Program
    {
        static void Main(string[] args)
        {
            IConfiguration Configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            .AddEnvironmentVariables()
            .AddCommandLine(args)
            .Build();

            var twitchSettings = new TwitchSettings
            {
                Channel = Configuration["TwitchSettings:Channel"],
                BotName = Configuration["TwitchSettings:BotName"],
                AccessToken = Configuration["TwitchSettings:AccessToken"]
            };

            var bot = new Bot(twitchSettings);
            Console.ReadLine();
        }
    }
}
