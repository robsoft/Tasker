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

        private static string GetTasksFile()
        {
            //todo go find this in the regular places if not in immediate dir
            return Path.Combine(Directory.GetCurrentDirectory(), "tasks.txt");
        }


        private static string GetDoneFile()
        {
            //todo go find this in the regular places if not in immediate dir
            return Path.Combine(Directory.GetCurrentDirectory(), "done.txt");
        }


        private static string GetTasksBakFile()
        {
            return Path.ChangeExtension(GetTasksFile(), ".bak");
        }


        private static string GetDoneBakFile()
        {
            return Path.ChangeExtension(GetDoneFile(), ".bak");
        }


        private static string GetDailyBackupTasksName()
        {
            var name = Path.GetDirectoryName(GetTasksFile());
            var date = DateTime.Today.AddDays(-1);
            var bak = $"{date:yyyy-MM-dd}_tasks.txt";
            return Path.Combine(name, bak);
        }


        private static string GetDailyBackupDoneName()
        {
            var name = Path.GetDirectoryName(GetDoneFile());
            var date = DateTime.Today.AddDays(-1);
            var bak = $"{date:yyyy-MM-dd}_done.txt";
            return Path.Combine(name, bak);
        }


        private static bool AssertDailyBackupTasks()
        {
            var file = GetDailyBackupTasksName();
            if (File.Exists(file))
            {
                return true;
            }

            CreateBackup(GetTasksFile(), file);
            return false;
        }


        private static bool AssertDailyBackupDone()
        {
            var file = GetDailyBackupDoneName();
            if (File.Exists(file))
            {
                return true;
            }

            CreateBackup(GetDoneFile(), file);
            return false;
        }


        private static void LoadTasks()
        {
            using (var reader = new StreamReader(GetTasksFile()))
            {
                var csvConf = new CsvConfiguration(CultureInfo.InvariantCulture)
                {
                    HasHeaderRecord = true,
                    Delimiter = ","
                };
                using (var csv = new CsvReader(reader, csvConf))
                {
                    taskerTasks.Clear();
                    taskerTasks.AddRange(csv.GetRecords<TaskerTask>());
                }
                CheckForActive();
            }
        }


        private static void CheckForActive()
        {
            var changed = false;
            foreach (var task in taskerTasks)
            {
                if (task.Sleeping != NOT_SLEEPING)
                {
                    // can we parse the sleeping time?
                    if (DateTime.TryParse(task.Sleeping, out var activeTime))
                    {
                        // if the time has passed, make the task active
                        if (activeTime < DateTime.Now)
                        {
                            task.Sleeping = string.Empty;
                            changed = true;
                            GeneralOutput($"Task {task.Id} ({task.Name}) now awake");
                        }
                    }
                    else
                    {
                        if (task.Sleeping != "0")
                        {
                            GeneralError($"Task {task.Id} ({task.Name}) has an invalid sleeping time: {task.Sleeping}");
                        }
                    }
                }
            }

            if (changed)
            {
                SaveTasks();
            }
        }


        private static void CleanBackups()
        {
            GeneralOutput("not implemented yet");
            //var dir = Path.GetDirectoryName(GetTasksFile());
        }


        private static void SaveTasks()
        {
            if (!AssertDailyBackupTasks())   // if we just made the daily backup, don't make the within-day backup
            {
                CreateBackup(GetTasksFile(), GetTasksBakFile());
            }

            using (var writer = new StreamWriter(GetTasksFile()))
            {
                var csvConf = new CsvConfiguration(CultureInfo.InvariantCulture)
                {
                    HasHeaderRecord = true,
                    Delimiter = ","
                };
                using (var csv = new CsvWriter(writer, csvConf))
                {
                    csv.WriteRecords(taskerTasks);
                }
            }
        }


        private static void LoadDone()
        {
            using (var reader = new StreamReader(GetDoneFile()))
            {
                var csvConf = new CsvConfiguration(CultureInfo.InvariantCulture)
                {
                    HasHeaderRecord = true,
                    Delimiter = ","
                };
                using (var csv = new CsvReader(reader, csvConf))
                {
                    taskerDone.Clear();
                    taskerDone.AddRange(csv.GetRecords<TaskerDone>());
                }
            }
        }


        private static void SaveDone()
        {
            if (!AssertDailyBackupDone())   // if we just made the daily backup, don't make the within-day backup
            {
                CreateBackup(GetDoneFile(), GetDoneBakFile());
            }

            using (var writer = new StreamWriter(GetDoneFile()))
            {
                var csvConf = new CsvConfiguration(CultureInfo.InvariantCulture)
                {
                    HasHeaderRecord = true,
                    Delimiter = ","
                };
                using (var csv = new CsvWriter(writer, csvConf))
                {
                    csv.WriteRecords(taskerDone);
                }
            }
        }


        private static void CreateBackup(string source, string dest)
        {
            File.Copy(source, dest, true);
        }


    }
}
