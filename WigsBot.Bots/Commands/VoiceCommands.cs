using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;
using DSharpPlus.VoiceNext;
using System.Diagnostics;
using WigsBot.Core.Services.Profiles;
using WigsBot.DAL.Models.Profiles;
using Microsoft.EntityFrameworkCore.Internal;
using Google.Apis.YouTube.v3;
using Google.Apis.Services;
using YoutubeExplode;
using YoutubeExplode.Videos.Streams;
using Google.Apis.YouTube.v3.Data;
using DSharpPlus.Interactivity;
using DSharpPlus.VoiceNext.EventArgs;
using System.Collections.Concurrent;

namespace WigsBot.Bot.Commands
{
    public class VoiceCommands : BaseCommandModule
    {
        private readonly IProfileService _profileService;

        public VoiceCommands(
                IProfileService profileService
                )
        {
            _profileService = profileService;
        }

        //private ConcurrentDictionary<uint, Process> ffmpegs;

        [Command("join")]
        [RequirePrefixes("w!", "W!")]
        [Description("Connects bot to call.")]
        [Aliases("connectvc")]
        public async Task joinsound(CommandContext ctx)
        {
            await JoinVC(ctx);
        }

        [Command("disconnect")]
        [RequirePrefixes("w!", "W!")]
        [Description("Disconnect bot from voice chat.")]
        public async Task leavesound(CommandContext ctx)
        {
            LeaveVCAsync(ctx);
        }

        [Command("playfile")]
        [RequirePrefixes("w!", "W!")]
        [Description("Plays an audio file.")]
        [RequireOwner]
        public async Task play(CommandContext ctx, [RemainingText, Description("The file you want to play.")] string filepath)
        {
            if (filepath.Contains("Mimics") || filepath.Contains("mimics"))
            {
                await ctx.Channel.SendMessageAsync($"Please use the `w!mimic` command, some users do not want to mimicked and you cannot use this command to get around that, if you are trying to mimic a user who is no longer on the server, a fix for that will be coming soon.");
                return;
            }

            string file = $"Resources/Sound/{ filepath }";
            var VoiceClient = ctx.Client.GetVoiceNext();

            var VoiceConnection = VoiceClient.GetConnection(ctx.Guild);
            if (VoiceConnection == null)
            {
                var VoiceChannel = ctx.Member?.VoiceState?.Channel;
                if (VoiceChannel == null)
                {
                    await ctx.Channel.SendMessageAsync("You need to be in a voice channel.");
                    return;
                }

                await VoiceClient.ConnectAsync(VoiceChannel);
                VoiceConnection = VoiceClient.GetConnection(ctx.Guild);
            }

            if (!File.Exists(file))
            {
                await ctx.Channel.SendMessageAsync("This file was not found.");
                return;
            }

            var psi = new ProcessStartInfo
            {
                FileName = "ffmpeg",
                Arguments = $@"-i ""{file}"" -ac 2 -f s16le -ar 48000 pipe:1",
                RedirectStandardOutput = true,
                UseShellExecute = false
            };

            var ffmpeg = Process.Start(psi);
            var ffout = ffmpeg.StandardOutput.BaseStream;

            ffout.CopyTo(VoiceConnection.GetTransmitStream());

            await VoiceConnection.SendSpeakingAsync(false); // we're not speaking anymore
        }

