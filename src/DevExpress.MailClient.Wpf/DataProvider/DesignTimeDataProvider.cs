using DevExpress.MailClient.Helpers;
using DevExpress.MailClient.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Imaging;

namespace DevExpress.MailClient.DataProvider {
    public class DesignTimeDataProvider : DataProviderBase {

        protected override IList<TaskItemViewModel> FillTasks(IEnumerable<ContactItemViewModel> contacts) {
            return TaskGenerator.GenerateTasks(contacts.ToList());
        }
        protected override IList<MailMessageViewModel> FillMessages() {
            IList<MailMessageViewModel> result = new List<MailMessageViewModel>();
            var message = MailMessageViewModel.Create();
            message.Date = DateTime.Today.Date.AddHours(19).AddMinutes(19);
            message.From = "John Heart";
            message.To = "peter_thorpe@dxmail.com";
            message.Subject = "AsyncMode for Pivot Grid";
            message.Text = DataHelper.GetMHTTextFromHTML("Cool will it work as normal if you use chart integration or need some modification?");
            message.Folder = MessageFolderName.Management;
            message.IsReply = true;
            message.Priority = MessagePriority.Low;
            result.Add(message);

            message = MailMessageViewModel.Create();
            message.Date = DateTime.Today.Date.AddHours(17).AddMinutes(54);
            message.From = "Kobus Smit";
            message.Subject = "XAF – Model Merge Tool";
            message.Text = DataHelper.GetMHTTextFromHTML("Great! This is an useful enhancement that will save us time. One more step making XAF more RAD.");
            message.Folder = MessageFolderName.IT;
            message.IsReply = true;
            message.Priority = MessagePriority.Medium;
            result.Add(message);

            message = MailMessageViewModel.Create();
            message.Date = DateTime.Today.Date.AddHours(17).AddMinutes(11);
            message.From = "Tony Taylor-Moran";
            message.Subject = "WinForms Tab Control - Custom Buttons";
            message.Text = DataHelper.GetMHTTextFromHTML("Sorry, I don't agree with Chris at all, the assumption that Winforms is legacy doesn’t site right with me, " +
                "quite a lot of my users are not interested, so I am more inclined to agree with the others. More attention really" +
                " does need to be given to the Winforms collection of controls and in particular the MDITab control (My personal request!), the recent" +
                " offerings haven’t seemed to give me much for the money (although I have a DXperience Enterprise Subscription, I really only use the " +
                "WinForms controls) – &amp;nbsp;my subscription is due for renewal in May, unless I see something that is worth the renewal fee, I will " +
                "give one of the other vendors a try.");
            message.Folder = MessageFolderName.Sales;
            message.IsReply = true;
            message.IsUnread = true;
            message.Priority = MessagePriority.Medium;
            result.Add(message);

            message = MailMessageViewModel.Create();
            message.Date = DateTime.Today.Date.AddDays(-2).AddHours(14).AddMinutes(20);
            message.From = "Mehul Harry (DevExpress)";
            message.Subject = "ASP.NET MVC - Code Usability Improvement";
            message.Text = DataHelper.GetMHTTextFromHTML("Graeme, Thanks! And it's possible so we'll consider it for a future release.");
            message.Folder = MessageFolderName.Support;
            message.IsUnread = true;
            message.HasAttachment = true;
            message.Priority = MessagePriority.High;
            result.Add(message);
            return result;
        }
        protected override IList<AppointmentViewModel> FillAppointments() {
            var result = new List<AppointmentViewModel>();
            var subjects = new List<string>() {
                "Webinar - Julian on JavaScript - Learning from reading: underscore.js",
                "Webinar - Advanced CodeRush Plug-ins – Working with the TextDocument and TextView",
                "Webinar - Rich data visualization with Pivot Grid and Charts",
                "Webinar - Ask the ASP.NET Team"
            };
            int dayOffset = -2;

            foreach(var subject in subjects){
                var appointment = AppointmentViewModel.Create();
                appointment.EventType = 0;
                appointment.StartDate = DateTime.Today.Date.AddDays(dayOffset).AddHours(9);
                appointment.EndDate = DateTime.Today.Date.AddDays(dayOffset).AddHours(10);
                appointment.AllDay = false;
                appointment.Subject = subject;
                appointment.Status = 0;
                appointment.Label = 2;
                result.Add(appointment);
                dayOffset++;
            }

            return result;
        }

        protected override IList<ContactItemViewModel> FillContacts() {
            var result = new List<ContactItemViewModel>();
            var contact = ContactItemViewModel.Create();
            contact.Name.FirstName = "Andrew";
            contact.Name.MiddleName = "Dennis";
            contact.Name.LastName = "Carter";
            contact.Email = "andrew_carter@dxmail.com";
            contact.Prefix = DevAV.PersonPrefix.Mr;
            contact.Address = AddressViewModel.Create("7121 Bailey St,  Worcester, MA 01605");
            contact.Phone = "(555)578-3946";
            contact.Photo = DataHelper.UnknownUserPicture;
            contact.BirthDate = new DateTime(1967, 9, 19);
            contact.Notes = "Name: Andrew Dennis Carter \nBirth date: 9/19/1967";
            result.Add(contact);
            contact = ContactItemViewModel.Create();
            contact.Name.FirstName = "Annie";
            contact.Name.MiddleName = "O";
            contact.Name.LastName = "Vicars";
            contact.Email = "annie_vicars@dxmail.com";
            contact.Prefix = DevAV.PersonPrefix.Ms;
            contact.Address = AddressViewModel.Create("13202 Fm #1518,  Adkins, TX 78105");
            contact.Phone = "(555)922-1349";
            contact.Photo = DataHelper.UnknownUserPicture;
            contact.BirthDate = new DateTime(1971, 11, 24);
            result.Add(contact);

            return result;
        }
    }
}
