using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.EventArgs;
using Itbeard.DiscordBot.Commands;

namespace Itbeard.DiscordBot
{
    public static class CustomCommandHandler
    {
        public static Task Execute(DiscordClient client, MessageCreateEventArgs e)
        {
            var commandsClient = client.GetCommandsNext();
            var msg = e.Message;

            var cmdStart = msg.GetStringPrefixLength(Settings.GetInstance().CommandsPrefix);
            Command command;
            CommandContext ctx;
            
            // If message start not from prefix - go to TextToSpeech module
            // Else - try to find needed command
            if (cmdStart == -1)
            {
                command = commandsClient.FindCommand(TextToSpeechCommandModule.CommandName, out var args);
                var prefix = Settings.GetInstance().CommandsPrefix;
                if (command == null) return Task.CompletedTask;

                ctx = commandsClient.CreateContext(msg, prefix, command, args);
            }
            else
            {
                var prefix = msg.Content.Substring(0, cmdStart);
                var cmdString = msg.Content.Substring(cmdStart);

                command = commandsClient.FindCommand(cmdString, out var args);
                if (command == null) return Task.CompletedTask;

                ctx = commandsClient.CreateContext(msg, prefix, command, args);
            }
            Task.Run(async () => await commandsClient.ExecuteCommandAsync(ctx));

            return Task.CompletedTask;
        }
    }
}