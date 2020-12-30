using DSharpPlus;
using DSharpPlus.VoiceNext;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using Itbeard.DiscordBot.Commands;
using Microsoft.Extensions.Logging;

namespace Itbeard.DiscordBot
{
    /// <summary>
    /// Yandex SpeechKin FAQ: https://cloud.yandex.ru/docs/speechkit/quickstart
    /// </summary>
    public static class Program
    {
        static async Task Main()
        {
            // Configure and setup discord client ("DSharpPlus" package)
            var config = new DiscordConfiguration
            {
                Token = Settings.GetInstance().DiscordBotToken,
                TokenType = TokenType.Bot,
                MinimumLogLevel = LogLevel.Debug,
                LogTimestampFormat = "MMM dd yyyy - hh:mm:ss tt"
            };
            var discord = new DiscordClient(config);

            // Register discord custom commands client ("DSharpPlus.CommandsNext" package)
            var commands = discord.UseCommandsNext(new CommandsNextConfiguration()
            {
                UseDefaultCommandHandler = false
            });

            // Initialize custom commands
            commands.RegisterCommands<GeneralCommandsModule>();
            commands.RegisterCommands<TextToSpeechCommandModule>();
            
            // Register discord voice client ("DSharpPlus.VoiceNext" package)
            discord.UseVoiceNext();
            
            // Bind custom commands handler
            discord.MessageCreated += CustomCommandHandler.Execute;

            // Connect bot to discord client
            await discord.ConnectAsync();

            // Prevent console app closing
            await Task.Delay(-1);
        }
    }
}