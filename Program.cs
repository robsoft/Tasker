// Tasker © 2024 by Rob Uttley is licensed under CC BY 4.0. To view a copy of this license, visit https://creativecommons.org/licenses/by/4.0/

//todo:
/*
 * - some kind of verbose option to easily flip the config option

 * - clean command
 * - finish tidying output
 * - tidy-up inputs (capitalise recurring etc)
 * - think about sub tasks ....
 *   probably tasker sub 11 "thingy" (no recurring or sleeping)
 *   adds a task to 11 (how many levels, how to store in the csv file)
 *   
 * - ability to fetch the files from somewhere else (tasker.cfg)
 * - also a 'quiet' option in cfg
 *
 * look at making a single executable with embedded libraries etc
 * dotnet publish -c Release -r win-x64 --self-contained -p:PublishSingleFile=true
 *  
 * - test on Mac
 * - put in GH as public
 * 
 */


using Spectre.Console;
using System.CommandLine;

namespace Tasker
{
    partial class Program
    {
        static string _tasksfile = "tasks.txt";
        static string _donefile = "done.txt";

        private static List<TaskerTask> taskerTasks = [];
        private static List<TaskerDone> taskerDone = [];


        static async Task<int> Main(string[] args)
        {
            var rootCommand = new RootCommand("Tasker - task/to-do console tool");


            // setup subcommand options            

            var listDoneOption = new Option<bool>(
                name: "-done",
                description: "List all done tasks",
                getDefaultValue: () => false );

            var clearDoneOption = new Option<bool>(
                name: "-done",
                description: "Clear only the done tasks",
                getDefaultValue: () => false);

            var listSleepOption = new Option<bool>(
                name: "-sleep",
                description: "List all sleeping tasks",
                getDefaultValue: () => false);

            var addRecurringOption = new Option<string>(
                name: "-r",
                description: "recurs ([d]aily, [w]eekly, [m]onthly, [a]nnual, or [n]um of days",
                getDefaultValue: () => string.Empty);

            var addSleepingOption = new Option<bool>(
                name: "-s",
                description: "is created as sleeping",
                getDefaultValue: () => false);

            var idArgument = new Argument<string>(
                name: "Id (or name)",
                description: "The task Id or name",
                getDefaultValue: () => string.Empty);
            
            var taskNameArgument = new Argument<string>(
                name: "Name[;Description]",
                description: "The name of the task, and optionally a description. Use \" to escape spaces",
                getDefaultValue: () => string.Empty);


            // setup main commands

            //tasker list -done -sleep
            var listCommand = new Command("list", "List all open tasks")
            { listDoneOption, listSleepOption };

            //tasker + taskname;tasknotes 
            var addCommand = new Command("+", "Add a task to the file")
            { taskNameArgument, addRecurringOption, addSleepingOption };

            //tasker - id
            var completeCommand = new Command("-", "Mark a task as completed")
            { idArgument };

            //tasker rem id
            var removeCommand = new Command("rem", "Remove (delete) a task entirely")
            { idArgument };


            //tasker sleep id
            var sleepCommand = new Command("sleep", "Mark a task as sleeping")
            { idArgument };

            //tasker wake id
            var wakeCommand = new Command("wake", "Mark a sleeping task as awake (active)")
            { idArgument };

            //tasker clean
            var cleanCommand = new Command("clean", "Remove all the backup files") { };

            //tasker renum
            var renumberCommand = new Command("renum", "Renumber the existing tasks (active and sleeping)") { };

            //tasker clean
            var clearCommand = new Command("clear", "Remove all tasks, optionally just the done tasks")
            { clearDoneOption };

            // instantiate commands; they appear in this order at the 'help' prompts 

            rootCommand.AddCommand(addCommand);
            rootCommand.AddCommand(completeCommand);
            rootCommand.AddCommand(listCommand);
            rootCommand.AddCommand(sleepCommand);
            rootCommand.AddCommand(wakeCommand);
            rootCommand.AddCommand(removeCommand);
            rootCommand.AddCommand(renumberCommand);
            rootCommand.AddCommand(cleanCommand);
            rootCommand.AddCommand(clearCommand);


            // setup handlers

            listCommand.SetHandler( (done, sleep) =>
                {
                    DoList(done, sleep);
                },
                listDoneOption, listSleepOption);


            sleepCommand.SetHandler( (id) =>
                {
                    DoSleep(id);
                },
                idArgument);


            wakeCommand.SetHandler( (id) =>
                {
                    DoWake(id);
                },
                idArgument);


            removeCommand.SetHandler( (id) =>
                {
                    DoRemove(id);
                },
                idArgument);


            completeCommand.SetHandler( (id) =>
                {
                    DoComplete(id);
                },
                idArgument);
            

            addCommand.SetHandler((taskName, recurs, sleeping) =>
                {
                    DoAdd(taskName, recurs, sleeping);
                },
                taskNameArgument, addRecurringOption, addSleepingOption);


            renumberCommand.SetHandler( () => 
                { 
                    DoRenum(); 
                });


            clearCommand.SetHandler( (onlyDone) => 
                { 
                    DoClear(onlyDone); 
                },
                clearDoneOption);


            cleanCommand.SetHandler(() =>
            {
                DoClean();
            });


            try
            {
                LoadConfig();

                // execute command
                return rootCommand.InvokeAsync(args).Result;
            }
            catch (Exception ex)
            {
                AnsiConsole.WriteException(ex,
                    ExceptionFormats.ShortenPaths | ExceptionFormats.ShortenTypes |
                    ExceptionFormats.ShortenMethods);
                return 1;
            }

        }


        


    }


}