        [Command("makefunofspelling")]
        [RequirePrefixes("w!", "W!")]
        [Description("Made this command at 5am don't judge.")]
        [Cooldown(1, 10, CooldownBucketType.User)]
        public async Task makefunofspelling(CommandContext ctx)
        {
            await ctx.Message.DeleteAsync().ConfigureAwait(false);


            string file = $"Resources/Sound/Music/WeirdAl/WordCrimes.mp3";
            var VoiceClient = ctx.Client.GetVoiceNext();

            var VoiceConnection = VoiceClient.GetConnection(ctx.Guild);
            if (VoiceConnection == null)
            {
                var VoiceChannel = ctx.Member?.VoiceState?.Channel;
                if (VoiceChannel == null)
                {
                    await ctx.Channel.SendMessageAsync("You need to be in a voice channel.");
                    return;
                }

                await VoiceClient.ConnectAsync(VoiceChannel);
                VoiceConnection = VoiceClient.GetConnection(ctx.Guild);
            }

            await ctx.Channel.SendMessageAsync("I hate these woooooord criiiiiimesss!");

            var psi = new ProcessStartInfo
            {
                FileName = "ffmpeg",
                Arguments = $@"-i ""{file}"" -ac 2 -f s16le -ar 48000 pipe:1",
                RedirectStandardOutput = true,
                UseShellExecute = false
            };

            var ffmpeg = Process.Start(psi);
            var ffout = ffmpeg.StandardOutput.BaseStream;

            ffout.CopyTo(VoiceConnection.GetTransmitStream());

            await VoiceConnection.SendSpeakingAsync(false); // we're not speaking anymore
        }

        [Group("mimic")]
        [Description("Impersonates a user through a voice channel.")]
        public class MimicCommands : BaseCommandModule
        {
            private readonly IProfileService _profileService;
            private readonly IMimicableService _mimicableService;

            public MimicCommands(
                IProfileService profileService,
                IMimicableService mimicableService
                )
            {
                _profileService = profileService;
                _mimicableService = mimicableService;
            }

            [GroupCommand]
            [RequirePrefixes("w!", "W!")]
            [Cooldown(5, 60, CooldownBucketType.Global)]
            public async Task mimic(CommandContext ctx, [Description("Mention or use member Id.")] DiscordMember discordMember, [Description("Mimic Id you wish to play")] int Id = 50000)
            {
                await ctx.Message.DeleteAsync().ConfigureAwait(false);

                Profile profile = await _profileService.GetOrCreateProfileAsync(discordMember.Id, ctx.Guild.Id);

                if (!profile.IsMimicable)
                {
                    await ctx.Channel.SendMessageAsync($"{discordMember.Username} does not wish to be mimicked, need to find a new way to bully them.");
                    return;
                }

                try
                {
                    Directory.GetFiles($"Resources/Sound/Mimics/{discordMember.Id}/", "*");
                }
                catch
                {
                    await ctx.Channel.SendMessageAsync("This user does not have any mimics yet. If you have a sound clip that you want to add to the mimics list, save it as any sound file, and name it by the users Id, then send it too Wiggims if you don't know how to get the Id, Google it.");
                    return;
                }

                var mimicFilePaths = Directory.GetFiles($"Resources/Sound/Mimics/{discordMember.Id}/", "*");

                if (Id == 50000)
                {
                    Id = new Random().Next(0, mimicFilePaths.Length);
                }

                string file = mimicFilePaths[Id];
                var VoiceClient = ctx.Client.GetVoiceNext();

                var VoiceConnection = VoiceClient.GetConnection(ctx.Guild);
                if (VoiceConnection == null)
                {
                    var VoiceChannel = ctx.Member?.VoiceState?.Channel;
                    if (VoiceChannel == null)
                    {
                        await ctx.Channel.SendMessageAsync("You need to be in a voice channel.");
                        return;
                    }

                    await VoiceClient.ConnectAsync(VoiceChannel);
                    VoiceConnection = VoiceClient.GetConnection(ctx.Guild);
                }

                if (!File.Exists(file))
                {
                    await ctx.Channel.SendMessageAsync("This mimic does not exist.");
                    return;
                }

                await ctx.Channel.SendMessageAsync($"Mimicking {discordMember.Username} (Mimic Id: {Id}, Mimic Name: {file.Remove(0, 42).Replace(".mp3", " ")}), if they don't want people to have the ability to do this, they can use `W!Profile ToggleMimicking`.");
                await _mimicableService.TackAMimic(profile);

                var psi = new ProcessStartInfo
                {
                    FileName = "ffmpeg",
                    Arguments = $@"-i ""{file}"" -ac 2 -f s16le -ar 48000 pipe:1",
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                };

                var ffmpeg = Process.Start(psi);
                var ffout = ffmpeg.StandardOutput.BaseStream;

                ffout.CopyTo(VoiceConnection.GetTransmitStream());

                await VoiceConnection.SendSpeakingAsync(false); // we're not speaking anymore            

                await VoiceConnection.WaitForPlaybackFinishAsync();

                VoiceConnection.Disconnect();
            }

