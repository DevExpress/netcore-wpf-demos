using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Media;
using DevExpress.DevAV.Common;
using DevExpress.DevAV.Common.Utils;
using DevExpress.DevAV.Common.ViewModel;
using DevExpress.Mvvm;
using DevExpress.Mvvm.POCO;
using DevExpress.Mvvm.Localization;
using DevExpress.Mvvm.DataModel;
using System.Windows.Threading;

namespace DevExpress.DevAV.ViewModels {
    partial class EmployeeTaskViewModel {
        DateTime StartTimeOfWorkDay = new DateTime(1, 1, 1, 9, 0, 0);
        DateTime EndTimeOfWorkDay = new DateTime(1, 1, 1, 19, 0, 0);

        IReportService PrintService { get { return this.GetRequiredService<IReportService>("PrintService"); } }
        IReportService ExportService { get { return this.GetRequiredService<IReportService>("ExportService"); } }
        protected virtual IOpenFileDialogService OpenFileDialogService { get { return null; } }
        public virtual ObservableCollection<AttachedFileInfo> FilesInfo { get; set; }
        public virtual IEnumerable<Employee> EntityContextLookUpEmployees { get; set; }
        public virtual IEnumerable<Employee> AssignedEmployees { get; set; }
        public virtual string RtfTextDescription { get; set; }
        public virtual string TextDescription { get; set; }

        public AttachedFileInfo SelectedFile { get; set; }
        bool allowEntityChange;
        bool disableAttachedFilesReload;
        bool allowParameterChanged;
        LinksViewModel linksViewModel;
        List<string> createdFilePaths;
        public string actualRtfTextDescription;

        protected override EmployeeTask CreateEntity() {
            EmployeeTask entity = base.CreateEntity();
            entity.StartDate = DateTime.Now;
            entity.DueDate = DateTime.Now + new TimeSpan(48, 0, 0);
            entity.Category = Colors.Transparent.ToString();
#if !DXCORE3
            var id = EmployeeTaskDetailsCollectionViewModel.LastSelectedEmployeeId;
            if(id != null) {
                var employee = this.UnitOfWork.Employees.SingleOrDefault(x => x.Id == id.Value);
                if(employee != null)
                    entity.AssignedEmployees.Add(employee);
                EmployeeTaskDetailsCollectionViewModel.LastSelectedEmployeeId = null;
            }
#endif
            return entity;
        }

        protected override void OnEntityChanged() {
            base.OnEntityChanged();
            allowEntityChange = false;
            if(!disableAttachedFilesReload) {
                FilesInfo = new ObservableCollection<AttachedFileInfo>();
                if(Entity != null && Entity.AttachedFiles != null)
                    foreach(var file in Entity.AttachedFiles)
                        FilesInfo.Add(FilesHelper.GetAttachedFileInfo(file.Name, "", file.Id));
            }
            this.RaisePropertyChanged(vm => vm.ReminderTime);
            this.RaisePropertyChanged(vm => vm.Status);
            this.RaisePropertyChanged(vm => vm.Completion);
            EntityContextLookUpEmployees = this.UnitOfWork.Employees.AsEnumerable();
            if(Entity != null) {
                allowParameterChanged = false;
                AssignedEmployees = Entity.AssignedEmployees;
                RtfTextDescription = Entity.RtfTextDescription;
                Action rtfAction = () => {
                    actualRtfTextDescription = RtfTextDescription;
                    allowParameterChanged = true;
                };
                Dispatcher.CurrentDispatcher.BeginInvoke(rtfAction);
                Logger.Log(string.Format("OutlookInspiredApp: Edit Task: {0}",
                    string.IsNullOrEmpty(Entity.Subject) ? "<New>" : Entity.Subject));
            }
            allowEntityChange = true;
        }
        public virtual void OnAssignedEmployeesChanged() {
            if(!allowParameterChanged || AssignedEmployeesAreEqual())
                return;
            if(allowEntityChange && Entity != null) {
                Entity.AssignedEmployees = AssignedEmployees.ToList();
                SetCollectionChange();
            }
        }
        public virtual void OnRtfTextDescriptionChanged() {
            if(!allowParameterChanged || Entity == null || !allowEntityChange)
                return;
            if(actualRtfTextDescription != RtfTextDescription) {
                Entity.RtfTextDescription = RtfTextDescription;
                actualRtfTextDescription = RtfTextDescription;
                Update();
            }
        }
        public virtual void OnTextDescriptionChanged() {
            if(Entity != null && TextDescription != Entity.Description && allowParameterChanged)
                Entity.Description = TextDescription;
        }
        public virtual void Unloaded() {
            FilesHelper.DeleteTempFiles(ref createdFilePaths);
        }
        public DateTime? ReminderTime {
            get {
                if(this.Entity == null || this.Entity.ReminderDateTime == null)
                    return null;
                return this.Entity.ReminderDateTime.Value;
            }
            set {
                if(this.Entity == null || value == null || this.Entity.ReminderDateTime == null)
                    return;
                DateTime reminderDateTime = (DateTime)this.Entity.ReminderDateTime;
                this.Entity.ReminderDateTime = new DateTime(reminderDateTime.Year, reminderDateTime.Month, reminderDateTime.Day, ((DateTime)value).Hour, ((DateTime)value).Minute, ((DateTime)value).Second);
            }
        }

