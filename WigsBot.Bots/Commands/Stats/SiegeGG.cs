
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;
using WigsBot.Core.Services.Profiles;
using WigsBot.DAL.Models.Profiles;
using WigsBot.Core.ViewModels;
using WigsBot.Bot.Handlers.Dialogue.Steps;
using WigsBot.Bot.Handlers.Dialogue;
using DSharpPlus.CommandsNext.Exceptions;
using System;
using RestSharp;
using RestSharp.Authenticators;

namespace WigsBot.Bot.Commands.Stats
{
    [Group("siegegg")]
    [Description("Shows SiegeGG stats.")]
    [RequireOwner]
    public class SiegeGG : BaseCommandModule
    {
        [Command("news")]
        [RequirePrefixes("w!", "W!")]
        public async Task Basic(CommandContext ctx)
        {
            await GetSiegeGGJson("https://api.siege.gg/v1/news");
        }
        
        //######### tasks ##########
        public async Task<SiegeGGJson> GetSiegeGGJson(string urlApi)
        {
            {
                var client = new RestClient(urlApi);
                var request = new RestRequest(Method.GET)
                {
                    RequestFormat = DataFormat.Json
                };
                request.AddHeader("Bearer", "spWYA4Y_lef.hoximzGFKYH5TlyhZG-E");

                IRestResponse response = client.Execute(request);

                string stringresult = response.Content;

                SiegeGGJson Json = JsonConvert.DeserializeObject<SiegeGGJson>(stringresult);

                return Json;
            }
        }
             

        public class SiegeGGJson
        {
        
        }
    }
}
*/