            [GroupCommand]
            [RequirePrefixes("w!", "W!")]
            public async Task mimic(CommandContext ctx, [Description("Mention or use member Id.")] DiscordMember discordMember, [Description("Mimic you wish to play"), RemainingText] string mimicName)
            {
                await ctx.Message.DeleteAsync().ConfigureAwait(false);

                Profile profile = await _profileService.GetOrCreateProfileAsync(discordMember.Id, ctx.Guild.Id);

                if (!profile.IsMimicable)
                {
                    await ctx.Channel.SendMessageAsync($"{discordMember.Username} does not wish to be mimicked, need to find a new way to bully them.");
                    return;
                }

                try
                {
                    Directory.GetFiles($"Resources/Sound/Mimics/{discordMember.Id}/", "*");
                }
                catch
                {
                    await ctx.Channel.SendMessageAsync("This user does not have any mimics yet. If you have a sound clip that you want to add to the mimics list, save it as any sound file, and name it by the users Id, then send it too Wiggims if you don't know how to get the Id, Google it.");
                    return;
                }

                string filePath = $"Resources/Sound/Mimics/{discordMember.Id}/{mimicName}.mp3";

                var VoiceClient = ctx.Client.GetVoiceNext();

                var VoiceConnection = VoiceClient.GetConnection(ctx.Guild);
                if (VoiceConnection == null)
                {
                    var VoiceChannel = ctx.Member?.VoiceState?.Channel;
                    if (VoiceChannel == null)
                    {
                        await ctx.Channel.SendMessageAsync("You need to be in a voice channel.");
                        return;
                    }

                    await VoiceClient.ConnectAsync(VoiceChannel);
                    VoiceConnection = VoiceClient.GetConnection(ctx.Guild);
                }

                if (!File.Exists(filePath))
                {
                    await ctx.Channel.SendMessageAsync("This mimic does not exist, please make sure you have typed in the correct name, or use the Id.");
                    return;
                }

                await ctx.Channel.SendMessageAsync($"Mimicking {discordMember.Username}, if they don't want people to have the ability to do this, they can use `W!Profile ToggleMimicking`.");
                await _mimicableService.TackAMimic(profile);

                var psi = new ProcessStartInfo
                {
                    FileName = "ffmpeg",
                    Arguments = $@"-i ""{filePath}"" -ac 2 -f s16le -ar 48000 pipe:1",
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                };

                var ffmpeg = Process.Start(psi);
                var ffout = ffmpeg.StandardOutput.BaseStream;

                ffout.CopyTo(VoiceConnection.GetTransmitStream());

                await VoiceConnection.SendSpeakingAsync(false); // we're not speaking anymore            

                await VoiceConnection.WaitForPlaybackFinishAsync();

                VoiceConnection.Disconnect();
            }

            [Command("list")]
            [RequirePrefixes("w!", "W!")]
            [Description("Shows a list of users that can be mimicked.")]
            public async Task Mimiclist(CommandContext ctx)
            {
                await ctx.Message.DeleteAsync().ConfigureAwait(false);

                var fileCount = Directory.EnumerateDirectories($"Resources/Sound/Mimics/").ToList();
                StringBuilder sb = new StringBuilder();
                foreach (var thing in fileCount)
                {
                    Console.WriteLine(thing);
                    var thing2 = thing.Remove(0, 23);
                    try
                    {
                        var member = await ctx.Client.GetUserAsync(ulong.Parse(thing2));
                        int mimicCount = Directory.GetFiles($"Resources/Sound/Mimics/{ member.Id }/", "*", SearchOption.AllDirectories).Length;
                        if (mimicCount < 2)
                            sb.Append($"- {member.Username} has {mimicCount} mimic\n");
                        else
                            sb.Append($"- {member.Username} has {mimicCount} mimics\n");
                    }
                    catch { }
                }

                await ctx.Channel.SendMessageAsync($"Users that can be mimicked:\n{sb}\nTo see more information on someones mimics use `w!mimic list <Member>`\nIf you have a sound clip that you want to add to the mimics list, save it as an mp3 and use the `w!mimic submit <discord member> <mimic file>` command.");
            }