        public EmployeeTaskStatus Status {
            get {
                return (this.Entity == null) ? EmployeeTaskStatus.NotStarted : this.Entity.Status;
            }
            set {
                if(this.Entity == null) return;
                string oldStatus = this.Entity.Status.ToString();
                this.Entity.Status = value;
                if(oldStatus == (EmployeeTaskStatus.Completed).ToString() && this.Entity.Status != EmployeeTaskStatus.Completed)
                    this.Entity.Completion = 50;
                if(this.Entity.Status == EmployeeTaskStatus.Completed && this.Entity.Completion != 100)
                    this.Entity.Completion = 100;
                this.RaisePropertyChanged(vm => vm.Entity);
                this.RaisePropertyChanged(vm => vm.Entity.Status);
                this.RaisePropertyChanged(vm => vm.Entity.Completion);
            }
        }
        public int Completion {
            get {
                return (this.Entity != null) ? this.Entity.Completion : 0;
            }
            set {
                if(this.Entity == null) return;
                this.Entity.Completion = value;
                if(this.Entity.Completion == 100)
                    this.Entity.Status = EmployeeTaskStatus.Completed;
                if(this.Entity.Completion != 100 && this.Entity.Status == EmployeeTaskStatus.Completed)
                    this.Entity.Status = EmployeeTaskStatus.InProgress;
                this.RaisePropertyChanged(vm => vm.Entity.Status);
                this.RaisePropertyChanged(vm => vm.Entity);
            }
        }
        public virtual void FollowUp(string name) {
            switch(name) {
                case "Today":
                    SetFollowUpDateTime(DateTime.Now, DateTime.Now, true, DateTime.Now, EndTimeOfWorkDay.AddHours(-1));
                    break;
                case "Tomorrow":
                    SetFollowUpDateTime(DateTime.Now.AddDays(1), DateTime.Now.AddDays(1), true, NextWorkDay(DateTime.Now), StartTimeOfWorkDay);
                    break;
                case "ThisWeek":
                    SetFollowUpDateTime(GetThisWeekStartDate(), GetBorderWorkDay(false), true, GetThisWeekStartDate(), StartTimeOfWorkDay);
                    break;
                case "NextWeek":
                    SetFollowUpDateTime(GetBorderWorkDay(true, true), GetBorderWorkDay(false, true), true, GetBorderWorkDay(true, true), StartTimeOfWorkDay);
                    break;
                case "NoDate":
                    SetFollowUpDateTime(null, null, false, null, null);
                    break;
                case "Custom":
                    SetFollowUpDateTime(DateTime.Now, DateTime.Now, true, DateTime.Now, DateTime.Now);
                    break;
            }
            this.RaisePropertyChanged(vm => vm.Entity);
        }
        public virtual void MarkComplete() {
            Status = EmployeeTaskStatus.Completed;
            this.RaisePropertyChanged(vm => vm.Status);
            this.RaisePropertyChanged(vm => vm.Completion);
        }
        public virtual void AttachFiles(DragEventArgs e) {
            string[] filesName = new string[] { };
            if(e != null && e.Data.GetDataPresent(DataFormats.FileDrop))
                filesName = (string[])(e.Data.GetData(DataFormats.FileDrop));
            else {
                bool dialogResult = OpenFileDialogService.ShowDialog();
                if(dialogResult)
                    filesName = OpenFileDialogService.Files.Select(x => Path.Combine(x.DirectoryName, x.Name)).ToArray();
            }
            AttachedFilesCore(filesName);
        }
        public virtual void PrintTaskDetail() {
            ShowReport(ReportInfoFactory.TaskDetailReport(Entity), "TaskDetail");
        }
        void AttachedFilesCore(string[] names) {
            bool fileLengthExceed = false;
            FileInfo info;
            foreach(string name in names) {
                info = new FileInfo(name);
                if(info.Length > FilesHelper.MaxAttachedFileSize * 1050578)
                    fileLengthExceed = true;
                else
                    FilesInfo.Add(FilesHelper.GetAttachedFileInfo(info.Name, info.DirectoryName));
            }
            if(fileLengthExceed)
                MessageBoxService.ShowMessage(string.Format("The size of one of the files exceeds {0} MB.", FilesHelper.MaxAttachedFileSize), "Error attaching files");
            SetCollectionChange();
            Update();
        }

