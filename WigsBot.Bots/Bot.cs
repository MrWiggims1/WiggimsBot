using DSharpPlus;
using System.Threading.Tasks;
using DSharpPlus.Interactivity;
using System;
using System.Collections.Generic;
using System.Text;
using DSharpPlus.EventArgs;
using System.IO;
using DSharpPlus.CommandsNext;
using Newtonsoft.Json;
using WigsBot.Bot.Commands;
using DSharpPlus.CommandsNext.Exceptions;
using DSharpPlus.Entities;
using DSharpPlus.CommandsNext.Converters;
using System.Threading;
using WeCantSpell.Hunspell;
using WigsBot.DAL.Models.Profiles;
using WigsBot.Core.Services.Profiles;
using WigsBot.Core.ViewModels;
using System.Text.RegularExpressions;
using System.Net.Http;
using System.Linq;
using WigsBot.Bot.Commands.Stats;
using WigsBot.Bot.Commands.Profilecommands;
using DSharpPlus.VoiceNext;
using DSharpPlus.VoiceNext.Codec;
using WigsBot.Bot.Commands.Item;
using DSharpPlus.CommandsNext.Attributes;

namespace WigsBot.Bot
{
    public class Bot
    {
        public DiscordClient Client { get; private set; }
        public CommandsNextExtension Commands { get; private set; }
        public DiscordMember DiscordMember { get; private set; }
        public VoiceNextExtension Voice { get; private set; }
        public Bot(IServiceProvider services)

        {
            var json = string.Empty;

            using (var fs = File.OpenRead("config.json"))
            using (var sr = new StreamReader(fs, new UTF8Encoding(false)))
                json = sr.ReadToEnd();

            var configJson = JsonConvert.DeserializeObject<ConfigJson>(json);

            var config = new DiscordConfiguration
            {
                Token = configJson.Token,
                TokenType = TokenType.Bot,

                AutoReconnect = true,
                LogLevel = LogLevel.Info,
                UseInternalLogHandler = true
            };

            Client = new DiscordClient(config);

            this.Client.Ready += OnClientReady;
            this.Client.GuildAvailable += this.Client_GuildAvailable;
            this.Client.ClientErrored += this.Client_ClientError;

            Client.UseInteractivity(new InteractivityConfiguration
            {
                Timeout = TimeSpan.FromSeconds(60)
            });

            var commandsConfig = new CommandsNextConfiguration
            {
                StringPrefixes = configJson.Prefix,
                EnableDms = false,
                EnableMentionPrefix = true,
                Services = services
                //UseDefaultCommandHandler = false
            };

            Commands = this.Client.UseCommandsNext(commandsConfig);
            //this.Commands.SetHelpFormatter<WiggimsBotHelpFormatter>();    

            this.Commands.CommandExecuted += this.Commands_CommandExecuted;
            this.Commands.CommandErrored += this.Commands_CommandErrored;


            Commands.RegisterCommands<FunCommands>();
            Commands.RegisterCommands<ManagementCommands>();
            Commands.RegisterCommands<tools>();
            Commands.RegisterCommands<TestCommands>();
            //Commands.RegisterCommands<InfoCommands>();
            Commands.RegisterCommands<adminCommands>();
            Commands.RegisterCommands<minecraft>();
            Commands.RegisterCommands<ProfileCommands>();
            Commands.RegisterCommands<ExtendedProfileCommands>();
            Commands.RegisterCommands<AdminProfileCommands>();
            Commands.RegisterCommands<GoldCommands>();
            Commands.RegisterCommands<CrimeCommands>();
            Commands.RegisterCommands<ItemCommands>();
            Commands.RegisterCommands<memes>();
            Commands.RegisterCommands<GambleCommands>();
            Commands.RegisterCommands<csgo>();
            Commands.RegisterCommands<Beatsaber>();
            Commands.RegisterCommands<Siege>();
            Commands.RegisterCommands<TodoCommands>();
            Commands.RegisterCommands<VoiceCommands>();
            //Commands.RegisterCommands<SiegeGG>();
            Commands.RegisterCommands<VersionCommands>();

            var VoiceConfig = new VoiceNextConfiguration
            {
                AudioFormat = AudioFormat.Default,
                EnableIncoming = false
            };

            this.Voice = this.Client.UseVoiceNext(VoiceConfig);


            Client.ConnectAsync();

            Client.UpdateStatusAsync(new DiscordActivity("w!help or w@help."), UserStatus.Online, null);

            Client.GuildMemberAdded += async e =>
            {
                if (e.Member.IsBot) { return; }

                if (e.Member.Id == 205291283076349952 || e.Member.Id == 419398092823986177)
                {
                    await e.Client.SendMessageAsync(e.Guild.SystemChannel, $"luv u {e.Member.Mention} :heart:").ConfigureAwait(false);

                    var cmd = this.Commands.FindCommand("addtacoleave", out var args);
                    var ctx = this.Commands.CreateFakeContext(e.Member, e.Guild.SystemChannel, "not null", "w!", cmd);
                    await this.Commands.ExecuteCommandAsync(ctx);
                }
                else
                {
                    await Client.SendMessageAsync(e.Guild.SystemChannel, $"Welcome {e.Member.Mention}. Type 'w!role join' in the bot commands channel to and choose what games you'll like to receive pings for.").ConfigureAwait(false);
                }
            };

            Client.MessageCreated += async e =>
            {
                await Spelling_message(e);

                if (e.Message.MentionEveryone)
                {
                    if (e.Message.Author.Id != 121560374943285251 && e.Message.Author.Id != 318999364192174080 && e.Message.Author.Id != 232067231209619469 && !e.Message.Author.IsBot)
                    {
                        await e.Message.DeleteAsync();
                        await e.Channel.SendMessageAsync("Please don't @every one in the server. Instead go to the appropriate channel, then ping the group relevant to what you need to say. Just because you don't mind, doesn't mean others don't mind too.");
                    }
                }

                if (e.Message.Content.ToLower().StartsWith("w1"))
                    await e.Message.RespondAsync("Hehehe");

                if (e.Message.Author.Id == 629312237575733260)
                {
                    var prevMessage = (await e.Channel.GetMessagesBeforeAsync(e.Message.Id, 1).ConfigureAwait(false)).First();

                    if (!prevMessage.Author.IsBot)
                    {
                        try
                        {
                            var cmd = this.Commands.FindCommand("addgot", out var args);
                            var ctx = this.Commands.CreateContext(e.Message, "w!", cmd, $"1 {prevMessage.Author.Id}");
                            await this.Commands.ExecuteCommandAsync(ctx);

                            Console.WriteLine($"father bot added a got to {prevMessage.Author.Username}");
                        }
                        catch { Console.WriteLine($"father bot missed a got {prevMessage.Author.Username}"); }
                    }
                }

                if (e.Message.Author.Id == 503720029456695306)
                {
                    if (e.Message.Content.Contains("Dad") || e.Message.Content.Contains("Listen here ") || e.Message.Content.Contains("instead, take your own advice."))
                    {
                        if (e.Message.Content.Length > 1000)
                            return;

                        var prevMessage = (await e.Channel.GetMessagesBeforeAsync(e.Message.Id, 1).ConfigureAwait(false)).First();
                        DiscordUser discordUser = await this.Client.GetUserAsync(503720029456695306);
                        var cmd = this.Commands.FindCommand("AddGotautomaticgot", out var args);
                        //var fctx = this.Commands.CreateFakeContext(discordUser, e.Channel, $"{prevMessage.Author.Id} 1", "w!", cmd, $"{prevMessage.Author.Id} 1");
                        var ctx = this.Commands.CreateContext(e.Message, "w!", cmd, $"{prevMessage.Author.Id} 1");
                        await this.Commands.ExecuteCommandAsync(ctx);

                        Console.WriteLine($"dad bot added a got to {prevMessage.Author.Username}");
                    }
                }
            };
        }