            [Command("list")]
            [RequirePrefixes("w!", "W!")]
            public async Task Mimiclist(CommandContext ctx, [Description("The member who's mimic list you would like to see")] DiscordMember member)
            {
                var files = Directory.GetFiles($"Resources/Sound/Mimics/{member.Id}/", "*");

                StringBuilder sb = new StringBuilder();

                foreach (var file in files)
                {
                    sb.Append($"- {file.Remove(0, 42).Replace(".mp3", " ")} - Id: {files.IndexOf(file)}\n");
                }

                await ctx.Channel.SendMessageAsync($"Mimics for {member.Username}\n{sb}");
            }

            [Command("rename")]
            [RequirePrefixes("w!", "W!")]
            [Description("Rename one of your mimics.")]
            public async Task RenameMimic(CommandContext ctx, [Description("mimic Id")] int Id, [Description("New name."), RemainingText] string newName)
            {
                var files = Directory.GetFiles($"Resources/Sound/Mimics/{ctx.Member.Id}/", "*");
                string fileToRename = files[Id];
                string newFilePath = fileToRename.Replace(fileToRename.Remove(0, 42), newName + ".mp3");
                File.Move(fileToRename, newFilePath);

                await ctx.Channel.SendMessageAsync("your mimic has been renamed, please keep in mind this may have affected mimic Id's.");
            }

            [Command("Delete")]
            [RequirePrefixes("w!", "W!")]
            [Description("Delete a mimic of yours you don't want available.")]
            public async Task deletemimic(CommandContext ctx, [Description("Mimic Id.")] int Id)
            {
                var files = Directory.GetFiles($"Resources/Sound/Mimics/{ctx.Member.Id}/", "*");
                string fileToRename = files[Id];
                string newFilePath = $"Resources/Sound/Mimics/{ctx.Member.Username} deleted {fileToRename.Remove(0, 42).Replace(".mp3", " ")}.mp3";
                File.Move(fileToRename, newFilePath);

                await ctx.Message.DeleteAsync().ConfigureAwait(false);
                await ctx.Channel.SendMessageAsync($"{ctx.Member.Username} has deleted a mimic (Id: {Id}). This will affect the Id of all other mimics.").ConfigureAwait(false);
            }

            [Command("submit")]
            [RequirePrefixes("w!", "W!")]
            [Description("Submit a new mimic for a user, as long as they are mimicable.")]
            public async Task submiMimic(CommandContext ctx, [Description("Mention or use discord Id.")] DiscordMember member)
            {
                if (ctx.Message.Attachments.Count == 0)
                {
                    await ctx.Channel.SendMessageAsync("You must send a mp3 file within the same message as the command.");
                    return;
                }

                if (!ctx.Message.Attachments.First().FileName.EndsWith(".mp3"))
                {
                    await ctx.Channel.SendMessageAsync("That is not an mp3 file, please use .mp3.");
                    return;
                }

                await ctx.Channel.SendMessageAsync($"your clip for {member.Username}#{member.Discriminator} has been submitted and is waiting approval. This has to happen to ensure the clip the right user and isn't boring.\n\n`w@mimic approve {member.Id} {ctx.Message.Id}` to approve.");
            }

