using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using WigsBot.Core.ViewModels;
using WigsBot.DAL;
using WigsBot.DAL.Models.Profiles;
using Newtonsoft.Json;
using System.Text;

namespace WigsBot.Core.Services.Profiles
{
    public interface ITodoService
    {
        /// <summary>
        /// Adds a new to-do list to the members to-do lists JSON.
        /// </summary>
        /// <param name="discordId">The member of the Member.</param>
        /// <param name="guildId">The guild Id.</param>
        /// <param name="listName">The name of the new list.</param>
        /// <returns></returns>
        public Task AddNewListAsync(ulong discordId, ulong guildId, string listName);


        /// <summary>
        /// Gets a members to-do list JSON.
        /// </summary>
        /// <param name="discordId">The members Id.</param>
        /// <param name="guildId">The guild Id.</param>
        /// <returns>The lists a member has.</returns>
        public Task<ToDoList> GetListsAsync(ulong discordId, ulong guildId);


        /// <summary>
        /// Gets a specific to-do list for a member.
        /// </summary>
        /// <param name="discordId">The Id of a member.</param>
        /// <param name="guildId">The guild Id,</param>
        /// <param name="listName">The name of the list to get.</param>
        /// <returns>The list.</returns>
        public Task<Lists> GetListAsync(ulong discordId, ulong guildId, string listName);


        /// <summary>
        /// Adds a new task to a list.
        /// </summary>
        /// <param name="discordId">The Id of the member</param>
        /// <param name="guildId">The guild Id.</param>
        /// <param name="listName">The name of the list.</param>
        /// <param name="taskName">The name of the new task to add.</param>
        /// <param name="taskDescription">The description of the task.</param>
        /// <returns></returns>
        public Task AddNewTaskAsync(ulong discordId, ulong guildId, string listName, string taskName, string taskDescription);
    }

    public class TodoService : ITodoService
    {
        private readonly DbContextOptions<RPGContext> _options;
        private readonly IProfileService _profileService;

        public TodoService(DbContextOptions<RPGContext> options, IProfileService profileService)
        {
            _options = options;
            _profileService = profileService;
        }

        public async Task AddNewListAsync(ulong discordId, ulong guildId, string listName)
        {
            using var context = new RPGContext(_options);

            Profile profile = await _profileService.GetOrCreateProfileAsync(discordId, guildId);

            ToDoList todoArray = JsonConvert.DeserializeObject<ToDoList>(profile.ToDoJson);

            if (todoArray.lists.Exists(x => x.name == listName))
            {
                throw new Exception("This list already exists, please add a new task to that list, or create a new list with another name.");
            }

            List<Tasks> defaultTask = new List<Tasks>();
            defaultTask.Add(new Tasks { name = "Add first item to todolist!", description = "Use the w!todo addtask command to add a new task to your list." });


            Lists listToAdd = new Lists { name = listName , tasks = defaultTask };

            todoArray.lists.Add(listToAdd);

            string newListArray = JsonConvert.SerializeObject(todoArray);

            profile.ToDoJson = newListArray;

            context.Profiles.Update(profile);

            await context.SaveChangesAsync().ConfigureAwait(false);
        }

        public async Task<ToDoList> GetListsAsync(ulong discordId, ulong guildId)
        {
            using var context = new RPGContext(_options);

            Profile profile = await _profileService.GetOrCreateProfileAsync(discordId, guildId);

            return JsonConvert.DeserializeObject<ToDoList>(profile.ToDoJson);
        }

        public async Task<Lists> GetListAsync(ulong discordId, ulong guildId, string listName)
        {
            using var context = new RPGContext(_options);

            Profile profile = await _profileService.GetOrCreateProfileAsync(discordId, guildId);

            ToDoList todoArray = JsonConvert.DeserializeObject<ToDoList>(profile.ToDoJson);

            if (!todoArray.lists.Any(x => x.name == listName))
            {
                return null;
            }

            return todoArray.lists.First(x => x.name == listName);
        }

        public async Task AddNewTaskAsync(ulong discordId, ulong guildId, string listName, string taskName, string taskDescription)
        {
            if (string.IsNullOrWhiteSpace(listName))
                throw new Exception("Please read the instructions carefully, you did not use the command correctly.");

            if (string.IsNullOrWhiteSpace(taskName))
                throw new Exception("Please read the instructions carefully, you did not use the command correctly.");

            if (string.IsNullOrWhiteSpace(taskDescription))
                throw new Exception("Please read the instructions carefully, you did not use the command correctly.");

            using var context = new RPGContext(_options);

            Profile profile = await _profileService.GetOrCreateProfileAsync(discordId, guildId);

            ToDoList todoArray = JsonConvert.DeserializeObject<ToDoList>(profile.ToDoJson);

            if (!todoArray.lists.Any(x => x.name.ToLower() == listName.ToLower()))
                throw new Exception("This list does not exist.");

            Tasks newtask = new Tasks() { name = taskName, description = taskDescription, done = false};
            todoArray.lists.First(x => x.name.ToLower() == listName.ToLower()).tasks.Add(newtask);

            string newListArray = JsonConvert.SerializeObject(todoArray);

            profile.ToDoJson = newListArray;

            context.Profiles.Update(profile);

            await context.SaveChangesAsync().ConfigureAwait(false);
        }

        /*
        public async Task<bool> RemoveTaskAsync(ulong discordId, ulong guildId, string listName, string taskName)
        {
            using var context = new RPGContext(_options);

            Profile profile = await _profileService.GetOrCreateProfileAsync(discordId, guildId);

            ToDoList todoArray = JsonConvert.DeserializeObject<ToDoList>(profile.ToDoJson);
        }*/
    }

    public class ToDoList
    {
        public List<Lists> lists { get; set; }
    }

    public class Lists
    {
        public string name { get; set; }
        public List<Tasks> tasks { get; set; }
    }

    public class Tasks
    {
        public string name { get; set; }
        public string description { get; set; }
        public bool done { get; set; }
    }
}