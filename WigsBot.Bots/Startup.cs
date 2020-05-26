using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using WigsBot.Core.Services.GuildPreferenceServices;
using WigsBot.Core.Services.GuildPreferencesServices;
using WigsBot.Core.Services.Items;
using WigsBot.Core.Services.Profiles;
using WigsBot.DAL;

namespace WigsBot.Bot
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            var json = string.Empty;

            using (var fs = File.OpenRead("config.json"))
            using (var sr = new StreamReader(fs, new UTF8Encoding(false)))
                json = sr.ReadToEnd();

            var configJson = JsonConvert.DeserializeObject<ConfigJson>(json);

            services.AddDbContext<RPGContext>(options =>
            {
                options.UseSqlServer(configJson.dbconnectionstring,
                    x => x.MigrationsAssembly("WigsBot.DAL.Migrations"));
                options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
                options.EnableDetailedErrors(false);
            });

            services.AddScoped<IProfileService, ProfileService>();
            services.AddScoped<IExperienceService, ExperienceService > ();
            services.AddScoped<IGotService, GotService>();
            services.AddScoped<ILeaveService, LeaveService>();
            services.AddScoped<IGoldService, GoldService>();
            services.AddScoped<ISpellErrorService, SpellErrorService>();
            services.AddScoped<ISpellCorrectService, SpellCorrectService>();
            services.AddScoped<IBoganCountService, BoganCountService>();
            services.AddScoped<ISteamIdService, SteamIdService>();
            services.AddScoped<IBeatSaberIdService, BeatSaberIdService>();
            services.AddScoped<IUplayIdService, UplayIdService>();
            services.AddScoped<IQuietModeService, QuietModeService>();
            services.AddScoped<ITextProcessorService, TextProcessorService>();
            services.AddScoped<IGuildPreferences, GuildPreferencesService>();
            services.AddScoped<IAssignableRoleService, AssignableRoleService>();
            services.AddScoped<ISpellingSettingsService, SpellingSettingsService>();
            services.AddScoped<ITodoService, TodoService>();
            services.AddScoped<IMimicableService, MimicableService>();
            services.AddScoped<IRobbingItemService, RobbingItemService>();

#pragma warning disable ASP0000 // Do not call 'IServiceCollection.BuildServiceProvider' in 'ConfigureServices'
            var serviceProvider = services.BuildServiceProvider();
#pragma warning restore ASP0000 // Do not call 'IServiceCollection.BuildServiceProvider' in 'ConfigureServices'

            var bot = new Bot(serviceProvider);
            services.AddSingleton(bot);
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
        }
    }
}
