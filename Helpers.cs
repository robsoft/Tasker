using Spectre.Console;

namespace Tasker
{
    partial class Program
    {

        const int BAD_TASK_ID = 0;
        const string NOT_SLEEPING = "";
        const string SLEEPING = "0";    // something that we won't try to parse as a datetime


        private static int GetNextId()
        {
            var id = BAD_TASK_ID;
            foreach (var task in taskerTasks)
            {
                if (task.Id > id) id = task.Id;
            }
            id++;
            return id;
        }


        private static string GetNow()
        {
            return DateTime.Now.ToString("yyyy-MM-dd");
        }


        // convert the string to a task Id, potentially by looking for it in the 'name' property
        private static int ArgToTaskId(string arg)
        {
            var id = arg.ToUpper();
            if (int.TryParse(id, out int taskId))
            {
                return taskId;
            }
            else
            {
                foreach (var task in taskerTasks)
                {
                    if (task.Name.ToUpper() == id)
                    {
                        return task.Id;
                    }
                }
            }
            return BAD_TASK_ID;
        }

        private static TaskerTask? GetTaskFromArgId(string argId)
        {
            var taskId = ArgToTaskId(argId);
            return GetTaskFromId(taskId);
        }


        private static bool ValidateRecurring(string recurring)
        {
            var arg = recurring.ToUpper().TrimStart('R');
            if (arg == "D" || arg == "DAILY") return true;
            if (arg == "W" || arg == "WEEKLY") return true;
            if (arg == "M" || arg == "MONTHLY") return true;
            if (arg == "A" || arg == "ANNUAL") return true;
            if (int.TryParse(arg, out int _)) return true;
            return false;
        }


        private static string GetNextSleep(string recurring)
        {
            var arg = recurring.ToUpper().TrimStart('R');
            if (arg == "D" || arg == "DAILY")
            {
                return DateTime.Now.AddDays(1).ToString("yyyy-MM-dd");
            }
            if (arg == "W" || arg == "WEEKLY")
            {
                return DateTime.Now.AddDays(7).ToString("yyyy-MM-dd");
            }
            if (arg == "M" || arg == "MONTHLY")
            {
                return DateTime.Now.AddMonths(1).ToString("yyyy-MM-dd");
            }
            if (arg == "A" || arg == "ANNUAL")
            {
                return DateTime.Now.AddYears(1).ToString("yyyy-MM-dd");
            }

            // ok, maybe it's a number of days
            if (int.TryParse(arg, out int days))
            {
                return DateTime.Now.AddDays(days).ToString("yyyy-MM-dd");
            }

            // for safety just return tomorrow, we can't make sense of the option
            return DateTime.Now.AddDays(1).ToString("yyyy-MM-dd");
        }


        private static TaskerTask? GetTaskFromId(int id)
        {
            if (id > BAD_TASK_ID)
            {
                foreach (var task in taskerTasks)
                {
                    if (task.Id == id)
                    {
                        return task;
                    }
                }
            }
            return null;
        }


        private static void GeneralError(string message)
        {
            if (config.UseColor)
            {
                AnsiConsole.MarkupLine($"[{config.Colors.ErrorColor}]{message}[/]");
            }
            else
            {
                Console.WriteLine($"Error : {message}");
            }
        }


        private static void GeneralOutput(string message)
        {
            if (config.UseColor)
            {
                AnsiConsole.MarkupLine($"[{config.Colors.DefaultColor}]{message}[/]");
            }
            else
            {
                Console.WriteLine(message);
            }
        }
        private static void OutputTask(TaskerTask task)
        {
            if (config.UseColor)
            {
                AnsiConsole.MarkupLine($"[{config.Colors.TitleColor}]{task.Id,3}[/] [{config.Colors.DefaultColor}]{task.Name,-60}[/] [{config.Colors.DescriptionColor}]{task.Recurring,4} {task.Sleeping}[/]");
            }
            else
            {
                Console.WriteLine($"{task.Id,3} {task.Name,-60} {task.Recurring,4} {task.Sleeping}");
            }
        }
        private static void OutputDone(TaskerDone task)
        {
            if (config.UseColor)
            {
                AnsiConsole.MarkupLine($"[{config.Colors.TitleColor}]{task.Completed,19} {task.Id,3}[/] [{config.Colors.DefaultColor}]{task.Name,-54}[/]");
            }
            else
            {
                Console.WriteLine($"{task.Completed,19} {task.Id,3} {task.Name,-54}");
            }
        }

        private static void TitleOutput(string message)
        {
            if (config.UseColor)
            {
                AnsiConsole.MarkupLine($"[{config.Colors.TitleColor}]{message}[/]");
            }
            else
            {
                Console.WriteLine(message);
            }
        }


        private static void SingleLineOutput(string message)
        {
            if (config.UseColor)
            {
                AnsiConsole.Markup($"[{config.Colors.DescriptionColor}]{message}[/]");
            }
            else
            {
                Console.Write(message);
            }
        }


        private static void OutputDescription(string description)
        {
            if (!string.IsNullOrEmpty(description))
            {
                if (config.UseColor)
                {
                    AnsiConsole.MarkupLine($"[{config.Colors.DescriptionColor}]    {description,-75}[/]");
                }
                else
                {
                    Console.WriteLine($"    {description,-75}");
                }
            }
        }


        private static void ListTasks(bool output, bool sleep = false)
        {
            if (!output) return;

            if (sleep)
            {
                TitleOutput("Sleeping Tasks");
                foreach (var record in taskerTasks.Where(x => x.Sleeping != NOT_SLEEPING))
                {
                    OutputTask(record);
                    OutputDescription(record.Description);
                }

            }
            else
            {
                TitleOutput("Active Tasks");
                foreach (var record in taskerTasks.Where(x => x.Sleeping == NOT_SLEEPING))
                {
                    OutputTask(record);
                    OutputDescription(record.Description);
                }
            }
        }


        private static void ListDone(bool output)
        {
            if (!output) return;

            TitleOutput("Completed Tasks");
            foreach (var record in taskerDone.OrderByDescending(x=>x.Completed))
            {
                OutputDone(record);
                OutputDescription(record.Description);
            }
        }


        private static bool ShowYNPrompt(string prompt)
        {
            bool result;
            SingleLineOutput($"{prompt} (Y/N) ");
            ConsoleKeyInfo key;
            do
            {
                key = Console.ReadKey();
                result = key.Key == ConsoleKey.Y;
            }
            while (!(key.Key == ConsoleKey.Y || key.Key == ConsoleKey.N));
            
            Console.WriteLine();
            return result;
        }
    }


}
