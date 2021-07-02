using TwitchLib;
using TwitchLib.Client;
using TwitchLib.Api;
using TwitchLib.Client.Enums;
using TwitchLib.Client.Events;
using TwitchLib.Client.Extensions;
using TwitchLib.Client.Models;

using System;
using System.Diagnostics;

namespace twitchbot
{
    internal class TwitchChatBot
    {
        readonly ConnectionCredentials credentials = new ConnectionCredentials(info.BotUsername, info.BotToken);
        TwitchClient client;
        Stopwatch stopwatch;
        Stopwatch timeAlive;
        TwitchAPI api;
        bool enabled;
        string cChannel;
        bool isAlive;
        string channelid;
        int interval; //in mins

        public TwitchChatBot()
        {
            
        }

        private bool IsStreaming(string id)
        {
            
            return api.Streams.v5.BroadcasterOnlineAsync(id).Result;
        }
 
        //forsens bot
        internal void Connectforsen()
        {
            api = new TwitchAPI();
            api.Settings.ClientId = info.ClientId;
            api.Settings.AccessToken = info.BotToken;
            TwitchLib.Api.Models.v5.Users.User[] users = api.Users.v5.GetUserByNameAsync(info.Channelforsen).Result.Matches;
            channelid = users[0].Id;

            interval = 45;
            enabled = false;
            stopwatch = new Stopwatch();
            Console.WriteLine("connected to forsens chat, starting stopwatch");
            stopwatch.Start();
            timeAlive = new Stopwatch();
            timeAlive.Start();
            client = new TwitchClient();
            this.cChannel = info.Channelforsen;
            client.Initialize(credentials, info.Channelforsen);
            client.OnMessageReceived += Client_OnMessageReceived;
            client.OnConnectionError += Client_OnConnectionError;
            client.OnDisconnected += Client_OnDisconnected;
            client.Connect();
        }

 
        //nymns bot
        internal void Connectnymn()
        {
            api = new TwitchAPI();
            api.Settings.ClientId = info.ClientId;
            api.Settings.AccessToken = info.BotToken;
            TwitchLib.Api.Models.v5.Users.User[] users = api.Users.v5.GetUserByNameAsync(info.Channelnymn).Result.Matches;
            channelid = users[0].Id;

            interval = 60;
            enabled = false;
            stopwatch = new Stopwatch();
            Console.WriteLine("connected to nymns chat, starting stopwatch");
            stopwatch.Start();
            timeAlive = new Stopwatch();
            timeAlive.Start();
            client = new TwitchClient();
            this.cChannel = info.Channelnymn;
            client.Initialize(credentials, info.Channelnymn);
            client.OnMessageReceived += Client_OnMessageReceived;
            client.OnConnectionError += Client_OnConnectionError;
            client.OnDisconnected += Client_OnDisconnected;

            client.Connect();
        }

        //pajs bot
        internal void ConnectPajlada()
        {
            api = new TwitchAPI();
            api.Settings.ClientId = info.ClientId;
            api.Settings.AccessToken = info.BotToken;
            TwitchLib.Api.Models.v5.Users.User[] users = api.Users.v5.GetUserByNameAsync(info.ChannelPajlada).Result.Matches;
            channelid = users[0].Id;

            interval = 30;
            enabled = false;
            stopwatch = new Stopwatch();
            Console.WriteLine("connected to pajladas chat, starting stopwatch");
            stopwatch.Start();
            timeAlive = new Stopwatch();
            timeAlive.Start();
            client = new TwitchClient();
            this.cChannel = info.ChannelPajlada;
            client.Initialize(credentials, info.ChannelPajlada);
            client.OnMessageReceived += Client_OnMessageReceived;
            client.OnConnectionError += Client_OnConnectionError;
            client.OnDisconnected += Client_OnDisconnected;

            client.Connect();
        }
        private void Client_OnMessageReceived(object sender, OnMessageReceivedArgs e)
        {
            if (e.ChatMessage.Message.StartsWith("!status", StringComparison.InvariantCultureIgnoreCase) && ((e.ChatMessage.DisplayName == "hmmmu") || (e.ChatMessage.DisplayName == this.cChannel) || (e.ChatMessage.DisplayName == "pajlada")|| (e.ChatMessage.DisplayName == "noTheOwNeR")))
            {
                if(this.enabled)
                    client.SendMessage(this.cChannel, $"/me OMGScoods Current uptime " + timeAlive.Elapsed.Minutes + " mins. Last message was " + stopwatch.Elapsed.Minutes + " mins ago."+" Currently enabled.");
                else
                    client.SendMessage(this.cChannel, $"/me OMGScoods Current uptime " + timeAlive.Elapsed.Minutes + " mins. Last message was " + stopwatch.Elapsed.Minutes + " mins ago." + " Currently Disabled.");
            }
            if (e.ChatMessage.Message.StartsWith("!enable", StringComparison.InvariantCultureIgnoreCase) && ((e.ChatMessage.DisplayName == "hmmmu") || (e.ChatMessage.DisplayName == this.cChannel) || (e.ChatMessage.DisplayName == "pajlada") || (e.ChatMessage.DisplayName == "noTheOwNeR")))
            {
                this.enabled = true;
                client.SendMessage(this.cChannel, $"/me OMGScoods Now enabled in this chat. FeelsGoodMan");
            }
            if (e.ChatMessage.Message.StartsWith("!disable", StringComparison.InvariantCultureIgnoreCase) && ((e.ChatMessage.DisplayName == "hmmmu") || (e.ChatMessage.DisplayName == this.cChannel) || (e.ChatMessage.DisplayName == "pajlada") || (e.ChatMessage.DisplayName == "noTheOwNeR")))
            {
                this.enabled = false;
                client.SendMessage(this.cChannel, $"/me OMGScoods Now disabled in this chat. FeelsBadMan");
            }

            if ((stopwatch.Elapsed.Minutes >= interval) && this.enabled)
            {
                if (IsStreaming(channelid))
                {
                    client.SendMessage(this.cChannel, $"/me : OMGScoods 👆 Friendly reminder to check your posture FeelsOkayMan");
                }
                stopwatch.Restart();
            }
        }
        private void Client_OnWhisperReceived(object sender, OnWhisperReceivedArgs e)
        {
            client.SendWhisper("hmmmu", $"{e.WhisperMessage.Username} said: {e.WhisperMessage.Message}");
        }
        private void Client_OnDisconnected(object sender, OnDisconnectedArgs e)
        {
            client.Reconnect();
            timeAlive.Restart();
        }
        private void Client_OnConnectionError(object sender, OnConnectionErrorArgs e)
        {
            Console.WriteLine($"Error!!! {e.Error}");
        }

        internal void Disconnect()
        {
            Console.WriteLine("Disconnecting");
        }

    }

}