        // let us know the client is ready
        private Task OnClientReady(ReadyEventArgs e)
        {
            e.Client.DebugLogger.LogMessage(LogLevel.Info, "WigsBot", "Client is ready to process events.", DateTime.Now);
            return Task.CompletedTask;
        }

        // list guilds connected to the bot
        private Task Client_GuildAvailable(GuildCreateEventArgs e)
        {
            e.Client.DebugLogger.LogMessage(LogLevel.Info, "WigsBot", $"Guild available: {e.Guild.Name}", DateTime.Now);
            return Task.CompletedTask;
        }

        // let us know if there were any errors while starting the client
        private Task Client_ClientError(ClientErrorEventArgs e)
        {
            e.Client.DebugLogger.LogMessage(LogLevel.Error, "WigsBot", $"Exception occurred: {e.Exception.GetType()}: {e.Exception.Message}", DateTime.Now);
            return Task.CompletedTask;
        }

        private async Task Spelling_message(MessageCreateEventArgs e)
        {
            if (e.Message.Author.IsBot) { return; }
            var cmd = this.Commands.FindCommand("WiggimsBotSpell", out var args);
            var ctx = this.Commands.CreateContext(e.Message, "w!", cmd);
            await this.Commands.ExecuteCommandAsync(ctx);
        }

        // let us know if and how a command has been executed
        private Task Commands_CommandExecuted(CommandExecutionEventArgs e)
        {
            e.Context.Client.DebugLogger.LogMessage(LogLevel.Info, "WigsBot", $"{e.Context.User.Username} successfully executed within the {e.Context.Guild.Name}. '{e.Command.QualifiedName}'", DateTime.Now);
            return Task.CompletedTask;
        }


