# Tasker  
Tasker © 2024 by Rob Uttley is licensed under CC BY 4.0. To view a copy of this license, visit https://creativecommons.org/licenses/by/4.0/

* command-line, Windows & Mac  
* just needs to be somewhere on the system path  
* in the same folder as the executable are the tasks.txt and done.txt  
* possible future option to have a redirect so that the 2 files could be in dropbox, onedrive etc **tasker.cfg** etc  
* **tasks.txt** is the list of active and sleeping tasks  
* **done.txt** is just a list of tasks that have been marked as done, with a timestamp of when they were done
* always creates a backup of **tasks.txt** and **done.txt** at the start of the day (the first time it's used in a given day), 
in the format *yyyy-mm-dd.tasks.txt* and *yyyy-mm-dd.done.txt*
* and then after that a simple .bak is maintained for both, which is the previous version of the file throughout the day  
* eg;  
```  
  tasks.txt              <- current tasks (and sleeping tasks)
  done.txt               <- done tasks
  tasks.txt.bak          <- previous version of tasks
  done.txt.bak           <- previous version of done
  2024-01-03.tasks.txt   <- tasks at end of 3-jan-2024  
  2024-01-03.done.txt    <- done at end of 3-jan-2024  
```


## tasker + taskname;tasknotes 
  * add taskname with optional notes (both strings can be " escaped)  
  * console returns the task id  
  * priority implied by number of + (so + is default, ++ is higher, +++ highest)  
  * eg **tasker + "My First Task";"This is a note"**
  * sample output;
```  
  1 'My First Task' added  
```
### additional options for tasker + command;
  * **-rd** daily recurrence generated when task is done  
  * **-rw** weekly (etc)  
  * **-rm** monthly  
  * **-ra** annual  
  * **-rn** n is number of days (so r1=rd, r7=rw etc)  
  * **-s** start off as a sleeping task  


## tasker list
  * shows all outstanding tasks in order of priority and then id  
  * eg **tasker list**  
  * sample output;
```  
  Active tasks;
  1 My first task  
    This is a note  
  2 Another task  
  3 A Recurring Task         (daily)  
  4 Another task  
    Which has a note
```
  * supports -sleep option to show sleeping tasks  
  * eg **tasker list -s**  
  * sample output;
```  
  Sleeping tasks;
  2 Another task  
```
  * supports -done option to show done tasks  
  * eg **tasker list -done**  
  * sample output;
```  
  Completed tasks;
  2024-05-01 2 Another task  
```

## tasker - id *(or taskname)*
  * mark a task as done (adds to done list with timestamp of when done)
  * eg **tasker - "Myfirst task"**
  * eg **tasker - 2**
  * sample output;
```  
  2 'Another task' completed  
```


## tasker rem id *(or taskname)*
  * just remove a task (don't move to done list) 
  * will prompt y/n
  * eg **tasker rem 2**
  * sample output;
```  
  Remove task 2 'Another task'? (Y/N) Y  
  2 'Another task' removed  
```

## tasker renum
  * will prompt y/n
  * renumber all the ids of the outstanding list,  
  * then print it out (as per list)
  * eg **tasker renum**  
  * sample output;
```  
  Renumber tasks? (Y/N) Y  
  1 My first task  
    This is a note  
  2 Another task  
  3 A Recurring Task         (daily)  
  4 Another task  
    Which has a note
```

## tasker sleep id *(or taskname)*
  * suppress this task for the time being
  * eg **tasker sleep 2**
  * sample output;
```  
  2 'Another task' sleeping
```

## tasker wake id *(or taskname)*
  * reactivate this task
  * eg **tasker wake 2**
  * sample output;
```  
  2 'Another task' active
```


## tasker clean
  * delete all the backup files
  * will prompt y/n  
  * eg **tasker clean**
  * sample output;
```  
  Clean-up backup files (Y/N) Y  
  15 files deleted
```


## tasker -h
  * help
  * also -help, --help and /? are supported  









