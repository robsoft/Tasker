using Spectre.Console;

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
            // remove any protective quotes that were only there to help us get spaces passed in
            name = name.Trim('"');
            desc = desc.Trim('"');

            // sleeping by default?
            var sleep = sleeping ? GetNow() : NOT_SLEEPING;

            if (name == string.Empty)
            {
                GeneralError("Bad or missing task name");
                return;
            }

            if (recurs != string.Empty)
            {
                if (!ValidateRecurring(recurs))
                {
                    GeneralError($"Bad 'recurring' value ({recurs})");
                    return;
                }
            }

            LoadTasks();
            var id = GetNextId();
            var task = new TaskerTask()
            {
                Id = id,
                Name = name,
                Description = desc,
                Recurring = recurs,
                Sleeping = sleep
            };
            taskerTasks.Add(task);
            SaveTasks();

            GeneralOutput($"Added {task.Id} ({task.Name})");
            ListTasks(config.Verbose);
        }


        private static void DoSleep(string id)
        {
            LoadTasks();
            var task = GetTaskFromArgId(id);
            if (task == null)
            {
                GeneralError($"Task '{id}' not found");
                ListTasks(config.Verbose);
                return;
            }
            if (task.Sleeping != NOT_SLEEPING)
            {
                GeneralError($"Task '{id}' not awake");
            }
            else
            {
                task.Sleeping = SLEEPING;
                SaveTasks();
                GeneralOutput($"Task {task.Id} ({task.Name}) now sleeping");
            }

        }

        private static void DoList(bool done, bool sleep)
        {
                if (done)
                {
                    LoadDone();
                    if (taskerDone.Count == 0)  
                    {
                        GeneralOutput("No completed tasks");
                    }
                    else
                    {
                        ListDone(true);
                    }
                }
                else
                {
                    LoadTasks();
                    if (taskerTasks.Count==0)
                    {
                        GeneralOutput("No active tasks");
                    }
                    else
                    {
                        ListTasks(true, sleep);
                    }
                }
        }
    


        private static void DoWake(string id)
        {
            LoadTasks();
            var task = GetTaskFromArgId(id);
            if (task == null)
            {
                GeneralError($"Task '{id}' not found");
                ListTasks(config.Verbose);
                return;
            }

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


        private static void DoComplete(string id)
        {
            LoadTasks();
            var task = GetTaskFromArgId(id);
            if (task == null)
            {
                GeneralError($"Task '{id}' not found");
                ListTasks(config.Verbose);
                return;
            }
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
                if (task.Recurring != string.Empty)
                {
                    // calculate new sleeping time based on 'recurring' setting, update task
                    task.Sleeping = GetNextSleep(task.Recurring);
                    GeneralOutput($"Task {task.Id} ({task.Name}) will recur on {task.Sleeping}");
                }
                else
                {
                    GeneralOutput($"Task {task.Id} ({task.Name}) completed");
                    taskerTasks.Remove(task);
                }
                // either way, save the list
                SaveTasks();

            }
        }


        private static void DoRenum()
        {
            if (!ShowYNPrompt("Renumber tasks?")) return;
            GeneralOutput("renumber not implemented yet");
            LoadTasks();
            int id = 1;
            foreach(var task in taskerTasks.OrderBy(x=>x.Id).OrderBy(x=>x.Name))
            {
                task.Id = id++;
            }
            SaveTasks();
            ListTasks(config.Verbose);
        }


        private static void DoClean()
        {
            if (!ShowYNPrompt("Clean - all backups will be deleted?")) return;
            CleanBackups();
        }


        private static void DoClear(bool doneOnly)
        {
            if (doneOnly)
            {
                LoadDone();
                if (!ShowYNPrompt("Clear all completed tasks?")) return;
                taskerDone.Clear();
                SaveDone();
                GeneralOutput("All completed tasks removed");
            }
            else
            {
                LoadTasks();
                if (!ShowYNPrompt("Clear all tasks?")) return;
                taskerTasks.Clear();
                SaveTasks();
                GeneralOutput("All tasks removed");
            }
        }


        private static void DoRemove(string id)
        {
            LoadTasks();
            var task = GetTaskFromArgId(id);
            if (task == null)
            {
                GeneralError($"Task '{id}' not found");
                ListTasks(config.Verbose);
                return;
            }
            if (!ShowYNPrompt($"Remove task {task.Id} ({task.Name})?")) return;
            taskerTasks.Remove(task);
            SaveTasks();
            GeneralOutput($"Task {task.Id} ({task.Name}) removed");
        }


    }
}
