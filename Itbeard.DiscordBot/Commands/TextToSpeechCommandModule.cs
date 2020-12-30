using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.VoiceNext;

namespace Itbeard.DiscordBot.Commands
{
    public class TextToSpeechCommandModule : BaseCommandModule
    {
        private int MessageSymbolsLenghtLimit = 100;
        private string TtsApiUrl = "https://tts.api.cloud.yandex.net/speech/v1/tts:synthesize";
        public const string CommandName = "textToSpeech";

        /// <summary>
        /// This method triggered each time when message sent to specific chat
        /// </summary>
        /// <param name="context">Bot context</param>
        /// <returns></returns>
        [Command(CommandName)]
        public async Task TextToSpeechCommand(CommandContext context)
        {
            try
            {
                if (context.Message.Channel.Name.EndsWith(Settings.GetInstance().TtsTextChannelName))
                {
                    var message = context.Message.Content;
                    if (message.Length > MessageSymbolsLenghtLimit 
                        || message.Contains('@') // exclude user mentions
                        || message.ToCharArray().Count(c => c == ':') >= 2 // exclude emoji
                        || message.Contains("http") ) // exclude links
                    {
                        return;
                    }
                    
                    var voiceClient = context.Client.GetVoiceNext();
                    var voiceConnection = voiceClient.GetConnection(context.Guild);
                    
                    if (!voiceConnection.IsPlaying)
                    {
                        var transmit = voiceConnection.GetTransmitSink();
                
                        var speechFileName = await ConvertTextToSpeech(message);
                        var pcm = ConvertAudioToPcm(speechFileName);
                        await pcm.CopyToAsync(transmit);
                
                        await pcm.DisposeAsync();
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private Stream ConvertAudioToPcm(string filePath)
        {
            var ffmpeg = Process.Start(new ProcessStartInfo
            {
                FileName = "ffmpeg",
                Arguments = $@"-i ""{filePath}"" -ac 2 -f s16le -ar 48000 pipe:1",
                RedirectStandardOutput = true,
                UseShellExecute = false
            });

            return ffmpeg.StandardOutput.BaseStream;
        }

        private async Task<string> ConvertTextToSpeech(string message)
        {
            var random = new Random();
            var resultSpeechFileName = $"file{random.Next(0,20)}.ogg";
            using var client = new HttpClient();
            using var request = new HttpRequestMessage(HttpMethod.Post, TtsApiUrl);
            var requestPayload = new Dictionary<string, string>
            {
                {"text", message},
                {"lang", "ru-RU"},
                {"voice", "oksana"},
                {"emotion", "good"},
                {"speed", "1"},
                {"folderId", Settings.GetInstance().YandexCloudFolderId}
            };

            request.Headers.Add("Authorization", $"Bearer {Settings.GetInstance().YandexCloudToken}");
            request.Content = new FormUrlEncodedContent(requestPayload);

            using var response = await client.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                await using var responseStream  = await response.Content.ReadAsStreamAsync();
                await using var stream = new FileStream(resultSpeechFileName, FileMode.Create, FileAccess.Write, FileShare.None, 1024, true);
                await responseStream.CopyToAsync(stream);
            }

            return resultSpeechFileName;
        }
    }
}