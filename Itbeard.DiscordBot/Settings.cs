using System;

namespace Itbeard.DiscordBot
{
    public class Settings
    {
        private static readonly Settings instance = new Settings();
 
        /// <summary>
        /// Discord bot token
        /// </summary>
        public string DiscordBotToken { get; private set; }

        /// <summary>
        /// How to get Token: https://cloud.yandex.ru/docs/iam/operations/iam-token/create
        /// </summary>
        public string YandexCloudToken { get; private set; }

        /// <summary>
        /// YC Computing folder ID: https://cloud.yandex.ru/docs/resource-manager/operations/folder/get-id
        /// </summary>
        public string YandexCloudFolderId { get; private set; }

        public string CommandsPrefix { get; private set; }

        public string TtsTextChannelName { get; private set; }
 
        private Settings()
        {
            DiscordBotToken = Environment.GetEnvironmentVariable("ITBEARD_DISCORDBOT_TOKEN");
            YandexCloudToken = Environment.GetEnvironmentVariable("ITBEARD_YC_TOKEN");
            YandexCloudFolderId = Environment.GetEnvironmentVariable("ITBEARD_YC_FOLDERID");
            CommandsPrefix = "-";
            TtsTextChannelName = "танцпол";
        }
 
        public static Settings GetInstance()
        {
            return instance;
        }
    }
}