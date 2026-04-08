using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Maui.Controls;
using WyvernTimer.Models;


using WyvernTimer.Services;




namespace WyvernTimer
{

    public partial class MainPage : ContentPage
    {

        private List<TaskItem> _tasks;
        
        private readonly TimerService _timerService;
        private readonly TaskStorageService _taskStorageService;
        private int _nextTaskId = 1;


        public MainPage()
        {
            InitializeComponent();
            _tasks = new List<TaskItem>();
            _timerService = new TimerService();
           _taskStorageService = new TaskStorageService();

            LoadTasks();
        }

        private async void LoadTasks()
        {
            _tasks = await _taskStorageService.LoadTasksAsync();

            if (_tasks.Any())
            {
                _nextTaskId = _tasks.Max(task => task.Id) + 1;
            }
            RefreshTaskList();
        }

        private async void OnCreateTaskClicked(object sender, EventArgs e)
        {
            string taskName = TaskNameEntry.Text?.Trim() ?? string.Empty;
            string taskDescription = TaskDescriptionEntry.Text?.Trim() ?? string.Empty;

            if (string.IsNullOrWhiteSpace(taskName))
            {
                TaskStatusLabel.Text = "Please enter a task name.";
                return;
            }

            TaskItem newTask = new TaskItem
            {
                Id = _nextTaskId++,
                Name = taskName,
                Description = taskDescription
            };

            _tasks.Add(newTask);

            await _taskStorageService.SaveTasksAsync(_tasks);

            TaskNameEntry.Text = string.Empty;
            TaskDescriptionEntry.Text = string.Empty;
            TaskStatusLabel.Text = $"Task '{newTask.Name}' created.";
            

            RefreshTaskList();


            
        }


        private async void OnTaskSelected(object sender, SelectionChangedEventArgs e)
        {
            TaskItem? selectedTask = e.CurrentSelection.FirstOrDefault() as TaskItem;

            if (selectedTask == null)
            {
                return;
            }

            TaskStatusLabel.Text = $"Opening task: {selectedTask.Name}";

            await Navigation.PushAsync(new TaskDetailPage(selectedTask, _tasks));

            TaskCollectionView.SelectedItem = null;
        }







        private void RefreshTaskList()
        {
            TaskCollectionView.ItemsSource = null;
            TaskCollectionView.ItemsSource = _tasks;

        }
        protected override async void OnAppearing()
        {
            base.OnAppearing();

            _tasks = await _taskStorageService.LoadTasksAsync();
            RefreshTaskList();
        }
    }

    
}



