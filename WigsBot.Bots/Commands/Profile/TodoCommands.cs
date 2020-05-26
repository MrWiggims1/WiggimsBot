using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using System.Threading.Tasks;
using WigsBot.Core.Services.Profiles;
using WigsBot.DAL.Models.Profiles;
using WigsBot.Core.ViewModels;
using System.Linq;
using System;
using System.Text;

namespace WigsBot.Bot.Commands.Profilecommands
{
    [Group("todo")]
    [RequirePrefixes("w!", "W!")]
    [RequireOwner]
    [Description("A personal to-do list for everyone to use")]
    public class TodoCommands : BaseCommandModule
    {
        private readonly IProfileService _profileService;
        private readonly ITodoService _todoService;

        public TodoCommands(
            IProfileService profileService,
            ITodoService todoService
            )
        {
            _profileService = profileService;
            _todoService = todoService;
        }

        [GroupCommand]
        public async Task gettodolist(CommandContext ctx)
        {
            ToDoList todoLists = await _todoService.GetListsAsync(ctx.Member.Id, ctx.Guild.Id);

            if (todoLists.lists.Count == 0)
            {
                await ctx.Channel.SendMessageAsync("You currently do not have any lists, to add one try the `w!todo addlist [new list name]` and then `w!todo [list name].[task name].[task description]` to add a new task, where a full stops indicates the end of each option.");
                return;
            }

            if (todoLists.lists.Count == 1)
            {
                var list = await _todoService.GetListAsync(ctx.User.Id, ctx.Guild.Id, todoLists.lists.FirstOrDefault().name);

                StringBuilder sb = new StringBuilder();
                decimal tasksDone = 0;
                decimal tasksNotDone = 0;
                int place = 1;
                foreach (Tasks task in list.tasks)
                {
                    sb.Append($"[{place}]\t> #{task.name}\n\t\t'{task.description}'\n@ Completed: {task.done.ToString()}\n\n");
                    place++;
                    if (task.done) { tasksDone++; }
                    else { tasksNotDone++; }
                }
                await ctx.Channel.SendMessageAsync($"```py\n'{list.name}: {list.tasks.Count} tasks'\t#{tasksDone} out of {tasksNotDone + tasksDone} complete: {decimal.Round(decimal.Divide(tasksDone, tasksDone + tasksNotDone) * 100, 2)}% done.\n-------------------------------------------------------\n{sb.ToString()}-------------------------------------------------------\n@ Still work in progress and is not yet available.```");
                return;
            }
            else
            {
                StringBuilder sb = new StringBuilder();
                int place = 1;
                foreach (var List in todoLists.lists)
                {
                    decimal tasksDone = 0;
                    decimal tasksNotDone = 0;
                    try
                    {
                        foreach (var task in List.tasks)
                        {
                            if (task.done) { tasksDone++; }
                            else { tasksNotDone++; }
                        }
                    }
                    catch { }

                    try
                    {
                        sb.Append($"[{place}]\t> #{List.name}\n\t\tTotal tasks: {List.tasks.Count} - {decimal.Round(decimal.Divide(tasksDone, tasksDone + tasksNotDone) * 100, 2)}% done \n\n");
                    }
                    catch
                    {
                        sb.Append($"[{place}]\t> #{List.name}\n\t\tTotal tasks: 0 - 0 % done \n\n");
                    }
                    place += 1;
                }


                await ctx.Channel.SendMessageAsync($"```py\n{sb.ToString()}-------------------------------------------------------\n@ Still work in progress and is not yet available.```");
            }
        }

        [GroupCommand]
        public async Task gettodolist(CommandContext ctx, [RemainingText, Description("Lists name.")] string listName)
        {
            ToDoList todoLists = await _todoService.GetListsAsync(ctx.Member.Id, ctx.Guild.Id);

            if (!todoLists.lists.Any(x => x.name.ToLower() == listName.ToLower()))
            {
                await ctx.Channel.SendMessageAsync("This list does not exist, to add one try the `w!todo addlist [new list name]`. Or `w!todo` to see your lists.");
            }
            else if (todoLists.lists.Count == 1)
            {
                await ctx.Channel.SendMessageAsync("You currently do not have any lists, to add one try the `w!todo addlist [new list name]` and then `w!todo [list name].[task name].[task description]` to add a new task, where a full stops indicates the end of each option.");
                return;
            }

            var list = await _todoService.GetListAsync(ctx.User.Id, ctx.Guild.Id, todoLists.lists.FirstOrDefault(x => x.name.ToLower() == listName.ToLower()).name);

            StringBuilder sb = new StringBuilder();
            decimal tasksDone = 0;
            decimal tasksNotDone = 0;
            int place = 1;
            foreach (Tasks task in list.tasks)
            {
                sb.Append($"[{place}]\t> @{task.name}\n\t\t#{task.description}\n@ Completed: {task.done.ToString()}\n\n");
                place++;
                if (task.done) { tasksDone++; }
                else { tasksNotDone++; }
            }
            await ctx.Channel.SendMessageAsync($"```py\n'{list.name}: {list.tasks.Count} tasks'\t#{tasksDone} out of {tasksNotDone + tasksDone} complete: {decimal.Round(decimal.Divide(tasksDone, tasksDone + tasksNotDone) * 100, 2)}% done.\n-------------------------------------------------------\n{sb.ToString()}-------------------------------------------------------\n@ Still work in progress and is not yet available.```");
            return;
        }

        [Command("addlist")]
        [Description("Adds a new list to your to-do lists.")]
        public async Task addnewlist(CommandContext ctx, [RemainingText, Description("Name of new list")] string listName)
        {
            await _todoService.AddNewListAsync(ctx.User.Id, ctx.Guild.Id, listName);
        }

        [Command("addtask")]
        [Description("Adds a new task, it is important that you separate the lists name, tasks name and description with a full stop or it will not work.")]
        public async Task addnewtask(CommandContext ctx, [RemainingText, Description("[List Name].[task name].[Task Description]")] string taskDetails)
        {
            if (!ctx.Message.Content.Contains("."))
            {
                throw new ArgumentException("There was no separators present");
            }

            var stringResults = taskDetails.Split(".").ToList();

            StringBuilder sb = new StringBuilder();
            int count = 1;
            foreach (string stringPart in stringResults)
            {
                if (count > 2) { sb.Append(stringPart); }                
                count ++;
            }

            await _todoService.AddNewTaskAsync(ctx.User.Id, ctx.Guild.Id,  stringResults[0], stringResults[1], sb.ToString());

            await ctx.Channel.SendMessageAsync("Your task has been added!");
        }
    }
}
    
