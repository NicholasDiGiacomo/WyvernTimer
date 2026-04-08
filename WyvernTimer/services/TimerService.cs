using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WyvernTimer.Models;

namespace WyvernTimer.Services
{
    public class TimerService
    {
        private TimeEntry? _currentEntry;
        private TaskItem? _runningTask;

        public bool IsRunning => _runningTask != null && _currentEntry != null;

        public DateTime? CurrentStartTime => _currentEntry?.StartTime;

        public void StartTimer(TaskItem task)
        {
            if (_runningTask != null)
            {
                throw new InvalidOperationException("This task is already running.");
            }
            if (task.IsRunning) {
                throw new InvalidOperationException("This task is already running.");

            }
            _currentEntry = new TimeEntry
            {
                StartTime = DateTime.Now
            };

            _runningTask = task;

            task.IsRunning = true;
            
        }

        public void StopTimer(TaskItem task)
        {
            if (_runningTask == null || _currentEntry == null)
            {
                throw new InvalidOperationException("No timer is currently running.");
            }
            if(_runningTask != task)
            {
                throw new InvalidOperationException("This is not the currently running task.");
            }

            _currentEntry.EndTime = DateTime.Now;
            _currentEntry.Duration = _currentEntry.EndTime.Value - _currentEntry.StartTime;


            task.TimeEntries.Add(_currentEntry);
            task.IsRunning = false;

            _currentEntry = null;
            _runningTask = null;
        }
    }
}
