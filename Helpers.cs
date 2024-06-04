using System.CommandLine;
using System.CommandLine.Invocation;
using CsvHelper;
using System.IO;
using System.Globalization;
using System;
using CsvHelper.Configuration;
using System.Threading.Tasks;
using System.Net;

namespace Tasker
{
    partial class Program
    {

        const int BAD_TASK_ID = 0;
        const string NOT_SLEEPING = "";


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
            Console.WriteLine($"Error : {message}");
        }


        private static void GeneralOutput(string message)
        {
            Console.WriteLine(message);
        }


        private static void SingleLineOutput(string message)
        {
            Console.Write(message);
        }


        private static void ListTasks(bool sleep = false)
        {
            if (sleep)
            {
                GeneralOutput("Sleeping Tasks");
                foreach (var record in taskerTasks.Where(x => x.Sleeping != NOT_SLEEPING))
                {
                    GeneralOutput($"{record.Id},{record.Name},{record.Recurring}");
                    if (!string.IsNullOrEmpty(record.Description))
                    {
                        GeneralOutput($"  {record.Description}");
                    }
                }

            }
            else
            {
                GeneralOutput("Active Tasks");
                foreach (var record in taskerTasks.Where(x => x.Sleeping == NOT_SLEEPING))
                {
                    GeneralOutput($"{record.Id},{record.Name},{record.Recurring}");
                    if (!string.IsNullOrEmpty(record.Description))
                    {
                        GeneralOutput($"  {record.Description}");
                    }
                }
            }
        }


        private static void ListDone()
        {
            foreach (var record in taskerDone)
            {
                GeneralOutput($"{record.Completed},{record.Id},{record.Name},{record.Recurring},{record.Sleeping}");
                if (!string.IsNullOrEmpty(record.Description))
                {
                    GeneralOutput($"  {record.Description}");
                }
            }
        }


        private static bool ShowYNPrompt(string prompt)
        {
            var result = false;
            SingleLineOutput($"{prompt} (Y/N) ");
            ConsoleKeyInfo key;
            do
            {
                key = Console.ReadKey();
                result = key.Key == ConsoleKey.Y;
            }
            while (!(key.Key == ConsoleKey.Y || key.Key == ConsoleKey.N));

            GeneralOutput(result ? "Yes" : "No");
            return result;
        }


    }


}
