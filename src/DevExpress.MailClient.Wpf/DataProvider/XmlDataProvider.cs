using DevExpress.DevAV;
using DevExpress.MailClient.Helpers;
using DevExpress.MailClient.ViewModel;
using DevExpress.Xpf.Core.Native;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;

namespace DevExpress.MailClient.DataProvider {
    public class XmlDataProvider : DataProviderBase {
        DataSet messagesDataSet;
        List<Employee> employees;

        int initItemsCount = 0;

        public XmlDataProvider(string mailPath = "MailDevAv.xml") {
            messagesDataSet = InitDataSet(mailPath);
            employees = DataHelper.Employees;
        }

        void ReleaseDataIfNeeded() {
            if(++initItemsCount == 4) {
                messagesDataSet.Dispose();
                messagesDataSet = null;
            }
        }
        #region Fill data
        protected override IList<TaskItemViewModel> FillTasks(IEnumerable<ContactItemViewModel> contacts) {
            var result = TaskGenerator.GenerateTasks(contacts.ToList());
            ReleaseDataIfNeeded();
            return result;
        }
        protected override IList<MailMessageViewModel> FillMessages() {
            IList<MailMessageViewModel> result = new List<MailMessageViewModel>();
            DataTable messagesTable = messagesDataSet.Tables["Messages"];
            if(messagesTable != null && messagesTable.Rows.Count > 0) {
                foreach(DataRow row in messagesTable.Rows)
                    result.Add(CreateMessage(row));
            }
            ReleaseDataIfNeeded();
            return result;
        }
        protected override IList<AppointmentViewModel> FillAppointments() {
            var result = new List<AppointmentViewModel>();
            DataTable appointmentsTable = messagesDataSet.Tables["Appointments"];
            if(appointmentsTable != null && appointmentsTable.Rows.Count > 0) {
                foreach(DataRow row in appointmentsTable.Rows)
                    result.Add(CreateAppointment(row));
            }

            ReleaseDataIfNeeded();
            return result;
        }
        protected override IList<ContactItemViewModel> FillContacts() {
            var result = new List<ContactItemViewModel>();
            employees.ForEach(e => {
                var contact = CreateContact(e);
                FillPersonInformation(contact, e);
                result.Add(contact);
            });
            ReleaseDataIfNeeded();
            return result;
        }
        #endregion

        #region Static
        static DataSet InitDataSet(string path) {
            var result = new DataSet();
            string fullPath = FilePathHelper.GetFullPath(path);
            if(fullPath != string.Empty) {
                FileInfo fi = new FileInfo(fullPath);
                result.ReadXml(fi.FullName);
            }
            return result;
        }
        static ContactItemViewModel CreateContact(Employee employee) {
            var contact = ContactItemViewModel.Create();
            contact.Name.MiddleName = string.Empty;
            contact.Email = employee.Email;

            contact.Address = AddressViewModel.Create(employee.Address.ToString());
            contact.Phone = employee.MobilePhone;
            if(employee.Picture != null) 
                contact.Photo = ImageHelper.CreateImageFromStream(new MemoryStream(employee.Picture.Data as byte[]));
            return contact;
        }
        static void FillPersonInformation(ContactItemViewModel contact, Employee employee) {
            contact.Name.FirstName = employee.FirstName;
            contact.Name.LastName = employee.LastName;
            contact.Prefix = employee.Prefix;
            contact.BirthDate = employee.BirthDate.Value;
        }
        static AppointmentViewModel CreateAppointment(DataRow row) {
            AppointmentViewModel appointment = AppointmentViewModel.Create();
            appointment.EventType = (int?)row["EventType"];
            appointment.StartDate = (DateTime?)row["StartDate"];
            appointment.EndDate = (DateTime?)row["EndDate"];
            appointment.AllDay = (bool?)row["AllDay"];
            appointment.Subject = Convert.ToString(row["Subject"]);
            appointment.Location = Convert.ToString(row["Location"]);
            appointment.Description = Convert.ToString(row["Description"]);
            appointment.Status = (int?)row["Status"];
            appointment.Label = (int?)row["Label"];
            appointment.RecurrenceInfo = Convert.ToString(row["RecurrenceInfo"]);
            appointment.ReminderInfo = Convert.ToString(row["ReminderInfo"]);
            appointment.ContactInfo = Convert.ToString(row["ContactInfo"]);
            return appointment;
        }
        static MailMessageViewModel CreateMessage(DataRow row) {
            MailMessageViewModel message = MailMessageViewModel.Create();
            message.Date = DateTime.Now.AddDays((int)row["Day"]).AddSeconds(-new Random().Next(10000));
            message.From = string.Format("{0}", row["From"]);
            message.Subject = string.Format("{0}", row["Subject"]);
            message.IsUnread = (DateTime.Now - message.Date) < TimeSpan.FromHours(200);
            message.Text = string.Format("{0}", row["Text"]);
            message.HasAttachment = DataHelper.HasImages(message.Text);
            message.Folder = GetFolder(row);
            message.IsReply = (message.Subject).StartsWith("RE", StringComparison.InvariantCultureIgnoreCase);
            message.Priority = message.IsReply ? MessagePriority.High : MessagePriority.Medium;
            message.To = "Me";
            return message;
        }
        static MessageFolderName GetFolder(DataRow row) {
            object category = row["CategoryID"];
            string categoryString = string.Format("{0}", (Categories)(category == DBNull.Value ? 1 : (int)category));
            if(string.IsNullOrEmpty(categoryString)) return MessageFolderName.All;
            return (MessageFolderName)Enum.Parse(typeof(MessageFolderName), categoryString.Replace(" ", ""), true);
        }
#endregion
    }
}
