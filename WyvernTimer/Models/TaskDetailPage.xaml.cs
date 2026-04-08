using System;
using System.Collections.Generic;
using Microsoft.Maui.Controls;
using WyvernTimer.Models;
using WyvernTimer.Services;

namespace WyvernTimer
{
    public partial class TaskDetailPage : ContentPage
    {
        private readonly TaskItem _task;
        private readonly TaskStorageService _taskStorageService;
        private readonly List<TaskItem> _allTasks;

        private bool _isTimerRunning;
        private DateTime _currentSessionStartTime;

        public TaskDetailPage(TaskItem task, List<TaskItem> allTasks)
        {
            InitializeComponent();

            _task = task;
            _allTasks = allTasks;
            _taskStorageService = new TaskStorageService();

            LoadTaskDetails();
        }

        private void LoadTaskDetails()
        {
            TaskNameEntry.Text = _task.Name;
            TaskDescriptionEntry.Text = _task.Description;
            TotalTimeLabel.Text = _task.GetTotalTrackedTime().ToString(@"hh\:mm\:ss");

            if (_task.IsRunning)
            {
                TaskStatusLabel.Text = "Task is currently running.";
                StartButton.IsEnabled = false;
                StopButton.IsEnabled = true;
            }
            else
            {
                TaskStatusLabel.Text = "Task is not running.";
                StartButton.IsEnabled = true;
                StopButton.IsEnabled = false;
                ElapsedTimeLabel.Text = "00:00:00";
            }

            RefreshSessionLog();
        }

        private void RefreshSessionLog()
        {
            SessionLogCollectionView.ItemsSource = null;
            SessionLogCollectionView.ItemsSource = _task.TimeEntries;
        }

        private async void OnSaveChangesClicked(object sender, EventArgs e)
        {
            string updatedName = TaskNameEntry.Text?.Trim() ?? string.Empty;
            string updatedDescription = TaskDescriptionEntry.Text?.Trim() ?? string.Empty;

            if (string.IsNullOrWhiteSpace(updatedName))
            {
                await DisplayAlert("Validation Error", "Task name cannot be empty.", "OK");
                return;
            }

            _task.Name = updatedName;
            _task.Description = updatedDescription;

            await _taskStorageService.SaveTasksAsync(_allTasks);

            TaskStatusLabel.Text = "Task details updated successfully.";
        }

        private async void OnStartTimerClicked(object sender, EventArgs e)
        {
            try
            {
                _currentSessionStartTime = DateTime.Now;
                _isTimerRunning = true;
                _task.IsRunning = true;

                StartButton.IsEnabled = false;
                StopButton.IsEnabled = true;
                TaskStatusLabel.Text = "Task is currently running.";

                StartLiveTimer();

                await _taskStorageService.SaveTasksAsync(_allTasks);
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", ex.Message, "OK");
            }
        }

        private async void OnStopTimerClicked(object sender, EventArgs e)
        {
            try
            {
                if (!_isTimerRunning)
                {
                    await DisplayAlert("Error", "No timer is currently running.", "OK");
                    return;
                }

                DateTime endTime = DateTime.Now;
                TimeSpan duration = endTime - _currentSessionStartTime;

                string comment = await DisplayPromptAsync(
                    "Session Comment",
                    "Enter an optional comment for this session:",
                    initialValue: "",
                    maxLength: 200,
                    keyboard: Keyboard.Text);

                TimeEntry completedEntry = new TimeEntry
                {
                    StartTime = _currentSessionStartTime,
                    EndTime = endTime,
                    Duration = duration,
                    Comment = comment ?? string.Empty
                };

                _task.TimeEntries.Add(completedEntry);
                _task.IsRunning = false;
                _isTimerRunning = false;

                await _taskStorageService.SaveTasksAsync(_allTasks);

                ElapsedTimeLabel.Text = "00:00:00";
                LoadTaskDetails();
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", ex.Message, "OK");
            }
        }

        private void StartLiveTimer()
        {
            Dispatcher.StartTimer(TimeSpan.FromSeconds(1), () =>
            {
                if (!_isTimerRunning)
                {
                    return false;
                }

                TimeSpan elapsed = DateTime.Now - _currentSessionStartTime;
                ElapsedTimeLabel.Text = elapsed.ToString(@"hh\:mm\:ss");

                return true;
            });
        }

        private async void OnDeleteTaskClicked(object sender, EventArgs e)
        {
            if (_task.IsRunning)
            {
                await DisplayAlert("Delete Not Allowed", "Stop the timer before deleting this task.", "OK");
                return;
            }

            bool confirmDelete = await DisplayAlert(
                "Confirm Delete",
                $"Are you sure you want to delete '{_task.Name}'?",
                "Yes",
                "No");

            if (!confirmDelete)
            {
                return;
            }

            _allTasks.Remove(_task);
            await _taskStorageService.SaveTasksAsync(_allTasks);

            await DisplayAlert("Task Deleted", $"'{_task.Name}' has been deleted.", "OK");
            await Navigation.PopAsync();
        }
    }
}