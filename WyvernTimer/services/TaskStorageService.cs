using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using WyvernTimer.Models;


namespace WyvernTimer.Services
{
    public class TaskStorageService
    {
        private readonly string _filePath;

        public TaskStorageService()
        {
            _filePath = Path.Combine(FileSystem.AppDataDirectory, "tasks.json");
        }

        public async Task SaveTasksAsync(List<TaskItem> tasks)
        {
            var options = new JsonSerializerOptions
            {
                WriteIndented = true
            };

            string json = JsonSerializer.Serialize(tasks, options);
            await File.WriteAllTextAsync(_filePath, json);
        }

        public async Task<List<TaskItem>> LoadTasksAsync()
        {
            if (!File.Exists(_filePath))
            {
                return new List<TaskItem>();
            }

            string json = await File.ReadAllTextAsync(_filePath);

            List<TaskItem>? tasks = JsonSerializer.Deserialize<List<TaskItem>>(json);

            return tasks ?? new List<TaskItem>();
        }
    }
}