            [Command("approve")]
            [RequirePrefixes("w@", "W@")]
            [RequireRoles(RoleCheckMode.Any, "Dadmin", "Wiggims Bot")]
            [Description("Approves a mimic clip.")]
            public async Task approvemimic(CommandContext ctx, [Description("The member in the mimic")] DiscordMember member, [Description("The Id or link of submission message.")] DiscordMessage discordMessage)
            {
                await ctx.Message.DeleteAsync().ConfigureAwait(false);

                int newId = 0;
                try
                {
                    newId = Directory.GetFiles($"Resources/Sound/Mimics/{ member.Id }/", "*", SearchOption.AllDirectories).Length;
                    using (WebClient client = new WebClient())
                    {
                        client.DownloadFile(discordMessage.Attachments.FirstOrDefault().Url, $"Resources/Sound/Mimics/{ member.Id }/{discordMessage.Attachments[0].FileName}.mp3");
                    }
                }
                catch
                {
                    Directory.CreateDirectory($"Resources/Sound/Mimics/{ member.Id }/");
                    using (WebClient client = new WebClient())
                    {
                        client.DownloadFile(discordMessage.Attachments.FirstOrDefault().Url, $"Resources/Sound/Mimics/{ member.Id }/{discordMessage.Attachments[0].FileName}");
                    }
                }

                await discordMessage.DeleteAsync();

                await ctx.Channel.SendMessageAsync($"Mimic for {member.Username} was added, Id is {newId}, name is {discordMessage.Attachments[0].FileName}.");
            }

            // ########## Tasks ###########
        }

        [Command("play")]
        [RequirePrefixes("w!", "W!")]
        [Description("Plays sound from a youtube video (WIP).")]
        public async Task playYoutube(CommandContext ctx, [Description("Youtube search"), RemainingText] string search)
        {
            var searchResult = await GetSearchResultAsync(search);

            var VoiceConnection = await JoinVC(ctx);

            var youtube = new YoutubeClient();

            string file = $"Resources/Sound/Youtube/{searchResult.Id.VideoId}.webm";

            if (!File.Exists(file))
            {

                var plzWaitMsg = await ctx.Channel.SendMessageAsync("Please wait, getting audio...");
                await ctx.TriggerTypingAsync();

                var videoInfo = await youtube.Videos.GetAsync(searchResult.Id.VideoId);

                if (videoInfo.Duration > TimeSpan.FromHours(.5))
                {
                    throw new Exception("Due to current limitations you cannot play videos over 30 minutes long.");
                }

                var streamManifest = await youtube.Videos.Streams.GetManifestAsync(searchResult.Id.VideoId);

                var streamInfo = streamManifest.GetAudioOnly().Where(x => x.Container == Container.WebM).WithHighestBitrate();

                if (streamInfo != null)
                {
                    // Get the actual stream
                    var stream = await youtube.Videos.Streams.GetAsync(streamInfo).ConfigureAwait(false);

                    // Download the stream to file
                    await youtube.Videos.Streams.DownloadAsync(streamInfo, $"Resources/Sound/Youtube/{searchResult.Id.VideoId}.{streamInfo.Container}").ConfigureAwait(false);
                }

                await plzWaitMsg.DeleteAsync().ConfigureAwait(false);
            }

            await PlayAudio(ctx, VoiceConnection, file).ConfigureAwait(true);

            var embed = new DiscordEmbedBuilder
            {
                Title = $"{searchResult.Snippet.Title.Replace("&quot;", "\"").Replace("&#39;", "'").Replace("&amp;", "&").Replace("&lt;", "<").Replace("&gt;", ">")}",
                Color = DiscordColor.Orange,
                Description = $"By {searchResult.Snippet.ChannelTitle}"
            };

            string[] musicFiles = Directory.GetFiles($"Resources/Sound/Youtube/", "*");
            long dirSize = 0;
            foreach (var musicFile in musicFiles)
            {
                FileInfo info = new FileInfo(musicFile);
                dirSize += info.Length;
            }

            if (dirSize > 1000000000)
                embed.WithFooter("Directory size: " + BytesToString(dirSize));

