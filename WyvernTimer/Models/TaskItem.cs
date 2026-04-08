using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

 namespace WyvernTimer.Models
{
    public class TaskItem
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public List<TimeEntry> TimeEntries { get; set; }
        public bool IsRunning { get; set; }

        public String DisplayText
        {
            get
            { return $"{Name} - {GetTotalTrackedTime():hh\\:mm\\ss}";
            }
        }

        public TaskItem() 
        {  
            Name = string.Empty;
            Description = string.Empty;
            TimeEntries = new List<TimeEntry>();
            IsRunning = false;

        }
        public TimeSpan GetTotalTrackedTime()
        {
            return TimeSpan.FromSeconds(TimeEntries.Sum(entry => entry.Duration.TotalSeconds));
        }
    }
}
