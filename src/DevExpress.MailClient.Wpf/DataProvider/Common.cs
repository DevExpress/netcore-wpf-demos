using DevExpress.MailClient.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DevExpress.MailClient.DataProvider {
    public interface IDataProvider {
        IEnumerable<T> GetItems<T>();
    }
    public static class DataSource {
        static IDataProvider instance;
        public static IDataProvider GetDefaultDataProvider() {
            return instance ?? (instance = CreateDefaultDataProvider());
        }
        static IDataProvider CreateDefaultDataProvider() {
            return new XmlDataProvider();
        }
    }
    public abstract class DataProviderBase : IDataProvider {
        #region Props
        IList<MailMessageViewModel> messages;
        IList<TaskItemViewModel> tasks;
        IList<AppointmentViewModel> appointments;
        IList<ContactItemViewModel> contacts;
        #endregion

        IEnumerable<TaskItemViewModel> GetTasks() { return tasks ?? (tasks = FillTasks(GetContacts())); }
        IEnumerable<AppointmentViewModel> GetAppointments() { return appointments ?? (appointments = FillAppointments()); }
        IEnumerable<ContactItemViewModel> GetContacts() { return contacts ?? (contacts = FillContacts()); }
        IEnumerable<MailMessageViewModel> GetMailMessages() { return messages ?? (messages = FillMessages()); }

        public virtual IEnumerable<T> GetItems<T>() {
            Type requestedType = typeof(T);

            if(IsDerivedFrom<TaskItemViewModel, T>())
                return GetTasks().Cast<T>();
            if(IsDerivedFrom<AppointmentViewModel, T>())
                return GetAppointments().Cast<T>();
            if(IsDerivedFrom<ContactItemViewModel, T>())
                return GetContacts().Cast<T>();
            if(IsDerivedFrom<MailMessageViewModel, T>())
                return GetMailMessages().Cast<T>();

            throw new NotSupportedException();
        }

        protected abstract IList<TaskItemViewModel> FillTasks(IEnumerable<ContactItemViewModel> contacts);
        protected abstract IList<AppointmentViewModel> FillAppointments();
        protected abstract IList<ContactItemViewModel> FillContacts();
        protected abstract IList<MailMessageViewModel> FillMessages();

        protected static bool IsDerivedFrom<TBase, TAncestor>() {
            Type baseType = typeof(TBase);
            Type ancestorType = typeof(TAncestor);
            return baseType.Equals(ancestorType) || baseType.IsAssignableFrom(ancestorType);
        }
    }
}
