using Hmigo.Bot.Game;
using Hmigo.Bot.Game.Models;
using HMigo.Bot.Constants;
using System;
using TwitchLib.Client;
using TwitchLib.Client.Enums;
using TwitchLib.Client.Events;
using TwitchLib.Client.Extensions;
using TwitchLib.Client.Models;
using TwitchLib.Communication.Clients;
using TwitchLib.Communication.Models;

namespace HMigo.Bot
{
    public class Bot
    {
        readonly TwitchClient client;
        readonly GameManager gameManager;
        Data gameData;

        public Bot(TwitchSettings twitchSettings)
        {
            var credential = new ConnectionCredentials(twitchSettings.BotName, twitchSettings.AccessToken);

            var clientOptions = new ClientOptions
            {
                MessagesAllowedInPeriod = 100,
                ThrottlingPeriod = TimeSpan.FromSeconds(30)
            };

            var customClient = new WebSocketClient(clientOptions);

            // Need to setup game
            gameManager = new GameManager();
            gameData = gameManager.SetupGame();

            if (gameData == null)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Could not load game data");
                Console.ForegroundColor = ConsoleColor.White;
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Loaded game data");
                Console.ForegroundColor = ConsoleColor.White;
            }

            client = new TwitchClient(customClient);
            client.Initialize(credential, twitchSettings.Channel);

            client.OnLog += Client_OnLog;
            client.OnJoinedChannel += Client_OnJoinedChannel;
            client.OnMessageReceived += Client_OnMessageReceived;
            client.OnWhisperReceived += Client_OnWhisperReceived;
            client.OnNewSubscriber += Client_OnNewSubscriber;
            client.OnConnected += Client_OnConnected;

            client.Connect();
        }

        private void Client_OnLog(object sender, OnLogArgs e)
        {
            Console.WriteLine($"{e.DateTime}: {e.BotUsername} - {e.Data}");
        }

        private void Client_OnConnected(object sender, OnConnectedArgs e)
        {
            Console.WriteLine($"Connected to {e.AutoJoinChannel}");
        }

        private void Client_OnJoinedChannel(object sender, OnJoinedChannelArgs e)
        {
            client.SendMessage(e.Channel, "Hey guys! I am a bot connected via TwitchLib!");
        }

        private void Client_OnMessageReceived(object sender, OnMessageReceivedArgs e)
        {
            CheckAndRunCommands(e);
        }

        private void CheckAndRunCommands(OnMessageReceivedArgs e)
        {
            var commandLetter = e.ChatMessage.Message.ToLowerInvariant()[0];

            if (commandLetter == '!')
            {
                var command = e.ChatMessage.Message.Remove(0, 1);
                switch (command)
                {
                    case BotCommands.AboutBot:
                        AboutBotCommand(e);
                        break;
                    case BotCommands.ClearChat:
                        ClearChat(e);
                        break;
                    case BotCommands.JoinGame:
                        JoinGame(e);
                        break;
                }
            }
        }

        private void JoinGame(OnMessageReceivedArgs e)
        {
            var message = gameManager.JoinGame(e.ChatMessage.Username, gameData: ref gameData);
            client.SendMessage(e.ChatMessage.Channel, $"@{message}");
        }

        private void ClearChat(OnMessageReceivedArgs e)
        {
            if (e.ChatMessage.IsModerator || e.ChatMessage.IsBroadcaster)
            {
                Console.WriteLine("Clearing Chat...");
                client.ClearChat(e.ChatMessage.Channel);
            }
        }

        private void AboutBotCommand(OnMessageReceivedArgs e)
        {
            Console.WriteLine($"{e.ChatMessage.Username} asked about the bot...");
            client.SendMessage(e.ChatMessage.Channel, $"@{e.ChatMessage.Username} I am running on a Raspberry Pi using .NET 6! How sweet is that!");
        }

        private void Client_OnWhisperReceived(object sender, OnWhisperReceivedArgs e)
        {
            if (e.WhisperMessage.Username == "hmigo")
            {
                client.SendWhisper(e.WhisperMessage.Username, "Hey! Whispers are so cool!!");
            }
        }

        private void Client_OnNewSubscriber(object sender, OnNewSubscriberArgs e)
        {
            if (e.Subscriber.SubscriptionPlan == SubscriptionPlan.Prime)
            {
                client.SendMessage(e.Channel, $"Welcome {e.Subscriber.DisplayName} to the Amigos! You just earned 500 points! So kind of you to use your Twitch Prime on this channel!");
            }
            else
            {
                client.SendMessage(e.Channel, $"Welcome {e.Subscriber.DisplayName} to the Amigos! You just earned 500 points!");
            }
        }
    }
}
