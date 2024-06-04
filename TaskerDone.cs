namespace Tasker
{
    partial class Program
    {
        public class TaskerDone
        {
            public DateTime Completed { get; set; }
            public int Id { get; set; }
            public string Name { get; set; }
            public string Recurring { get; set; }
            public string Sleeping { get; set; }
            public string Description { get; set; }

            public TaskerDone()
            {
                Completed = DateTime.MinValue;
                Id = BAD_TASK_ID;
                Name = string.Empty;
                Recurring = string.Empty;
                Sleeping = NOT_SLEEPING;
                Description = string.Empty;
            }
        }

    }


}