        public virtual void OpenFile() {
            if(SelectedFile.Id == -1) {
                FilesHelper.OpenFileFromDisc(SelectedFile.FullPath);
                return;
            }
            if(createdFilePaths == null)
                createdFilePaths = new List<string>();
            createdFilePaths.Add(FilesHelper.OpenFileFromDb(SelectedFile.Name, Entity.AttachedFiles.Single(x => x.Id == SelectedFile.Id).Content));
        }
        public virtual void DeleteFile() {
            FilesInfo.Remove(SelectedFile);
            SetCollectionChange();
            Update();
        }
        public override void Delete() {
            if(MessageBoxService.ShowMessage(string.Format(ScaffoldingLocalizer.GetString(ScaffoldingStringId.Confirmation_Delete), EntityDisplayName), GetConfirmationMessageTitle(), MessageButton.YesNo) != MessageResult.Yes)
                return;
            try {
                if(Entity.AttachedFiles != null) {
                    long[] deletedFileKeys = Entity.AttachedFiles.Select(x => x.Id).ToArray();
                    foreach(var key in deletedFileKeys) {
                        var file = UnitOfWork.AttachedFiles.Find(key);
                        UnitOfWork.AttachedFiles.Remove(file);
                    }
                }
                OnBeforeEntityDeleted(PrimaryKey, Entity);
                Repository.Remove(Entity);
                UnitOfWork.SaveChanges();
                long primaryKeyForMessage = PrimaryKey;
                EmployeeTask entityForMessage = Entity;
                Entity = null;
                OnEntityDeleted(primaryKeyForMessage, entityForMessage);
                Close();
            } catch(DbException e) {
                MessageBoxService.ShowMessage(e.ErrorMessage, e.ErrorCaption, MessageButton.OK, MessageIcon.Error);
            }
        }
        void SetCollectionChange() {
            Entity.AttachedCollectionsChanged = !Entity.AttachedCollectionsChanged;
        }
        protected override bool SaveCore() {
            disableAttachedFilesReload = true;
            bool saveCoreResult = base.SaveCore();
            if(!saveCoreResult)
                return false;
            bool hasFilesOperations = false;
            disableAttachedFilesReload = false;
            if(Entity.AttachedFiles != null) {
                List<long> deletedItems = new List<long>();
                foreach(var file in Entity.AttachedFiles) {
                    long id = file.Id;
                    if(FilesInfo.FirstOrDefault(x => id == x.Id) == null)
                        deletedItems.Add(file.Id);
                }
                foreach(long item in deletedItems) {
                    var file = UnitOfWork.AttachedFiles.Find(item);
                    UnitOfWork.AttachedFiles.Remove(file);
                    hasFilesOperations = true;
                }
            }
            foreach(var fileInfo in FilesInfo) {
                if(fileInfo.Id == -1) {
                    try {
                        using(FileStream stream = new FileStream(fileInfo.FullPath, FileMode.Open, FileAccess.Read)) {
                            TaskAttachedFile attachedFile = UnitOfWork.AttachedFiles.Create();
                            attachedFile.EmployeeTaskId = Entity.Id;
                            attachedFile.Name = fileInfo.Name;
                            attachedFile.Content = new byte[(int)stream.Length];
                            stream.Read(attachedFile.Content, 0, (int)stream.Length);
                            hasFilesOperations = true;
                        }
                    } catch(Exception) {
                        MessageBoxService.ShowMessage("Error attaching file!", "Error");
                        return false;
                    }
                }
            }
            if(hasFilesOperations)
                UnitOfWork.SaveChanges();
            return true;
        }
        bool AssignedEmployeesAreEqual() {
            if(Entity == null || Entity.AssignedEmployees == null || AssignedEmployees == null)
                return true;
            var hasItems1 = (Entity.AssignedEmployees).Except(AssignedEmployees).Any();
            var hasItems2 = AssignedEmployees.Except(Entity.AssignedEmployees).Any();
            return !(hasItems1 || hasItems2);
        }
        public virtual bool CanMarkComplete() {
            return this.Entity != null && this.Entity.Status != EmployeeTaskStatus.Completed;
        }
        public virtual bool CanOpenFile() {
            return SelectedFile != null;
        }
        public virtual bool CanDeleteFile() {
            return SelectedFile != null;
        }
        protected override string GetTitle() {
#if DXCORE3
            return Entity.Owner != null ? Entity.Owner.FullName : (Entity.Subject ?? string.Empty);
#else
            return Entity.Subject ?? string.Empty;
#endif
        }
        protected override string GetConfirmationMessageTitle() {
            var baseTitle = base.GetConfirmationMessageTitle();
            if(string.IsNullOrEmpty(baseTitle)) return baseTitle;
            return baseTitle.Length <= 30 ? baseTitle : baseTitle.Substring(0, 30) + "...";
        }
        public LinksViewModel LinksViewModel {
            get {
                if(linksViewModel == null)
                    linksViewModel = LinksViewModel.Create();
                return linksViewModel;
            }
        }
        void SetFollowUpDateTime(DateTime? startDate, DateTime? dueDate, bool reminder, DateTime? reminderDateTime, DateTime? reminderTime) {
            this.Entity.StartDate = startDate;
            this.Entity.DueDate = dueDate;
            this.Entity.Reminder = reminder;
            this.Entity.ReminderDateTime = reminderDateTime;
            this.ReminderTime = reminderTime;
            this.RaisePropertyChanged(vm => vm.ReminderTime);
        }
        DateTime NextWorkDay(DateTime date) {
            do {
                date = date.AddDays(1);
            }
            while(IsHoliday(date));
            return date;
        }
        bool IsHoliday(DateTime dateTime) { // TODO full holiday detector
            return dateTime.DayOfWeek == DayOfWeek.Saturday || dateTime.DayOfWeek == DayOfWeek.Sunday;
        }
        DateTime GetBorderWorkDay(bool firstWorkDay = true, bool nextWeek = false) {
            DateTime dateTime = DateTime.Now;
            while(IsHoliday(dateTime))
                dateTime = dateTime.AddDays(1);
            if(nextWeek)
                dateTime = dateTime.AddDays(7);
            while(!IsHoliday(dateTime))
                dateTime = dateTime.AddDays(firstWorkDay ? -1 : 1);
            dateTime = dateTime.AddDays(firstWorkDay ? 1 : -1);
            return dateTime;
        }
        DateTime GetThisWeekStartDate() {
            DateTime dateTime = DateTime.Now;
            int realCounter = 0;
            while(IsHoliday(dateTime))
                dateTime = dateTime.AddDays(1);
            do {
                dateTime = dateTime.AddDays(1);
                if(!IsHoliday(dateTime))
                    realCounter++;
            } while(!IsHoliday(dateTime) && realCounter < 2);
            if(realCounter < 2)
                dateTime = dateTime.AddDays(-1);
            return dateTime;
        }
        void ShowReport(IReportInfo reportInfo, string reportId) {
            ExportService.ShowReport(reportInfo);
            PrintService.ShowReport(reportInfo);
            Logger.Log(string.Format("OutlookInspiredApp: Create Report : Task: {0}", reportId));
        }
    }
}