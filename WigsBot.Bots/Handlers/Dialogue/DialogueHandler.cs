using DSharpPlus;
using DSharpPlus.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;
using WigsBot.Bot.Handlers.Dialogue.steps;

namespace WigsBot.Bot.Handlers.Dialogue
{
    public class DialogueHandler
    {
        private readonly DiscordClient _client;
        private readonly DiscordChannel _channel;
        private readonly DiscordUser _user;
        private IDialogueStep _currentStep;

        public DialogueHandler(
            DiscordClient client,
            DiscordChannel channel,
            DiscordUser user,
            IDialogueStep startingStep)
        {
            _client = client;
            _channel = channel;
            _user = user;
            _currentStep = startingStep;
        }

        private readonly List<DiscordMessage> messages = new List<DiscordMessage>();

        public async Task<bool> ProcessDialogue()
        {
            try
            {
                while (_currentStep != null)
                {
                    _currentStep.OnMessageAdded += (message) => messages.Add(message);

                    bool canceled = await _currentStep.ProcessStep(_client, _channel, _user).ConfigureAwait(false);

                    if (canceled)
                    {
                        await DeleteMessages().ConfigureAwait(false);

                        var cancelEmbed = new DiscordEmbedBuilder
                        {
                            Title = "The dialogue has successfully been cancelled",
                            Description = _user.Mention,
                            Color = DiscordColor.Orange
                        };

                        await _channel.SendMessageAsync(embed: cancelEmbed).ConfigureAwait(false);

                        return false;
                    }

                    _currentStep = _currentStep.nextStep;
                }

                await DeleteMessages().ConfigureAwait(false);

                return true;
            }
            catch { return false; }
        }

        private async Task DeleteMessages()
        {
            if(_channel.IsPrivate) { return; }

            foreach(var message in messages)
            {
                await message.DeleteAsync().ConfigureAwait(false);
            }
        }
    }
}
