using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WigsBot.Core.ViewModels;
using WigsBot.DAL;
using WigsBot.DAL.Models.Profiles;

namespace WigsBot.Core.Services.Profiles
{
    public interface ITextProcessorService
    {
        /// <summary>
        /// Processes the users messages and updates the members profile.
        /// </summary>
        /// <param name="discordId">The id of the member.</param>
        /// <param name="guildId">The guild Id.</param>
        /// <param name="spellCorrect">The correct number of words.</param>
        /// <param name="spellWrong">The incorrect number of words.</param>
        /// <param name="boganCount">The count of bogan words.</param>
        /// <param name="xpAmount">The number of xp to give</param>
        /// <param name="arrayLength">The length of the incorrectly spelled words list.</param>
        /// <param name="incorrectWords">The incorrect words to add to a members list.</param>
        /// <returns>Weather or not if the member leveled up, don't know how it works, just copy and paste the code.</returns>
        Task<TextProcessorViewModel> ProcessTextAsync(ulong discordId, ulong guildId, string Username,int spellCorrect, int spellWrong, int boganCount, int xpAmount, int arrayLength, List<string> incorrectWords);


        /// <summary>
        /// Removes a word for all users incorrect words lists, as well as retracting a error count if the word is present.
        /// </summary>
        /// <param name="newWord">The word to remove from lists.</param>
        /// <returns></returns>
        Task RemoveDictionaryWordFromLists(string newWord);
    }

    public class TextProcessorService : ITextProcessorService
    {
        private readonly DbContextOptions<RPGContext> _options;
        private readonly IProfileService _profileService;

        public TextProcessorService(DbContextOptions<RPGContext> options, IProfileService profileService)
        {
            _options = options;
            _profileService = profileService;
        }

        public async Task<TextProcessorViewModel> ProcessTextAsync(ulong discordId, ulong guildId, string username,int spellCorrect, int spellWrong, int boganCount, int xpAmount, int arrayLength, List<string> incorrectWords)
        {
            using var context = new RPGContext(_options);

            Profile profile = await _profileService.GetOrCreateProfileAsync(discordId, guildId).ConfigureAwait(false);

            // process xp

            int levelBefore = profile.Level;

            profile.Xp += xpAmount;

            profile.UserName = username;

            // process spelling

            profile.SpellErrorCount += spellWrong;

            profile.SpellCorrectCount += spellCorrect;

            profile.BoganCount += boganCount;

            if (incorrectWords.Count != 0)
            {
                if (profile.SpellErrorList == "still empty")
                {
                    List<string> newSpellList = incorrectWords;

                    profile.SpellErrorList = String.Join(", ", newSpellList);
                }
                else
                {
                    string[] prevSpellList = profile.SpellErrorList.Split(", ");

                    List<string> newSpellList = new List<string>();

                    foreach (var word in prevSpellList)
                    {
                        newSpellList.Add(word);
                    }

                    foreach (var word in incorrectWords)
                    {
                        newSpellList.Add(word);
                    }

                    newSpellList.Reverse();

                    StringBuilder builder = new StringBuilder();
                    int indexWord = 1;
                    foreach (var word in newSpellList)
                    {
                        if (!string.IsNullOrWhiteSpace(word))
                        {
                            if (indexWord < arrayLength)
                            {
                                indexWord += 1;
                                builder.Append(word + ", ");
                            }
                        }
                    }

                    var endList = builder.ToString().Split(", ").Reverse().ToList<string>();

                    profile.SpellErrorList = String.Join(", ", endList);                
                } 
            }

            // update database and check for level up

            context.Profiles.Update(profile);

            await context.SaveChangesAsync().ConfigureAwait(false);

            int levelAfter = profile.Level;

            return new TextProcessorViewModel
            {
                Profile = profile,
                LevelledUp = levelAfter > levelBefore
            };
        }

        public async Task RemoveDictionaryWordFromLists(string newWord)
        {
            using var context = new RPGContext(_options);

            var profileList = context.Profiles.Where(x => x.Xp > 100).ToList();

            foreach (var profile in profileList)
            {
                string[] prevSpellList = profile.SpellErrorList.Split(", ");
                StringBuilder builder = new StringBuilder();
                foreach (var word in prevSpellList)
                {
                    if (word != newWord)
                    {
                        builder.Append($"{word}, ");
                    } else { profile.SpellErrorCount -= 1; }
                }

                var endList = builder.ToString().Split(", ").ToList<string>();

                profile.SpellErrorList = String.Join(", ", endList);
            }

            context.Profiles.UpdateRange(profileList);

            await context.SaveChangesAsync().ConfigureAwait(false);
        }
    }
}