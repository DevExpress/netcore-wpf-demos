using DevExpress.Mvvm.POCO;
using System;
using System.Linq;
using System.Collections.Generic;
using DevExpress.Data;
using System.Collections.ObjectModel;
using DevExpress.Utils;
using System.Windows;
using DevExpress.XtraGrid;
using DevExpress.Mvvm;
using DevExpress.Data.Filtering.Helpers;

namespace DevExpress.MailClient.ViewModel {

    public enum TaskItemType {
        List,
        ToDoItem,
        CompletedItem,
        TodayItem,
        PrioritizedItem,
        OverdueItem,
        SimpleListItem,
        DeferredItem
        }

    public class TasksViewModel : ContentViewModelBase<TaskItemViewModel> {
        public static TasksViewModel Create() {
            return ViewModelSource.Create(() => new TasksViewModel());
        }

        public IList<GridColumnViewModel> Columns { get; private set; }
        public IList<ContactItemViewModel> Employees { get; private set; }
        public virtual string FilterString { get; set; }
        public virtual TaskItemViewModel SelectedItem { get; set; }
        public virtual TaskItemType CheckedItemType { get; set; }
        public virtual ContactItemViewModel SelectedEmployee { get; set; }
        public virtual bool IsLoading { get; set; }

        protected TasksViewModel()
            : base(ModuleType.Tasks) {
            InitializeColumns();
            SetListView();
            CheckedItemType = TaskItemType.List;
        }