        // let us know if a command has errored
        private async Task Commands_CommandErrored(CommandErrorEventArgs e)
        {
            var emoji = DiscordEmoji.FromName(e.Context.Client, ":no_entry:");
            e.Context.Client.DebugLogger.LogMessage(LogLevel.Error, "WigsBot", $"{e.Context.User.Username} tried executing within the {e.Context.Guild.Name} in {e.Context.Channel.Name} {e.Context.Channel.Type} channel. '{e.Command?.QualifiedName ?? "<unknown command>"}' but it errored: {e.Exception.GetType()}: {e.Exception.Message ?? "<no message>"}", DateTime.Now);

            if (e.Exception is ChecksFailedException ex)
            {
                if (e.Command.Name == "earnxp" || e.Command.Name == "wiggimsbotspell") { return; }

                if (e.Command.Name == "fish")
                {
                    await e.Context.Channel.SendMessageAsync($"{e.Context.Member.Username} please wait 60 seconds between each cast.");
                    return;
                }

                if (e.Command.Name == "analyzevaults")
                {
                    await e.Context.Channel.SendMessageAsync($"Seems the code for this command is incomplete perhaps there's a way of obtaining it.");
                    return;
                }

                if (e.Command.Name == "mimic")
                {
                    await e.Context.Channel.SendMessageAsync("For some reason streaming audio gets messy if you request it too quick. Globally limited to 5 per minute.");
                    return;
                }

                var embed = new DiscordEmbedBuilder
                {
                    Title = "Access denied",
                    Description = $"{emoji} You can not execute this command. If you believe this is incorrect please let Wiggims know.\n\nYou failed one of the following checks:\n - {String.Join("\n - ",ex.Command.ExecutionChecks)}",
                    Color = new DiscordColor(0xFF0000), // red
                    Timestamp = System.DateTime.Now
                };
                
                await e.Context.RespondAsync("", embed: embed);
            } 

            else if (e.Exception is CommandNotFoundException)
            {
                var embed = new DiscordEmbedBuilder
                {
                    Title = "Command Not Found",
                    Description = "Please make sure used the correct spelling, or make sure the command you are trying to use exists.",
                    Color = new DiscordColor(0xFF0000), // red
                    Timestamp = System.DateTime.Now
                };
                await e.Context.RespondAsync("", embed: embed);
            }

            else if (e.Exception.Message.Contains("Could not find a suitable overload for the command.")) //if arguments are entered incorrectly
            {
                var embed = new DiscordEmbedBuilder
                {
                    Title = "Invalid arguments",
                    Description = $"The arguments sent with the command were not valid, try `w!help {e.Command.QualifiedName}` for help. If you feel the explanation for the command is not well written please let wiggims know and he will help.",
                    Color = new DiscordColor(0xFF0000), // red
                    Timestamp = System.DateTime.Now
                };
                await e.Context.RespondAsync("", embed: embed);
            }

            else if (e.Exception.Message.Contains("No matching subcommands were found, and this group is not executable.")) // if command group is called which is not executable
            {
                var embed = new DiscordEmbedBuilder
                {
                    Title = "Invalid command use",
                    Description = $"This command represents a group of commands, please try `w!help {e.Command.QualifiedName}` to see a list of valid sub commands and use `w!{e.Command.QualifiedName} <subcommand>` to execute it.",
                    Color = new DiscordColor(0xFF0000), // red
                    Timestamp = System.DateTime.Now
                };
                await e.Context.RespondAsync("", embed: embed);
            }

            else if (e.Exception is FileNotFoundException)
            {
                var embed = new DiscordEmbedBuilder
                {
                    Title = $"This file could not be found, please make sure you have specified the correct file path.",
                    Color = new DiscordColor(0xFF0000), // red
                    Timestamp = System.DateTime.Now
                };

                await e.Context.RespondAsync("", embed: embed);
            }

            else if (e.Exception is DSharpPlus.Exceptions.BadRequestException)
            {
                var embed = new DiscordEmbedBuilder
                {
                    Title = $"Bad Request Exception thrown, please make sure you're not referring to something from another discord.",
                    Color = new DiscordColor(0xFF0000), // red
                    Timestamp = System.DateTime.Now
                };

                embed.AddField("Exception message:", e.Exception.Message);

                await e.Context.RespondAsync("", embed: embed);
            }

            else
            {

                try
                {
                    var embed = new DiscordEmbedBuilder
                    {
                        Title = "Unknown Error",
                        Description = $"{e.Exception.Message}",
                        Color = new DiscordColor(0xFF0000), // red
                        Timestamp = System.DateTime.Now
                    };
                    embed.AddField("exception type:", $"{e.Exception.GetType()}" );
                    await e.Context.RespondAsync("", embed: embed);
                }
                catch
                {                    
                    var embed = new DiscordEmbedBuilder
                    {
                        Title = "Unknown Error",
                        Description = $"Error too large to send \n \n<@318999364192174080> please check. \nPlease don't delete this error message as Wiggims can use this to provide more helpful errors in the future (or just figure wtf he did wrong).",
                        Color = new DiscordColor(0xFF0000), // red
                        Timestamp = System.DateTime.Now
                    };
                    embed.AddField("exception type:", $"{e.Exception.GetType()}");
                    await e.Context.RespondAsync("", embed: embed);
                }
            }
        }
    }
}
