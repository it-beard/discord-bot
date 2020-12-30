using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.VoiceNext;

namespace Itbeard.DiscordBot.Commands
{
    public class GeneralCommandsModule : BaseCommandModule
    {
        /// <summary>
        /// Method allows bot join to voice channel. Example "-join test-voice-channel-name"
        /// </summary>
        /// <param name="context">Bot context</param>
        /// <param name="channel">Channel name (use "" symbols if channel name consists of several words)</param>
        /// <returns></returns>
        [Command("join")]
        public async Task JoinCommand(CommandContext context, DiscordChannel channel = null)
        {
            channel ??= context.Member.VoiceState?.Channel;
            await channel.ConnectAsync();
        }

        /// <summary>
        /// Method allows kick bot 
        /// </summary>
        /// <param name="context">Bot context</param>
        /// <returns></returns>
        [Command("leave")]
        public async Task LeaveCommand(CommandContext context)
        {
            var vnext = context.Client.GetVoiceNext();
            var connection = vnext.GetConnection(context.Guild);
            connection.Disconnect();
        }
    }
}