        #region Commands
        public void CreateNewTask() {
            EditTaskCore(TaskItemViewModel.Create("New Task", TaskCategory.Work), true);
        }
        public void EditTask() {
            if(SelectedItem == null)
                return;
            EditTaskCore(SelectedItem.Clone(), false);
        }
        void EditTaskCore(TaskItemViewModel task, bool isNew) {
            if(this.GetService<IDialogService>().ShowDialog(MessageButton.OKCancel, task.Subject, "EditTaskView", task) != MessageResult.OK)
                return;

            if(!isNew) {
                SelectedItem.Assign(task);
                this.GetService<INotificationService>().CreatePredefinedNotification("Task Changed", task.Subject, "").ShowAsync();
            } else {
                task.AssignTo = SelectedEmployee;
                Items.Add(task);
                ItemsSource.Add(task);
                SelectedItem = task;
                this.GetService<INotificationService>().CreatePredefinedNotification("New Task Created", task.Subject, "").ShowAsync();
            }
        }
        public void DeleteTask() {
            this.GetService<INotificationService>().CreatePredefinedNotification("Task Deleted", SelectedItem.Subject, "").ShowAsync();
            Items.Remove(SelectedItem);
            ItemsSource.Remove(SelectedItem);
        }
        public void FollowUp_Today() { FollowUp(FlagStatus.Today); }
        public void FollowUp_Tomorrow() { FollowUp(FlagStatus.Tomorrow); }
        public void FollowUp_ThisWeek() { FollowUp(FlagStatus.ThisWeek); }
        public void FollowUp_NextWeek() { FollowUp(FlagStatus.NextWeek); }
        public void FollowUp_NoDate() { FollowUp(FlagStatus.NoDate); }
        public void FollowUp_Custom() { FollowUp(FlagStatus.Custom); }
        void FollowUp(FlagStatus status) {
            switch(status) {
                case FlagStatus.Today: SelectedItem.DueDate = DateTime.Today;
                    break;
                case FlagStatus.Tomorrow: SelectedItem.DueDate = DateTime.Today.AddDays(1);
                    break;
                case FlagStatus.ThisWeek: SelectedItem.DueDate = EvalHelpers.GetWeekStart(DateTime.Today).AddDays(5);
                    break;
                case FlagStatus.NextWeek: SelectedItem.DueDate = EvalHelpers.GetWeekStart(DateTime.Today).AddDays(12);
                    break;
                case FlagStatus.NoDate: SelectedItem.DueDate = null;
                    break;
                case FlagStatus.Custom:
                    var model = CustomFlagViewModel.Create(SelectedItem.StartDate, SelectedItem.DueDate);
                    if(this.GetService<IDialogService>().ShowDialog(MessageButton.OKCancel, "Custom", "CustomFlagView", model) == MessageResult.OK) {
                        SelectedItem.StartDate = model.StartDate;
                        SelectedItem.DueDate = model.DueDate;
                    }
                    break;
            }
            SelectedItem.Complete = false;
        }
        public void SetListView() {
            ChangeColumnCompletedDateVisibility(true);
            ClearModel();
            IsLoading = true;
            GroupBy("CreatedDate");
            SortBy("CreatedDate", ColumnSortOrder.Descending);
            IsLoading = false;
        }
        public void SetToDoListView() {
            ChangeColumnCompletedDateVisibility(false);
            ClearModel();
            IsLoading = true;
            FilterString = "[Status] <> 'Completed' And [DueDate] Is Not Null";
            GroupBy("DueDate");
            SortBy("DueDate", ColumnSortOrder.Ascending);
            IsLoading = false;
        }
        public void SetCompletedView() {
            ChangeColumnCompletedDateVisibility(false);
            ClearModel();
            IsLoading = true;
            FilterString = "[Status] = 'Completed'";
            GroupBy("CompletedDate");
            SortBy("CompletedDate", ColumnSortOrder.Descending);
            IsLoading = false;
        }
        public void SetTodayView() {
            ChangeColumnCompletedDateVisibility(false);
            ClearModel();
            FilterString = "IsOutlookIntervalToday([DueDate])";
        }
        public void SetPrioritizedView() {
            ChangeColumnCompletedDateVisibility(true);
            ClearModel();
            IsLoading = true;
            GroupBy("Category");
            GroupBy("Priority");
            SortBy("Priority", ColumnSortOrder.Descending);
            SortBy("DueDate", ColumnSortOrder.Ascending);
            IsLoading = false;
        }
        public void SetOverdueView() {
            ChangeColumnCompletedDateVisibility(false);
            ClearModel();
            IsLoading = true;
            FilterString = "[Overdue] = 'True'";
            GroupBy("DueDate");
            SortBy("DueDate", ColumnSortOrder.Ascending);
            IsLoading = false;
        }
        public void SetSimpleListView() {
            ChangeColumnCompletedDateVisibility(false);
            ClearModel();
            IsLoading = true;
            SortBy("DueDate", ColumnSortOrder.Ascending);
            IsLoading = false;
        }
        public void SetDeferredView() {
            ChangeColumnCompletedDateVisibility(true);
            ClearModel();
            IsLoading = true;
            GroupBy("CompletedDate");
            SortBy("CreatedDate", ColumnSortOrder.Ascending);
            FilterString = "[Status] = 'Deferred' Or [Status] = 'WaitingOnSomeoneElse'";
            IsLoading = false;
        }
        #endregion