            await ctx.Channel.SendMessageAsync(embed: embed).ConfigureAwait(false);
        }

        [Group("youtube")]
        [RequirePrefixes("w!", "W!")]
        [RequireOwner]
        public class YoutubeCommands : BaseCommandModule
        {
            [Command("search")]
            [RequirePrefixes("w!", "W!")]
            public async Task SearchYoutube(CommandContext ctx, [Description("Youtube search"), RemainingText] string search)
            {
                var list = await GetYoutubeList(1, search);
                
                await ctx.Channel.SendMessageAsync(String.Format("Videos:\n{0}\n", string.Join("\n", list)));
            }

            [Command("search")]
            [RequirePrefixes("w!", "W!")]
            public async Task SearchYoutube(CommandContext ctx, [Description("how many results do you want to show?")] int count,[Description("Youtube search"), RemainingText] string search)
            {
                var list = await GetYoutubeList(count, search);

                await ctx.Channel.SendMessageAsync(String.Format("Videos:\n{0}\n", string.Join("\n", list)));
            }

            // #### Tasks ####

            static string GetApiKey()
            {
                var json = string.Empty;

                using (var fs = File.OpenRead("config.json"))
                using (var sr = new StreamReader(fs, new UTF8Encoding(false)))
                    json = sr.ReadToEnd();

                return JsonConvert.DeserializeObject<ConfigJson>(json).youtubeApiKey;
            }

            public async Task<string> GetYoutubeLink(string search)
            {
                var youtubeService = new YouTubeService(new BaseClientService.Initializer()
                {
                    ApiKey = GetApiKey(),
                    ApplicationName = this.GetType().ToString()
                });
                var searchListRequest = youtubeService.Search.List("snippet");
                searchListRequest.Q = (search);
                searchListRequest.MaxResults = 1;

                var searchListResponse = await searchListRequest.ExecuteAsync();

                string video = new string(" ");
                List<string> playlists = new List<string>();

                return "https://www.youtube.com/v/" + searchListResponse.Items[0].Id.VideoId;
            }

            public async Task<List<string>> GetYoutubeList(int numberOfLinks, string search)
            {
                var youtubeService = new YouTubeService(new BaseClientService.Initializer()
                {
                    ApiKey = GetApiKey(),
                    ApplicationName = this.GetType().ToString()
                });
                var searchListRequest = youtubeService.Search.List("snippet");
                searchListRequest.Q = (search);
                searchListRequest.MaxResults = numberOfLinks;

                var searchListResponse = await searchListRequest.ExecuteAsync();

                List<string> videos = new List<string>();

                foreach (var searchResult in searchListResponse.Items)
                {
                    if (searchResult.Id.Kind == "youtube#video")
                    {
                        videos.Add(String.Format("{0} ({1})", searchResult.Snippet.Title.Replace("&quot;", "\""), "https://www.youtube.com/v/" + searchResult.Id.VideoId));
                        break;
                    }
                }

                return videos;
            }

            public async Task<string> getVideoId(string search)
            {
                var youtubeService = new YouTubeService(new BaseClientService.Initializer()
                {
                    ApiKey = GetApiKey(),
                    ApplicationName = this.GetType().ToString()
                });
                var searchListRequest = youtubeService.Search.List("snippet");
                searchListRequest.Q = (search);
                searchListRequest.MaxResults = 1;

                var searchListResponse = await searchListRequest.ExecuteAsync();

                foreach (var searchResult in searchListResponse.Items)
                {
                    if (searchResult.Id.Kind == "youtube#video")
                    {
                        return searchResult.Id.VideoId;
                    }
                }
                return null;
            }
        }

        // ######## Tasks ########

        public async Task<VoiceNextConnection> JoinVC(CommandContext ctx)
        {
            var VoiceClient = ctx.Client.GetVoiceNext();

            var VoiceConnection = VoiceClient.GetConnection(ctx.Guild);

            var VoiceChannel = ctx.Member?.VoiceState?.Channel;
            if (VoiceChannel == null)
            {
                throw new Exception("You need to be in a voice channel.");
            }

            if (VoiceConnection == null)
            {
                await VoiceClient.ConnectAsync(VoiceChannel);
                var Connection = VoiceClient.GetConnection(ctx.Guild);

                //this.ffmpegs = new ConcurrentDictionary<uint, Process>();
                //Connection.VoiceReceived += OnVoiceReceived;

                return Connection;
            }
            else if (VoiceConnection.Channel != VoiceChannel)
            {
                throw new Exception("You need to be in the same channel as the bot to use the command.");
            }
            
            return VoiceClient.GetConnection(ctx.Guild);
        }

        void LeaveVCAsync(CommandContext ctx)
        {
            var VoiceClient = ctx.Client.GetVoiceNext();

            var VoiceConnection = VoiceClient.GetConnection(ctx.Guild);
            if (VoiceConnection == null)
            {
                throw new InvalidOperationException("Not connected in this guild.");
            }

            /*
            VoiceConnection.VoiceReceived -= OnVoiceReceived;
            foreach (var kvp in this.ffmpegs)
            {
                await kvp.Value.StandardInput.BaseStream.FlushAsync();
                kvp.Value.StandardInput.Dispose();
                kvp.Value.WaitForExit();
            }
            this.ffmpegs = null;
            */

            VoiceConnection.Disconnect();
        }

        public async Task PlayAudio(CommandContext ctx, VoiceNextConnection VoiceConnection, string filePath)
        {
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException("This file could not be found", filePath);
            }

            if (VoiceConnection.IsPlaying)
                await VoiceConnection.WaitForPlaybackFinishAsync();

            Exception exc = null;
            await VoiceConnection.SendSpeakingAsync(true);

            try
            {

                var psi = new ProcessStartInfo
                {
                    FileName = "ffmpeg",
                    Arguments = $@"-i ""{filePath}"" -ac 2 -f s16le -ar 48000 pipe:1",
                    RedirectStandardOutput = true,
                    UseShellExecute = false
                };

                var ffmpeg = Process.Start(psi);
                var ffout = ffmpeg.StandardOutput.BaseStream;

                ffout.CopyTo(VoiceConnection.GetTransmitStream());
            }
            catch (Exception ex) { exc = ex; }
            finally
            {
                await VoiceConnection.SendSpeakingAsync(false);
            }

            if (exc != null)
                await ctx.RespondAsync($"An exception occurred during playback: `{exc.GetType()}: {exc.Message}`");
        }

        static string GetApiKey()
        {
            var json = string.Empty;

            using (var fs = File.OpenRead("config.json"))
            using (var sr = new StreamReader(fs, new UTF8Encoding(false)))
                json = sr.ReadToEnd();

            return JsonConvert.DeserializeObject<ConfigJson>(json).youtubeApiKey;
        }

        public async Task<SearchResult> GetSearchResultAsync(string search)
        {
            var youtubeService = new YouTubeService(new BaseClientService.Initializer()
            {
                ApiKey = GetApiKey(),
                ApplicationName = this.GetType().ToString()
            });
            var searchListRequest = youtubeService.Search.List("snippet");
            searchListRequest.Q = (search);
            searchListRequest.MaxResults = 3;

            var searchListResponse = await searchListRequest.ExecuteAsync();

            foreach (var searchResult in searchListResponse.Items)
            {
                if (searchResult.Id.Kind == "youtube#video")
                {
                    return searchResult;
                }
            }
            throw new Exception("The search could not find a video.");
        }

        static String BytesToString(long byteCount)
        {
            string[] suf = { "B", "KB", "MB", "GB", "TB", "PB", "EB" }; //Longs run out around EB
            if (byteCount == 0)
                return "0" + suf[0];
            long bytes = Math.Abs(byteCount);
            int place = Convert.ToInt32(Math.Floor(Math.Log(bytes, 1024)));
            double num = Math.Round(bytes / Math.Pow(1024, place), 1);
            return (Math.Sign(byteCount) * num).ToString() + suf[place];
        }

        /*public async Task OnVoiceReceived(VoiceReceiveEventArgs ea)
        {
            if (!this.ffmpegs.ContainsKey(ea.SSRC))
            {
                var psi = new ProcessStartInfo
                {
                    FileName = "ffmpeg",
                    Arguments = $@"-ac 2 -f s16le -ar 48000 -i pipe:0 -ac 2 -ar 44100 Resources/Sound/Recording/{ea.SSRC}.wav",
                    RedirectStandardInput = true
                };

                this.ffmpegs.TryAdd(ea.SSRC, Process.Start(psi));
            }

            var buff = ea.PcmData.ToArray();

            var ffmpeg = this.ffmpegs[ea.SSRC];
            await ffmpeg.StandardInput.BaseStream.WriteAsync(buff, 0, buff.Length);
            await ffmpeg.StandardInput.BaseStream.FlushAsync();
        }*/
    }
} 