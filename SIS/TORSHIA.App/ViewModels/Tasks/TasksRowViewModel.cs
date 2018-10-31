using System;
using System.Collections.Generic;
using System.Text;

namespace TORSHIA.App.ViewModels.Tasks
{
    public class TasksRowViewModel
    {
        public TasksRowViewModel()
        {
            this.Tasks = new List<TaskViewModel>();
        }

        public List<TaskViewModel> Tasks { get; set; }

        public string[] Empty => new string[5 - this.Tasks.Count];
    }
}
