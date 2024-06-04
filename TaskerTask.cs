//todo:
/*
 * - make sleeping string be just a zero for things that are slept,
 * otherwise it signals when the task will become active
 * 
 * - renum command
 * - clean command
 * 
 * - when we load tasks,
 *   if the datetime in 'sleeping' has passed, we remove it to make the task active
 * 
 * - ability to fetch the files from somewhere else (tasker.cfg)
 * - also a 'quiet' option in cfg
 * - cfg file auto-generated if not present
 * 
 * - test on Mac
 * - put in GH as public
 */

namespace Tasker
{
    partial class Program
    {
        public class TaskerTask
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string Recurring { get; set; }
            public string Sleeping { get; set; }
            public string Description { get; set; }

            public TaskerTask()
            {
                Id = BAD_TASK_ID;
                Name = string.Empty;
                Recurring = string.Empty;
                Sleeping = NOT_SLEEPING;
                Description = string.Empty;
            }
        }

    }


}
