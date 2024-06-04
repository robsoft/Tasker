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

        private static void DoAdd(string taskName, string recurs, bool sleeping)
        {
            // break out the potential description in the taskName
            var name = taskName;
            var desc = NOT_SLEEPING;

            //todo: fix bug here if ; is inside quotes
            if (taskName.Contains(';'))
            {
                var split = taskName.Split(';');
                name = split[0];
                desc = split[1];
            }
            // remove any protective quotes
            name = name.Trim('"');
            desc = desc.Trim('"');

            // sleeping by default?
            var sleep = sleeping ? GetNow() : NOT_SLEEPING;

            //todo: validate 'recurs'

            if (name == string.Empty)
            {
                GeneralError("Bad or missing task name");
                return;
            }

            LoadTasks();
            var id = GetNextId();
            taskerTasks.Add(new TaskerTask()
            {
                Id = id,
                Name = name,
                Description = desc,
                Recurring = recurs,
                Sleeping = sleep
            }
            );
            SaveTasks();
            ListTasks();

        }

        private static void DoSleep(string id)
        {
            LoadTasks();
            var task = GetTaskFromArgId(id);
            if (task != null)
            {
                if (task.Sleeping != NOT_SLEEPING)
                {
                    GeneralError($"Task '{id}' not awake");
                }
                else
                {
                    task.Sleeping = GetNow();
                    SaveTasks();
                    GeneralOutput($"Task {task.Id} ({task.Name}) now sleeping");
                }
            }
            else
            {
                GeneralError($"Task '{id}' not found");
                ListTasks();
            }

        }

        private static void DoList(bool done, bool sleep)
        {
            if (done)
            {
                LoadDone();
                ListDone();
            }
            else
            {
                LoadTasks();
                ListTasks(sleep);
            }
        }

        private static void DoWake(string id)
        {
            LoadTasks();
            var task = GetTaskFromArgId(id);
            if (task != null)
            {
                if (task.Sleeping == NOT_SLEEPING)
                {
                    GeneralError($"Task '{id}' not sleeping");
                }
                else
                {
                    task.Sleeping = NOT_SLEEPING;
                    SaveTasks();
                    GeneralOutput($"Task {task.Id} ({task.Name}) now awake");
                }
            }
            else
            {
                GeneralError($"Task '{id}' not found");
                ListTasks();
            }

        }


        private static void DoComplete(string id)
        {
            LoadTasks();
            var task = GetTaskFromArgId(id);
            if (task != null)
            {
                if (task.Sleeping != NOT_SLEEPING)
                {
                    GeneralError($"Task '{id}' is sleeping");
                }
                else
                {
                    LoadDone();
                    taskerDone.Add(new TaskerDone()
                    {
                        Completed = DateTime.Now,
                        Id = task.Id,
                        Name = task.Name,
                        Description = task.Description,
                        Recurring = task.Recurring,
                        Sleeping = task.Sleeping
                    }
                    );
                    SaveDone();
                    //todo: manage recurring tasks
                    taskerTasks.Remove(task);
                    SaveTasks();
                    GeneralOutput($"Task {task.Id} ({task.Name}) completed");
                }
            }
            else
            {
                GeneralError($"Task '{id}' not found");
                ListTasks();
            }
        }


        private static void DoRenum()
        { }


        private static void DoClean()
        { }


        private static void DoRemove(string id)
        { }




    }
}
