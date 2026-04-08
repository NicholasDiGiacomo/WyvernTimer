using System;

namespace WyvernTimer.Models
{
    public class TimeEntry
    {
        public int Id { get; set; }

        public DateTime StartTime { get; set; }

        public DateTime? EndTime { get; set; }

        public TimeSpan Duration { get; set; }

        public string Comment { get; set; } = string.Empty;

        public string DisplayText
        {
            get
            {
                string dateText = StartTime.ToString("g");
                string durationText = Duration.ToString(@"hh\:mm\:ss");

                if (string.IsNullOrWhiteSpace(Comment))
                {
                    return $"{dateText} - {durationText}";
                }

                return $"{dateText} - {durationText} - {Comment}";
            }
        }
    }
}