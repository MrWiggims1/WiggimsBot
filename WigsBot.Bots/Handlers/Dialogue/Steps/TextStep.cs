using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using WigsBot.Bot.Handlers.Dialogue.steps;

namespace WigsBot.Bot.Handlers.Dialogue.Steps
{
    public class TextStep : DialogueStepBase
    {
        private IDialogueStep _nextStep;
        private readonly int? _minLength;
        private readonly int? _maxLength;

        public TextStep(
            string content,
            IDialogueStep nextStep,
            int? minLength = null,
            int? maxLength = null) : base(content)
        {
            _nextStep = nextStep;
            _minLength = minLength;
            _maxLength = maxLength;
        }

        public Action<string> onValidResult { get; set; } = delegate { };

        public override IDialogueStep nextStep => _nextStep;

        public void SetNextStep(IDialogueStep nextStep)
        {
            _nextStep = nextStep;
        }

        public override async Task<bool> ProcessStep(DiscordClient client, DiscordChannel channel, DiscordUser user)
        {
            var embedBuilder = new DiscordEmbedBuilder
            {
                Title = "Please Respond Below",
                Description = $"{user.Mention}, {_content}",
            };

            embedBuilder.AddField("To Stop The Dialogue", "Use the w!cancel Command");

            if (_minLength.HasValue)
            {
                embedBuilder.AddField("Min Length:", $"{_minLength.Value} Characters");
            }
            if (_maxLength.HasValue)
            {
                embedBuilder.AddField("Max Length:", $"{_maxLength.Value} Characters");
            }

            var interactivity = client.GetInteractivity();

            
                while (true)
                {
                    var embed = await channel.SendMessageAsync(embed: embedBuilder).ConfigureAwait(false);

                    OnMessageAdded(embed);

                    try
                    {

                        var messageResult = await interactivity.WaitForMessageAsync(
                            x => x.ChannelId == channel.Id && x.Author.Id == user.Id).ConfigureAwait(false);

                        OnMessageAdded(messageResult.Result);

                        if (messageResult.Result.Content.Equals("w!cancel", StringComparison.OrdinalIgnoreCase))
                        {
                            return true;
                        }

                        if (_minLength.HasValue)
                        {
                            if (messageResult.Result.Content.Length < _minLength.Value)
                            {
                                await TryAgain(channel, $"Your input is {_minLength.Value - messageResult.Result.Content.Length} characters too short").ConfigureAwait(false);
                                continue;
                            }
                        }
                        if (_maxLength.HasValue)
                        {
                            if (messageResult.Result.Content.Length > _maxLength.Value)
                            {
                                await TryAgain(channel, $"Your input is {messageResult.Result.Content.Length - _maxLength.Value} characters too long").ConfigureAwait(false);
                                continue;
                            }
                        }

                        onValidResult(messageResult.Result.Content);

                        return false;
                    }
                    catch
                    {
                        await embed.DeleteAsync();
                        await channel.SendMessageAsync("Session timed out.");
                        return true;
                    }
                }
        }
    }
}

