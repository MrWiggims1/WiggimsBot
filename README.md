The read me is still WIP and is no where near done.

# WiggimsBot
Wiggims Bot is a discord bot that I started working on both for fun and to learn some programing.
I would not recommend using my code as an example or as a base to start from, I am extremely
new to c# and coding in general and doubt reading though what I've made will helpful to anyone.

The purpose of this repository is mainly for others who interact with wiggims bot to have a
 better understanding for how the bot works if they are curious.

If you want to make your own discord bot using this library follow [this guide](https://dsharpplus.emzi0767.com/articles/intro.html) and feel free to ask for help if you need any. 

If you find something that is wrong or you disagree with PLEASE let me know, like I've said I'm very new and will make a lot of mistakes.

# DSharpPlus

The library used for wiggims bot is called [DSharp+](https://github.com/DSharpPlus/DSharpPlus) (D#), Its a C# library using .net framework.
You don't need to understand everything about D# or C# to understand how wiggims bot works I'll do my best to explain how everything works through out the code with comments (which are pretty lacking ATM) and with this guide.
#### Glossary
These terms are not 'proper' but just simple explanations from my understanding of them. C# and D# terms will have a a subscript following each respectfully if you want to learn more about them, C# terms a quick google search should show results, and D# terms can be explained in more detail in the github repository.


* Method or task<sub>C#</sub> - A method or task is a block of code which can be executed 

* Parameter<sub>C#</sub> - A parmeter is an option which can be entered into a method,
* Attributes<sub>C#</sub> - Attributes are a simple way of adding extra meta data to either a Task or Parameter such as names or requirements, these are usually found in between a pair of square brackets `[ attribute ]`. Attributes are a feature in C# however D# has its own all of which can be found [here](https://github.com/DSharpPlus/DSharpPlus/tree/master/DSharpPlus.CommandsNext/Attributes). These attributes usually determine names or descriptions of various things.
* Code comment - A comment is just a small note for any one who is reading the code later on. Comments are followed by `//` and anything written on this line afterwards is not executed.
* Command<sub>D#</sub> - A command in our context is some task that a discord member can call through a text channel. A command consists of:
  * A prefix - This is a small piece of text which lets the bot know a member is trying to execute a command. In wiggims bots case these prefixes are `w@` and `w!`.
  * The commands name - The name lets the member know what the user is trying to do.
  * Parameters or argument- Some commands will have extra inputs for the command. For example if a command is going to add 2 numbers together. the command will have 2 number parameters such as int or double, so the user could type `w!add 5 4.52` and execute the add command that takes in 2 numbers.
* CommandContext<sub>D#</sub> - The command context contains all the information needed for the rob to react to the environment. it contains the information on who executed the command, where it was executed and contains several methods for communicating with the discord API.

# Commands
~~All commands for wiggims bot can be found [here](https://github.com/MrWiggims1/WiggimsBot/tree/master/WigsBot.Bots/Commands). Commands are split up between files to keep everything organized and tidy, and some are further put into folders, for example any commands that interact with a discord members profile can be found in the profile folder.

below is my guide on how commands are made, however you can read this [guide here](https://dsharpplus.emzi0767.com/articles/commandsnext.html#4-creating-your-first-command) which will probably be better.

As an example for a basic command ill show how to create the add command mentioned earlier.

```cs
[Command("add")]
public async Task AddCommand(double firstNumber, double secondNumber)
{
    // Respond to the command with the two input numbers added together.
    await ctx.RespondAsync($"{firstNumber} + {secondNumber} = {firstNumber + secondNumber)").ConfigureAwait(false);
}
```

`[Command("add")]` - This is the name of the command, what ever is entered here is what members will use to execute the command. This example would be executed with [prefix]add.

`public async Task AddCommand(double firstNumber, double seccondNumber)` - This is the method which is ran with the command. The '`public async Task`' will start every command,
 this is followed by the task name It's not too important what its as long as It's unique and is clear what its meant to do. after the tasks name will be brakets which contain parameters
 for the command. In this case there are 2 parameters, the first one being a double called first number and the second being another double called second number. There are many parameters that can be entered into a command with a list below.

Type               | Parameter           
-------------      |-------------
Integral           | `byte`, `sbyte`, `ushort`, `short`, `uint`, `int`, `ulong`, `long` 
Floating-point     | `float`, `double`, `decimal`      
Text and character | `string`, `char`      
Date and time      | `DateTime`, `DateTimeOffset`, `TimeSpan`
Boolean types      | `bool`
Discord entities   | `DiscordGuild`, `DiscordChannel`, `DiscordMember`, `DiscordUser`, `DiscordRole`, `DiscordMessage`, `DiscordEmoji`


The last few lines encased within the `{ }` is the code that will be executed. Here we have a comment explaining what is happening and then the line code itself. 

`await ctx.RespondAsync("").ConfigureAwait(false);`

simply means respond to the context in which the command was given with the contents being that inside the apostrophes.

That is the basics of how a command works, there are many more things that you can do with the command but thats up to you to find out more if you wish, you can either take a look at my commands or [someone else's](https://github.com/DSharpPlus/Example-Bots/tree/master/DSPlus.Examples.CSharp.Ex02). 



