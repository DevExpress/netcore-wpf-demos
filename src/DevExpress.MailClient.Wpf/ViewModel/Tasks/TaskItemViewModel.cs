using DevExpress.Mvvm.DataAnnotations;
using DevExpress.Mvvm.POCO;
using DevExpress.XtraEditors.DXErrorProvider;
using System;

namespace DevExpress.MailClient.ViewModel {
    public enum TaskStatus { NotStarted, InProgress, Completed, WaitingOnSomeoneElse, Deferred }
    public enum TaskCategory { HouseChores, Shopping, Work }
    public enum FlagStatus { Today, Tomorrow, ThisWeek, NextWeek, NoDate, Custom, Completed }
    public enum TaskPriority { Low, Medium, High }

    public class TaskItemViewModel : IDXDataErrorInfo {
        public static TaskItemViewModel Create(string subject, TaskCategory category, DateTime date) {
            return ViewModelSource.Create(() => new TaskItemViewModel(subject, category, date));
        }
        public static TaskItemViewModel Create(string subject, TaskCategory category) {
            return Create(subject, category, DateTime.Now);
        }

        #region Props
        public virtual TaskPriority Priority { get; set; }
        public virtual int PercentComplete { get; set; }
        public virtual DateTime? StartDate { get; set; }
        public virtual DateTime? DueDate { get; set; }
        public virtual DateTime? CompletedDate { get; set; }
        public virtual string Subject { get; set; }
        public virtual string Description { get; set; }
        public virtual TaskCategory Category { get; set; }
        public virtual TaskStatus Status { get; set; }
        public virtual ContactItemViewModel AssignTo { get; set; }

        public DateTime CreatedDate { get; private set; }
        public int Icon { get { return Complete ? 0 : 1; } }
        public bool Overdue { get { return Status != TaskStatus.Completed && DueDate.HasValue && DateTime.Now >= DueDate.Value.Date.AddDays(1); } }
        public bool Complete {
            get { return Status == TaskStatus.Completed; }
            set {
                Status = value ? TaskStatus.Completed : TaskStatus.NotStarted;
                this.RaisePropertyChanged(x => x.Complete);
                this.RaisePropertyChanged(x => x.FlagStatus);
            }
        }
        public FlagStatus FlagStatus {
            get {
                DateTime today = DateTime.Today;
                if(Complete) return FlagStatus.Completed;
                if(!DueDate.HasValue) return FlagStatus.NoDate;
                if(DueDate.Value.Date.Equals(today)) return FlagStatus.Today;
                if(DueDate.Value.Date.Equals(today.AddDays(1))) return FlagStatus.Tomorrow;
                DateTime thisWeekStart = DevExpress.Data.Filtering.Helpers.EvalHelpers.GetWeekStart(today);
                if(DueDate.Value.Date >= thisWeekStart && DueDate.Value.Date < thisWeekStart.AddDays(7)) return FlagStatus.ThisWeek;
                if(DueDate.Value.Date >= thisWeekStart.AddDays(7) && DueDate.Value.Date < thisWeekStart.AddDays(14)) return FlagStatus.NextWeek;
                return FlagStatus.Custom;
            }
        }
        internal TimeSpan TimeDiff { get { return (DateTime.Now - CreatedDate); } }
        #endregion

        protected TaskItemViewModel() { }
        protected TaskItemViewModel(string subject, TaskCategory category, DateTime date) {
            Subject = subject;
            Category = category;
            CreatedDate = date;
            Priority = TaskPriority.Medium;
        }

        public void ChangeComplete() {
            Complete = !Complete;
        }
        [Command(false)]
        public void Assign(TaskItemViewModel task) {
            this.Subject = task.Subject;
            this.Priority = task.Priority;
            this.PercentComplete = task.PercentComplete;
            this.CreatedDate = task.CreatedDate;
            this.StartDate = task.StartDate;
            this.DueDate = task.DueDate;
            this.CompletedDate = task.CompletedDate;
            this.Description = task.Description;
            this.Category = task.Category;
            this.Status = task.Status;
            this.AssignTo = task.AssignTo;
        }
        [Command(false)]
        public TaskItemViewModel Clone() {
            TaskItemViewModel task = TaskItemViewModel.Create(Subject, Category);
            task.Assign(this);
            return task;
        }

        protected void OnPercentCompleteChanged() {
            if(PercentComplete == 100)
                Status = TaskStatus.Completed;
            else if(PercentComplete > 0)
                Status = TaskStatus.InProgress;
        }
        protected void OnStartDateChanged() {
            this.RaisePropertyChanged(x => x.Overdue);
        }
        protected void OnDueDateChanged() {
            this.RaisePropertyChanged(x => x.Overdue);
        }
        protected void OnStatusChanged() {
            if(Status == TaskStatus.Completed) {
                PercentComplete = 100;
                CompletedDate = DateTime.Now;
            } else
                CompletedDate = null;
            if(Status == TaskStatus.NotStarted)
                PercentComplete = 0;
            if(Status == TaskStatus.InProgress && PercentComplete == 100)
                PercentComplete = 75;
            if(Status == TaskStatus.Deferred || Status == TaskStatus.WaitingOnSomeoneElse)
                DueDate = null;

            this.RaisePropertyChanged(x => x.Complete);
        }

        #region IDXDataErrorInfo Members
        void IDXDataErrorInfo.GetError(DevExpress.XtraEditors.DXErrorProvider.ErrorInfo info) { }
        void IDXDataErrorInfo.GetPropertyError(string propertyName, DevExpress.XtraEditors.DXErrorProvider.ErrorInfo info) {
            if(propertyName == "DueDate") {
                if((DueDate.HasValue && StartDate.HasValue) && DueDate < StartDate)
                    SetErrorInfo(info, "Due Date can't be less than Start Date", ErrorType.Critical);
                if(!DueDate.HasValue && Status == TaskStatus.InProgress)
                    SetErrorInfo(info, "Please enter Due Date", ErrorType.Warning);
            }
        }
        void SetErrorInfo(DevExpress.XtraEditors.DXErrorProvider.ErrorInfo info, string errorText, ErrorType errorType) {
            info.ErrorText = errorText;
            info.ErrorType = errorType;
        }
        #endregion
    }
}