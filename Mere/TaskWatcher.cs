using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mere
{
    public class TaskWatcher
    {
        public int Throttle { get; private set; }

        public bool HasTasks
        {
            get { return _runningTasks.Count > 0 || _pendingTasks.Count > 0; }
        }

        public bool HasRunningTasks
        {
            get { return _runningTasks.Count > 0; }
        }

        public bool HasPendingTasks
        {
            get { return _pendingTasks.Count > 0; }
        }

        private bool _watchingTasks;

        private Dictionary<int, Task> _runningTasks;
        private Dictionary<int, Task> _pendingTasks;
        private Task _watchingRunningTask;
        private Task _watchingPendingTask;

        private TaskWatcher(int throttle)
        {
            Throttle = throttle;
            _runningTasks = new Dictionary<int, Task>();
            _pendingTasks = new Dictionary<int, Task>();
        }

        public static TaskWatcher CreateNew(int throttle)
        {
            return new TaskWatcher(throttle);
        }

        public static TaskWatcher StartNew(int throttle)
        {
            var tw = new TaskWatcher(throttle);

            tw.StartWatching();

            return tw;
        }

        public static TaskWatcher StartNew(int throttle, Task task)
        {
            var tw = new TaskWatcher(throttle);

            tw.StartWatching();
            tw.AddTask(task);

            return tw;
        }

        public event EventHandler OnAllTaskCompleted;

        private void AllTaskCompleted(EventArgs e)
        {
            if (OnAllTaskCompleted != null)
                OnAllTaskCompleted(this, e);
        }

        public event EventHandler OnTaskCompleted;

        private void TaskCompleted(EventArgs e)
        {
            if (OnTaskCompleted != null)
                OnTaskCompleted(this, e);
        }

        public void StartWatching()
        {
            if(!_watchingTasks)
                _watchingTasks = true;

            if(_watchingRunningTask == null || _watchingRunningTask.Status != TaskStatus.Running)
                _watchingRunningTask = Task.Run(() => WatchRunningTasks());

            if (_watchingPendingTask == null || _watchingPendingTask.Status != TaskStatus.Running)
                _watchingPendingTask = Task.Run(() => WatchPendingTasks());
        }

        public async void StopWatching()
        {
            _watchingTasks = false;

            await Task.WhenAll(_watchingRunningTask);
            await Task.WhenAll(_watchingPendingTask);

            while (_runningTasks.Count > 0)
            {
                await CheckRunningTasks();
            }

            AllTaskCompleted(new EventArgs());
        }

        public void AddTask(Task task)
        {
            if ((_runningTasks.Count <= 0 || Throttle > _runningTasks.Count) && _pendingTasks.Count <= 0)
            {
                task.Start();
                _runningTasks.Add(task.Id, task);
            }
            else
                _pendingTasks.Add(task.Id, task);
        }

        private async void WatchRunningTasks()
        {
            while (_watchingTasks)
            {
                await CheckRunningTasks();
            }
        }

        private async void WatchPendingTasks()
        {
            while (_watchingTasks)
            {
                await CheckPendingTasks();
            }
        }

        private async Task<Task> CheckRunningTasks()
        {
            if (_runningTasks != null && _runningTasks.Count > 0)
            {
                var finished = await Task.WhenAny(_runningTasks.Values);
                _runningTasks.Remove(finished.Id);

                TaskCompleted(new EventArgs());
            }

            return Task.Delay(1);
        }

        private Task CheckPendingTasks()
        {
            if (_pendingTasks != null && _pendingTasks.Count > 0)
            {
                if (_runningTasks == null)
                    _runningTasks = new Dictionary<int, Task>();

                if (_runningTasks.Count <= 0 || Throttle > _runningTasks.Count)
                {
                    var t = _pendingTasks.Values.First();
                    _pendingTasks.Remove(t.Id);
                    t.Start();

                    _runningTasks.Add(t.Id, t);
                }
            }

            return Task.Delay(1);
        }
    }
}