        protected override void InitializeItems() {
            base.InitializeItems();
            Employees = Items.Where(x => x.AssignTo != null && x.AssignTo.Photo != null).Select(x => x.AssignTo).Distinct().ToList();
            //Use as all value
            Employees.Add(ContactItemViewModel.Empty);
            SelectedEmployee = Employees[0];
        }
        protected override void InitializeInDesignMode() {
            SelectedItem = ItemsSource[0];
        }
        protected override void UpdateItemsSource() {
            if(SelectedEmployee == null)
                return;

            var result = SelectedEmployee == ContactItemViewModel.Empty ? Items : Items.Where(t => t.AssignTo == SelectedEmployee);
            ItemsSource = new ObservableCollection<TaskItemViewModel>(result);
        }
        protected virtual void OnSelectedEmployeeChanged() {
            UpdateItemsSource();
        }
        protected virtual void GroupBy(string fieldName) {
            Columns.First(c => c.FieldName == fieldName).IsGrouped = true;
        }
        protected void SortBy(string fieldName, ColumnSortOrder order) {
            Columns.First(c => c.FieldName == fieldName).SortOrder = order;
        }
        protected void ClearModel() {
            ForEachColumn(x => x.IsGrouped = false);
            ForEachColumn(x => x.SortOrder = ColumnSortOrder.None);
            FilterString = null;
            IsLoading = true;
            for(int i = 0; i < Columns.Count; i++)
                Columns[i].Index = i;
            IsLoading = false;
        }
        void ForEachColumn(Action<GridColumnViewModel> action) {
            foreach(var column in Columns)
                action(column);
        }
        void InitializeColumns() {
            Columns =  new List<GridColumnViewModel>();
            Columns.Add(GridColumnViewModel.Create(x => {
                x.FieldName = "Complete";
                x.Width = 45;
                x.HorizontalHeaderContentAlignment = HorizontalAlignment.Center;
                x.AllowFiltering = DefaultBoolean.False;
                x.AllowSorting = DefaultBoolean.False;
            }));
            Columns.Add(GridColumnViewModel.Create(x => {
                x.FieldName = "Icon";
                x.Width = 40;
                x.HorizontalHeaderContentAlignment = HorizontalAlignment.Center;
                x.AllowFiltering = DefaultBoolean.False;
                x.AllowSorting = DefaultBoolean.False;
            }));
            Columns.Add(GridColumnViewModel.Create(x => {
                x.FieldName = "Priority";
                x.Width = 40;
            }));
            Columns.Add(GridColumnViewModel.Create(x => {
                x.FieldName = "Subject";
                x.Width = 150;
                x.DisplayName = "Task Subject";
                x.AllowEditing = DefaultBoolean.False;
            }));
            Columns.Add(GridColumnViewModel.Create(x => {
                x.FieldName = "Status";
                x.Width = 100;
            }));
            Columns.Add(GridColumnViewModel.Create(x => {
                x.FieldName = "PercentComplete";
            }));
            Columns.Add(GridColumnViewModel.Create(x => {
                x.FieldName = "CreatedDate";
                x.Width = 90;
                x.DisplayName = "Date Created";
                x.GroupInterval = ColumnGroupInterval.DateRange;
            }));
            Columns.Add(GridColumnViewModel.Create(x => {
                x.FieldName = "StartDate";
                x.Width = 90;
            }));
            Columns.Add(GridColumnViewModel.Create(x => {
                x.FieldName = "DueDate";
                x.Width = 90;
                x.GroupInterval = ColumnGroupInterval.DateRange;
            }));
            Columns.Add(GridColumnViewModel.Create(x => {
                x.FieldName = "CompletedDate";
                x.Width = 90;
                x.DisplayName = "Date Completed";
                x.AllowEditing = DefaultBoolean.False;
                x.GroupInterval = ColumnGroupInterval.DateRange;
            }));
            Columns.Add(GridColumnViewModel.Create(x => {
                x.FieldName = "Category";
                x.AllowEditing = DefaultBoolean.False;
            }));
            Columns.Add(GridColumnViewModel.Create(x => {
                x.FieldName = "FlagStatus";
                x.Width = 40;
            }));
        }
        void ChangeColumnCompletedDateVisibility(bool value) {
            Columns.First(column => column.FieldName == "CompletedDate").IsVisible = value;
        }
    }

    public class CustomFlagViewModel {
        public static CustomFlagViewModel Create(DateTime? startDate, DateTime? dueDate) {
            return ViewModelSource.Create(() => new CustomFlagViewModel(startDate, dueDate));
        }

        public virtual DateTime? StartDate { get; set; }
        public virtual DateTime? DueDate { get; set; }

        protected CustomFlagViewModel() {
            if(this.IsInDesignMode()) {
                StartDate = DateTime.Today.Date;
                DueDate = DateTime.Today.Date.AddDays(5);
            }
        }
        protected CustomFlagViewModel(DateTime? startDate, DateTime? dueDate) {
            this.StartDate = startDate;
            this.DueDate = dueDate;
        }
    }

}
